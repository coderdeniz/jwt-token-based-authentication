using System.Text.Json.Serialization;

namespace UdemyShared.Dtos
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        
        [JsonIgnore]
        public bool IsSuccessful { get; private set; }
        
        public ErrorDto Error { get; private set; }

        public static Response<T> Success(T data, int statusCode) 
        { 
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessful = true };
        }
        
        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            { 
                Data = default,
                StatusCode = statusCode,
                Error = errorDto,
                IsSuccessful = false
            };
        }

        public static Response<T> Fail(string error, int statusCode, bool isShow)
        {
            return new Response<T>
            {
                Data = default,
                StatusCode = statusCode,
                Error = new ErrorDto(error, isShow),
                IsSuccessful = false
            };
        }
    }
}
