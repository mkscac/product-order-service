namespace Application.Models.Orders;

public sealed record Order(
    long Id,
    OrderState OrderState,
    DateTimeOffset CreatedAt,
    string CreatedBy);