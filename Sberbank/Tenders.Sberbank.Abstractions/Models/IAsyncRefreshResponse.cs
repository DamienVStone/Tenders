namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IAsyncRefreshResponse<T>
        where T : IAsyncRefreshResponseData
    {
        T d { get; set; }

        /// <summary>
        /// Преобразование десериализованного результата к абстракции
        /// </summary>
        /// <returns></returns>
        IAsyncRefreshResponse<IAsyncRefreshResponseData> ToAbstraction();
    }
}