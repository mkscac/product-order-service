namespace Application.Exceptions;

public class NotFoundException : Exception
{
    public string EntityName { get; }

    public long Id { get; }

    public NotFoundException(string entityName, long id)
        : base($"Ошибка. Сущность {entityName} с id = {id} не найдена")
    {
        EntityName = entityName;
        Id = id;
    }
}