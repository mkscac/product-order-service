namespace Application.Exceptions;

public class NotCorrectOrderStateException : Exception
{
    public string Action { get; }

    public NotCorrectOrderStateException(string action)
        : base($"Ошибка. {action} товара не произошло, статус заказа != Created")
    {
        Action = action;
    }
}