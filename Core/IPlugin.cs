namespace OtusHW4.Core
{
    /// <summary>
    ///     Интерфейс определяет плагин.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        ///     Метод загрузки плагина в сборку.
        /// </summary>
        void Load();
    }
}
