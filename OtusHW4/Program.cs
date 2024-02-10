// See https://aka.ms/new-console-template for more information
using OtusHW4.PluginsManager;

Console.WriteLine("Запускаем загрузку плагинов");

var pathToFolder = "C:\\Users\\Администратор.WIN-A4RBFQ3J62N\\source\\repos\\OTUS\\OtusHW4\\Plugins";

var pluginLoader = new PluginsLoader();

pluginLoader.LoadPlugins(pathToFolder);

Console.ReadKey();