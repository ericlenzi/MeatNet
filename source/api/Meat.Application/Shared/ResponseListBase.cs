namespace Meat.Application.Shared
{
    public class ResponseListBase<T>
    {
        public T Data { get; set; }

        public int TotalRows { get; set; }
    }
}
