using Application.Abstractions.Persistence;
using Application.Abstractions.Persistence.Queries;
using Application.Abstractions.Persistence.Repositories;
using Application.Exceptions;
using Application.Models.Orders;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Persistence.Repositories;

public class NpgsqlOrderRepository : IOrderRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlOrderRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateAsync(string createdBy, CancellationToken ct)
    {
        const string sql = """
                           insert into orders (order_state, order_created_at, order_created_by) 
                           values (@state, @created_at, @created_by)
                           returning order_id;
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("state", OrderState.Created);
        command.Parameters.AddWithValue("created_at", DateTimeOffset.UtcNow);
        command.Parameters.AddWithValue("created_by", createdBy);

        object? id = await command.ExecuteScalarAsync(ct);
        if (id == null)
            throw new CreateEntityException(nameof(Order));
        return (long)id;
    }

    public async Task ChangeStateAsync(long id, OrderState toState, CancellationToken ct)
    {
        const string sql = """
                           update orders
                           set order_state = :to_state
                           where order_id = :id
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("to_state", toState);
        command.Parameters.AddWithValue("id", id);

        if (await command.ExecuteNonQueryAsync(ct) == 0)
            throw new NotFoundException(nameof(Order), id);
    }

    public async Task<Page<Order>> SearchAsync(
        OrderQuery query,
        PaginationParams parameters,
        CancellationToken ct)
    {
        const string sql = """
                           select order_id, order_state, order_created_at, order_created_by
                           from orders
                           where
                               (cardinality(:ids) = 0 or order_id = any (:ids))
                               and (:state is null or order_state = :state)
                               and (:author is null or order_created_by ilike :author)
                           order by order_id
                           limit :lim offset :offs
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
        {
            Value = query.Ids ?? [],
        });
        command.Parameters.Add(new NpgsqlParameter<OrderState?>("state", query.State));
        command.Parameters.Add(new NpgsqlParameter("author", NpgsqlDbType.Text)
        {
            Value = string.IsNullOrWhiteSpace(query.CreatedBy) ? DBNull.Value : query.CreatedBy,
        });
        command.Parameters.AddWithValue("lim", parameters.PageSize);
        command.Parameters.AddWithValue("offs", (parameters.PageNumber - 1) * parameters.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        var items = new List<Order>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new Order(
                reader.GetInt64(0),
                reader.GetFieldValue<OrderState>(1),
                reader.GetFieldValue<DateTimeOffset>(2),
                reader.GetString(3)));
        }

        return new Page<Order>(items);
    }
}