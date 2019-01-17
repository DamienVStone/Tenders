namespace Tenders.Core.Helpers
{
    /// <summary>
    /// Дополнительные операции со строками
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Преобразует строку в формат для поиска
        /// </summary>
        /// <param name="source">Строка для преобразования</param>
        /// <returns>Преобразованная строка</returns>
        public static string ToSearchString(this string source)
        {
            var result = source?.Trim()?.ToUpperInvariant();
            return result;
        }
    }
}
