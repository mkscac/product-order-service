using Application.Abstractions.Persistence;
using Application.Abstractions.Persistence.Queries;
using Application.Abstractions.Persistence.Repositories;
using Application.Exceptions;
using Application.Models.Orders;
using Application.Models.Orders.OrderHistoryPayloads;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Persistence.Repositories;

public class NpgsqlOrderHistoryRepository : IOrderHistoryRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlOrderHistoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> AddNoteAsync(
        long orderId,
        OrderHistoryItemKind itemKind,
        OrderHistoryPayloadBase payload,
        CancellationToken ct)
    {
        const string sql = """
                           insert into order_history (order_id, 
                                                      order_history_item_created_at, 
                                                      order_history_item_kind, 
                                                      order_history_item_payload) 
                           values (:order_id, :created_at, :item_kind, :item_payload::jsonb)
                           returning order_history_item_id;
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("order_id", orderId);
        command.Parameters.AddWithValue("created_at", DateTimeOffset.UtcNow);
        command.Parameters.Add(new NpgsqlParameter<OrderHistoryItemKind>("item_kind", itemKind));
        command.Parameters.Add(new NpgsqlParameter("item_payload", NpgsqlDbType.Jsonb)
        {
            Value = OrderHistoryConvertJson.Serialize(payload),
        });

        object? id = await command.ExecuteScalarAsync(ct);
        if (id == null)
            throw new CreateEntityException(nameof(OrderHistory));
        return (long)id;
    }

    public async Task<Page<OrderHistory>> SearchAsync(
        OrderHistoryQuery query,
        PaginationParams parameters,
        CancellationToken ct)
    {
        const string sql = """
                           select order_history_item_id, 
                                  order_id, 
                                  order_history_item_created_at, 
                                  order_history_item_kind, 
                                  order_history_item_payload
                           from order_history oh
                           where
                               (cardinality(:order_ids) = 0 or order_id = any (:order_ids))
                               and (:item_kind is null or order_history_item_kind = :item_kind)
                           order by order_history_item_created_at 
                           limit @lim offset @offs;
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("order_ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
        {
            Value = query.OrderIds ?? [],
        });
        command.Parameters.Add(new NpgsqlParameter<OrderHistoryItemKind?>("item_kind", query.ItemKind));
        command.Parameters.AddWithValue("lim", parameters.PageSize);
        command.Parameters.AddWithValue("offs", (parameters.PageNumber - 1) * parameters.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        var items = new List<OrderHistory>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new OrderHistory(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetFieldValue<DateTimeOffset>(2),
                reader.GetFieldValue<OrderHistoryItemKind>(3),
                OrderHistoryConvertJson.Deserialize(reader.GetString(4))));
        }

        return new Page<OrderHistory>(items);
    }
}