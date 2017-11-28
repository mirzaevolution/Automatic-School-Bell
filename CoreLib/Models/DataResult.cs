namespace CoreLib.Models
{
    public class DataResult<T>
    {
        public T Data { get; set; }
        public MainResult Status { get; set; }
    }
}

