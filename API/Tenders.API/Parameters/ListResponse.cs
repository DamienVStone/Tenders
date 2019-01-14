namespace TenderPlanAPI.Parameters
{
    public class ListResponse<T>
    {
        public long Count { get; set; }
        public T[] Data { get; set; }
    }
}
