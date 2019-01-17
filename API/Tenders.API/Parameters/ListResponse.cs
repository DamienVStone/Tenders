namespace Tenders.API.Parameters
{
    public class ListResponse<T>
    {
        public long Count { get; set; }
        public T[] Data { get; set; }
    }
}
