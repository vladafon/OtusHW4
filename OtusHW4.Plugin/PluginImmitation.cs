using OtusHW4.Core;

namespace OtusHW4.Plugin
{
    public class PluginImmitation : IPlugin
    {
        public void Load()
        {
            var random = new Random();

            var loadTimeMs = random.Next(300, 5000);

            var failProbability = random.Next(0, 100);

            var isFailLoad = failProbability > 50;

            if (isFailLoad)
            {
                Thread.Sleep(100);

                throw new Exception("Плагин не загрузился.");
            }

            Thread.Sleep(loadTimeMs);
        }
    }
}
