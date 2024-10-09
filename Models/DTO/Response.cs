namespace project1.Models.DTO
{    
    public class Response<T>
    {
        public required int Code { get; set; }
        public required string Message { get; set; }
        public T? Data { get; set; }
    }
}