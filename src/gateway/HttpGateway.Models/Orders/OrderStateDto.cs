namespace HttpGateway.Models.Orders;

public enum OrderStateDto
{
    Unspecified,
    Created,
    Processing,
    Completed,
    Cancelled,
}