using System.Text.Json;

namespace observe.ConfigurationBuilder
{
    public class ConfigurationList
    {
        Dictionary<string, (object? model, FileInfo file)> _models = new();
        object? _defaultModel;
        bool _areExceptionsDisabled;

        public delegate void ConfigurationFileUpdateHandler(ConfigurationFileUpdateEventArgs args);
        public event ConfigurationFileUpdateHandler ConfigurationFileUpdate;

        List<FileSystemWatcher> _watchers = new();
        public ConfigurationList(Dictionary<string, (object?, FileInfo)> models, object? defaultModel, bool areExceptionsDisabled)
        {
            //  Перебираем модели
            foreach (var key in models.Keys)
            {
                //  Сохраняем модели в приватный массив
                _models[key] = models[key];

                //  Ищим в массиве FileWatcher, смотрящий за директорией перебираемой модели
                var fwatcher = _watchers.Find(w => w.Path == models[key].Item2.DirectoryName);
                if (fwatcher == null)
                {
                    // Если FileWatcher не найден, создаём его и заносим в массив
                    var watcher = new FileSystemWatcher(models[key].Item2.DirectoryName);
                    watcher.EnableRaisingEvents = true;
                    watcher.Changed += OnFileUpdate;
                    watcher.Filters.Add(models[key].Item2.Name);

                    this._watchers.Add(watcher);
                }
                else
                {
                    //  Если найден, тогда добавляем ему в фильтры файл конфига из модели
                    fwatcher.Filters.Add(models[key].Item2.Name);
                }
            }
            this._defaultModel = defaultModel;
            this._areExceptionsDisabled = areExceptionsDisabled;
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
                            List<(string,object?)> fieldsUpdate = new();
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
                                            //  Обновляем в основной модели значение
                                            prop.SetValue(model.Value.model, dProp.GetValue(deserializedModel));
                                            //  Собираем названия и значения обновлённых полей
                                            fieldsUpdate.Add((prop.Name, prop.GetValue(model.Value.model)));
                                        }
                                    }
                                }
                            }
                            //  Вызываем обработчик события
                            var args = new ConfigurationFileUpdateEventArgs(model.Value.file, model.Value.model, fieldsUpdate);
                            ConfigurationFileUpdate?.Invoke(args);

                        } catch
                        {
                            //  Если возникла ошибка чтения JSON-файла, выкинуть исключение
                            if (!_areExceptionsDisabled)
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
