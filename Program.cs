using observe;
using observe.ConfigurationBuilder;
using observe.ConfigurationsModels;

var configuration = new ConfigurationBuilder()
    .AddJsonFile<ConfigModel>(Path.GetFullPath("config.json"))
    .AddJsonFile<AppleModel>(Path.GetFullPath("apple.json"), "Apples")
    .SetDefaultConfiguration("config")
    .Build();

configuration.ConfigurationFileUpdate += (args) => {
    Console.WriteLine($"Field is updated:");
    Console.WriteLine($"\tPath: {args.file.FullName}");
    Console.WriteLine($"\tPath: {(args.Configuration as Consolable).GetPrintableObject()}");
    Console.WriteLine($"\tPath: {args.FieldsUpdate.Join(", ")}");
};

Console.WriteLine("Keys: " + configuration.GetConfigurationList().Join(", "));
Console.WriteLine(configuration.GetDefault<ConfigModel>()?.GetPrintableObject("Config"));
Console.WriteLine(configuration.Get<AppleModel>("Apples")?.GetPrintableObject("Apples"));

while (true) { };