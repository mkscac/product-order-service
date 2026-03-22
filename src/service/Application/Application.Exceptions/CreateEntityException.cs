namespace Application.Exceptions;

public class CreateEntityException : Exception
{
    public string EntityName { get; }

    public CreateEntityException(string entityName)
        : base($"Ошибка. Создание на совершилось в {entityName}")
    {
        EntityName = entityName;
    }
}