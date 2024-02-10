// See https://aka.ms/new-console-template for more information
using OtusHW4.PluginsManager;

Console.WriteLine("Запускаем загрузку плагинов");

var pathToFolder = "../../../../Plugins";

var pluginLoader = new PluginsLoader();

pluginLoader.LoadPlugins(pathToFolder);

Console.ReadKey();