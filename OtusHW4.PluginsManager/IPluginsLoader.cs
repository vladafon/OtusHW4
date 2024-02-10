namespace OtusHW4.PluginsManager
{
    /// <summary>
    ///     Интерфейс для работы с внешними плагинами.
    /// </summary>
    internal interface IPluginsLoader
    {
        /// <summary>
        ///     Позволяет загружать внешние плагины в систему. 
        /// </summary>
        /// <param name="path">Путь к папке с плагинами.</param>
        void LoadPlugins(string path);
    }
}
