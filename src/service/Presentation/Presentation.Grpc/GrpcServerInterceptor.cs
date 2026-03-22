using Application.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Presentation.Grpc;

public class GrpcServerInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (CreateEntityException e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
        catch (NotCorrectOrderStateException e)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
        }
        catch (NotFoundException e)
        {
            throw new RpcException(new Status(StatusCode.NotFound, e.Message));
        }
        catch (StateAlreadyAppliedException e)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
        }
        catch (Exception e)
        {
            throw new RpcException(new Status(
                StatusCode.Internal,
                detail: $"Возникла непредвиденная ошибка: {e.Message}"));
        }
    }
}