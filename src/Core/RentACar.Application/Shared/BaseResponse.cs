namespace RentACar.Application.Shared;
using System.Net;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public T? Data { get; set; }


    public BaseResponse(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        Success = (int)statusCode >= 200 && (int)statusCode < 300;
    }

 
    public BaseResponse(string message, HttpStatusCode statusCode)
    {
        Message = message;
        StatusCode = statusCode;
        Success = (int)statusCode >= 200 && (int)statusCode < 300;
    }

 
    public BaseResponse(string message, bool isSuccess, HttpStatusCode statusCode)
    {
        Message = message;
        Success = isSuccess;
        StatusCode = statusCode;
    }

  
    public BaseResponse(string message, T? data, HttpStatusCode statusCode)
    {
        Message = message;
        Data = data;
        StatusCode = statusCode;
        Success = (int)statusCode >= 200 && (int)statusCode < 300;
    }


    public static BaseResponse<T> SuccessResponse(T data, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new BaseResponse<T>(message ?? "Success", data, statusCode);
    }

    public static BaseResponse<T> FailResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new BaseResponse<T>(message, false, statusCode);
    }
}

