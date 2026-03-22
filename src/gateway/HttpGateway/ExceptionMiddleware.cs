#pragma warning disable IDE0072
using Grpc.Core;

namespace HttpGateway;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException e)
        {
            string message =
                $"При обработке запроса возникло исключение, тип = {e.GetType().Name}, сообщение = {e.Message}";

            context.Response.StatusCode = MapHttpStatus(e.StatusCode);
            await context.Response.WriteAsJsonAsync(new { Message = message });
        }
    }

    private int MapHttpStatus(StatusCode statusCode)
    {
        return statusCode switch
        {
            StatusCode.Internal => StatusCodes.Status500InternalServerError,
            StatusCode.FailedPrecondition => StatusCodes.Status412PreconditionFailed,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}