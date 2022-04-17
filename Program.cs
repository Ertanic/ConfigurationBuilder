using observe;
using observe.ConfigurationBuilder;
using observe.ConfigurationsModels;

var configuration = new ConfigurationBuilder()
    .DisableException() //  Должен находиться исключительно в самом начале для нормальной работы

    .AddJsonFile<ConfigModel>(Path.GetFullPath("config.json"))  //  Добавляет конфиг-файл config.json и маппит его к модели ConfigModel,
                                                                //  если файл не удалось обнаружить - его создаёт строитель и заполняет дефолтными значениями
                                                                //  определённые в модели. Игнорирует поля, работает только со свойствами.
                                                                //  Также, если не установлен ключ, по которому будет происходить обращение к модели конфига,
                                                                //  будет взято имя файла без расширения в качестве ключа модели.

    .AddJsonFile<AppleModel>(Path.GetFullPath("apple.json"), "Apples")  //  В данном случае установлен ключ Apples, по которому будет происходить обращение к модели.

    .SetDefaultConfiguration("config")  //  Если установить дефолтный конфиг до добавления конфигов, будет плюваться исключениями.
    .Build();   //  Должен находиться в самом конце, возвращает объект типа ConfigurationList.

//  Подпись на событие обновления файлов конфигов
configuration.ConfigurationFileUpdate += (args) => {
    Console.WriteLine($"Field is updated:");

    //  Аргументы содержат в себе:
    //  1. Информацию о файле конфига, представляет из себя FileInfo;
    Console.WriteLine($"\tPath: {args.file.FullName}");

    //  2. Саму модель конфига. Можно вывести информацию об анонимной модели с помощью вспомогательного
    //  класса Consolable и его метода GetPrintableObject;
    Console.WriteLine($"\tPath: {(args.Configuration as Consolable)?.GetPrintableObject()}");

    //  3. Массив с кортежами, содержащие информацию об обновлённых полях (имя свойства, новое значение).
    Console.WriteLine($"\tPath: {args.FieldsUpdate.Join(", ", (model) => $"{model.fieldName}: {model.newValue}")}");
};

//  Получение списка ключей конфигов.
Console.WriteLine("Keys: " + configuration.GetConfigurationList().Join(", "));
//  Получение дефолтного конфига.                                                           
Console.WriteLine(configuration.GetDefault<ConfigModel>()?.GetPrintableObject("Config"));   //
                                                                                            //  Во всех случаях вернётся модель, однако нужно явно указать её тип,
                                                                                            //  иначе вернётся пращур всех классов - object.
//  Получение конфига по ключу Apples.                                                      //
Console.WriteLine(configuration.Get<AppleModel>("Apples")?.GetPrintableObject("Apples"));   //

while (true) {};