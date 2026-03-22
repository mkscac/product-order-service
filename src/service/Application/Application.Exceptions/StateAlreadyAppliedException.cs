using Application.Models.Orders;

namespace Application.Exceptions;

public class StateAlreadyAppliedException : Exception
{
    public long Id { get; }

    public OrderState OrderState { get; }

    public StateAlreadyAppliedException(long id, OrderState orderState)
        : base($"Ошибка. Заказу id = {id} уже присвоено это состояние {orderState}. Повторно назначать нельзя")
    {
        Id = id;
        OrderState = orderState;
    }
}