#nullable enable
using System.Text.Json;

namespace observe.ConfigurationBuilder
{
    public class ConfigurationBuilder
    {
        Dictionary<string, (object?, FileInfo)> models = new();
        object? defaultModel;
        bool areExceptionsDisabled = false;

        public ConfigurationBuilder AddJsonFile<FileModel>(string filePath, string? name = null) where FileModel : IObserverFields
        {
            try
            {
                var file = new FileInfo(filePath);
                AddJsonFile<FileModel>(file, name);

            } catch (UnableAddConfigurationFile ex)
            {
                if (!areExceptionsDisabled)
                    throw ex;
            } catch
            {
                if (!areExceptionsDisabled)
                    throw new UnableAddConfigurationFile($"Unable to add {filePath} file, check if the file path is written correctly");
            }

            return this;
        }

        public ConfigurationBuilder AddJsonFile<FileModel>(FileInfo file, string? name = null) where FileModel : IObserverFields
        {
            var confName = String.IsNullOrWhiteSpace(name) ? Path.GetFileNameWithoutExtension(file.Name) : name;

            if (!file.Exists)
            {
                var instance = (FileModel)Activator.CreateInstance(typeof(FileModel));
                var jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
                var jsonContent = JsonSerializer.Serialize(instance, jsonOptions);

                using (var writer = file.CreateText())
                    writer.Write(jsonContent);

                models.Add(confName, (instance, file));
            } else
            {
                try
                {
                    using (var reader = file.OpenText())
                    {
                        FileModel? model = JsonSerializer.Deserialize<FileModel>(reader.ReadToEnd());
                        models.Add(confName, (model, file));
                    }
                }
                catch (JsonException)
                {
                    if (!areExceptionsDisabled)
                        throw new UnableAddConfigurationFile($"Unable to add {file.Name} file to ConfigurationBuilder, because json file has invalid format");
                }
            }

            return this;
        }

        public ConfigurationBuilder SetDefaultConfiguration(string configurationName)
        {
            try
            {
                this.defaultModel = models[configurationName].Item1;
            } catch
            {
                if (!areExceptionsDisabled)
                    throw new UnableSetDefaultConfigurationException($"Unable to set default configuration with {configurationName} name");
            }

            return this;
        }

        public ConfigurationBuilder DisableException()
        {
            areExceptionsDisabled = true;
            return this;
        }

        public ConfigurationList Build() => new ConfigurationList(models, defaultModel, areExceptionsDisabled);
    }
}