using System.Text.Json;

namespace observe.ConfigurationBuilder
{
    public class ConfigurationList
    {
        Dictionary<string, (object? model, FileInfo file)> _models = new();
        object? _defaultModel;

        public delegate void ConfigurationFileUpdateHandler(ConfigurationFileUpdateEventArgs args);
        public event ConfigurationFileUpdateHandler ConfigurationFileUpdate;

        List<FileSystemWatcher> _watchers = new();
        public ConfigurationList(Dictionary<string, (object?, FileInfo)> models, object? defaultModel = null)
        {
            foreach (var key in models.Keys)
            {
                _models[key] = models[key];
                Console.WriteLine(key);
                Console.WriteLine(models[key]);

                using (var fwatcher = _watchers.Find(w => w.Path == models[key].Item2.DirectoryName))
                    if (fwatcher == null)
                    {
                        var watcher = new FileSystemWatcher(models[key].Item2.DirectoryName);
                        watcher.EnableRaisingEvents = true;
                        watcher.Changed += OnFileUpdate;
                        watcher.Filters.Add(models[key].Item2.Name);

                        this._watchers.Add(watcher);
                    }
                    else
                    {
                        fwatcher.Filters.Add(models[key].Item2.Name);
                    }
            }
            this._defaultModel = defaultModel;
        }

        void OnFileUpdate(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;

            //  Перебираем массив моделей
            foreach (var model in _models)
            {
                //  Поиск модели, у которой путь совпадает с путём изменённого файла
                if (model.Value.file.FullName == e.FullPath)
                {
                    //  Получение типа модели
                    var modelType = model.Value.model.GetType();
                    //  Получение StreamReader`а изменнёного файла
                    using (var jsonReader = model.Value.file.OpenText())
                    {
                        //  Обработка ошибок при десерилизации
                        try
                        {
                            //  Получение десерилизованной модели из изменённого файла
                            var deserializedModel = JsonSerializer.Deserialize(jsonReader.ReadToEnd(), modelType);
                            Dictionary<string, object?> fieldsUpdate = new();
                            //  Перебор свойств модели
                            foreach (var prop in modelType.GetProperties())
                            {
                                //  Перебор свойств десерилизованной модели
                                foreach (var dProp in deserializedModel.GetType().GetProperties())
                                {
                                    //  Поиск одинаковых свойств
                                    if (prop.Name == dProp.Name)
                                    {
                                        //  Если значения разные - обновить значение у модели
                                        if (prop.GetValue(model.Value.model) != dProp.GetValue(deserializedModel))
                                        {
                                            prop.SetValue(model.Value.model, dProp.GetValue(deserializedModel));
                                            fieldsUpdate.Add(prop.Name, prop.GetValue(model.Value.model));
                                        }
                                    }
                                }
                            }
                            var args = new ConfigurationFileUpdateEventArgs(model.Value.file, model, fieldsUpdate);
                            ConfigurationFileUpdate?.Invoke(args);

                        } catch
                        {
                            //  Если возникла ошибка чтения JSON-файла, выкинуть исключение
                            throw new ErrorConfigurationFileUpdateException($"After editing {e.FullPath} file, it has become impossible to read");
                        }

                    }

                    //  Выходим из цикла поиска нужной модели
                    break;
                }
            }
        }

        public List<string> GetConfigurationList()
        {
            List<string> names = new();
            foreach (var key in _models.Keys)
                names.Add(key);

            return names;
        }

        public DataModel? Get<DataModel>(string name) where DataModel : IObserverFields
        {
            try
            {
                return (DataModel)_models[name].model;
            }
            catch
            {
                return default(DataModel);
            }
        }

        public object? GetDefault() => _defaultModel;
        public DataModel? GetDefault<DataModel>() => (DataModel)_defaultModel;
    }
}
