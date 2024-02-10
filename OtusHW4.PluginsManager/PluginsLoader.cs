using OtusHW4.Core;
using System.Collections.Concurrent;
using System.Reflection;

namespace OtusHW4.PluginsManager
{
    /// <inheritdoc/>
    public class PluginsLoader : IPluginsLoader
    {
        private const uint RetriesCount = 3;
        private const string DllExtension = ".dll";

        private ConcurrentQueue<object> _plugins = new ConcurrentQueue<object>();

        /// <inheritdoc/>
        public void LoadPlugins(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "Путь к папке с плагинами должен быть задан.");
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentException("Папка по указанному пути не существует.", nameof(path));
            }

            Task.Run(() => GetPluginsFromFolder(path));

            Thread.Sleep(50);

            Task.Run(() => LoadPluginsWithRetries(RetriesCount));

        }

        private void LoadPluginsWithRetries(uint retriesCount)
        {
            var pluginsFailedToLoad = new Queue<(object PluginInstance, uint Retries)>();

            while (!_plugins.IsEmpty)
            {
                _plugins.TryDequeue(out var plugin);

                if (plugin is null)
                {
                    continue;
                }

                var loadMethod = LoadMethodFromPlugin(plugin);

                try
                {
                    Console.WriteLine($"Началась загрузка плагина {plugin}.");
                    loadMethod.Invoke(plugin, null);
                    Console.WriteLine($"Загрузка плагина {plugin} завершена.");
                }
                catch
                {
                    Console.WriteLine($"Не удалось загрузить плагин {plugin}. Ставим в очередь на загрузку повторно.");
                    pluginsFailedToLoad.Enqueue(new(plugin, 0));
                }
            }

            var pluginsFailedToLoadCount = 0;

            while (pluginsFailedToLoad.Count != 0 && retriesCount > 0)
            {
                var pluginWithRetries = pluginsFailedToLoad.Dequeue();

                var plugin = pluginWithRetries.PluginInstance;
                var retries = pluginWithRetries.Retries;
                var loadMethod = LoadMethodFromPlugin(plugin);

                try
                {
                    Console.WriteLine($"Началась повторная загрузка плагина {plugin} №{retries+1}.");
                    loadMethod.Invoke(plugin, null);
                    Console.WriteLine($"Повторная загрузка плагина {plugin} завершена.");
                }
                catch
                {
                    Console.WriteLine($"Повторная загрузка плагина {plugin} не удалась.");
                    retries++;

                    if (retries < retriesCount)
                    {
                        Console.WriteLine($"Плагин {plugin} поставлен в очередь на повторную загрузку.");
                        pluginsFailedToLoad.Enqueue(new(plugin, retries));
                    }
                    else
                    {
                        pluginsFailedToLoadCount++;
                    }
                }
            }

            if (pluginsFailedToLoadCount != 0)
            {
                throw new InvalidOperationException("Не удалось активировать некоторые из плагинов.");
            }
        }

        private static MethodInfo LoadMethodFromPlugin(object plugin)
        {
            var loadMethod = plugin.GetType().GetMethod(nameof(IPlugin.Load));

            if (loadMethod is null)
            {
                throw new ArgumentException($"Плагин не содержит вызываемого метода {nameof(IPlugin.Load)}.");
            }

            return loadMethod;
        }

        private void GetPluginsFromFolder(string path)
        {
            var dllPaths = Directory.GetFiles(path)
                            .Where(file => Path.GetExtension(file) == DllExtension)
                            .ToArray();

            foreach (var dllPath in dllPaths)
            {
                var pluginType = Assembly.LoadFrom(dllPath)
                    .GetExportedTypes()
                    .Where(type => type.IsAssignableTo(typeof(IPlugin)))
                    .FirstOrDefault();

                if (pluginType is null)
                {
                    throw new ArgumentException($"Сборка {dllPath} не содержит класса, наследуемого от {nameof(IPlugin)}.");
                }

                var pluginInstance = Activator.CreateInstance(pluginType);

                if (pluginInstance is null)
                {
                    throw new ArgumentException($"Не удалось создать объект класса, наследуемого от {nameof(IPlugin)}, в сборка {dllPath}.");
                }

                _plugins.Enqueue(pluginInstance);
                Console.WriteLine($"Плагин {dllPath} поставлен в очередь на запуск.");
            }
        }
    }
}
