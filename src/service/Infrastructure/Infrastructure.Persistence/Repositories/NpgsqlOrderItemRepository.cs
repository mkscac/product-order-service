using Application.Abstractions.Persistence;
using Application.Abstractions.Persistence.Queries;
using Application.Abstractions.Persistence.Repositories;
using Application.Exceptions;
using Application.Models.Orders;
using Application.Models.Products;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Persistence.Repositories;

public class NpgsqlOrderItemRepository : IOrderItemRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlOrderItemRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> AddAsync(long orderId, long productId, int quantity, CancellationToken ct)
    {
        const string sql = """
                           insert into order_items (order_id, product_id, order_item_quantity, order_item_deleted) 
                           values (:order_id, :product_id, :quantity, :deleted)
                           returning order_item_id;
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("order_id", orderId);
        command.Parameters.AddWithValue("product_id", productId);
        command.Parameters.AddWithValue("quantity", quantity);
        command.Parameters.AddWithValue("deleted", false);

        object? id = await command.ExecuteScalarAsync(ct);
        if (id == null)
            throw new CreateEntityException(nameof(OrderItem));
        return (long)id;
    }

    public async Task DeleteAsync(long orderId, long productId, CancellationToken ct)
    {
        const string sql = """
                           update order_items
                           set order_item_deleted = true
                           where 
                               (order_id = :order_id)
                               and (product_id = :product_id) 
                               and (order_item_deleted = false)
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("order_id", orderId);
        command.Parameters.AddWithValue("product_id", productId);

        if (await command.ExecuteNonQueryAsync(ct) == 0)
            throw new NotFoundException(nameof(Product), productId);
    }

    public async Task<Page<OrderItem>> SearchAsync(
        OrderItemQuery query,
        PaginationParams parameters,
        CancellationToken ct)
    {
        const string sql = """
                           select order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
                           from order_items
                           where
                               (cardinality(:order_ids) = 0 or order_id = any (:order_ids))
                               and (cardinality(:product_ids) = 0 or product_id = any (:product_ids))
                               and (:deleted is null or order_item_deleted = :deleted)
                           order by order_item_id
                           limit :lim offset :offs
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("order_ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
        {
            Value = query.OrderIds ?? [],
        });
        command.Parameters.Add(new NpgsqlParameter("product_ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
        {
            Value = query.ProductIds ?? [],
        });
        command.Parameters.Add(new NpgsqlParameter("deleted", NpgsqlDbType.Boolean)
        {
            Value = query.IsDeleted.HasValue ? query.IsDeleted : DBNull.Value,
        });
        command.Parameters.AddWithValue("lim", parameters.PageSize);
        command.Parameters.AddWithValue("offs", (parameters.PageNumber - 1) * parameters.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        var items = new List<OrderItem>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new OrderItem(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt64(2),
                reader.GetInt32(3),
                reader.GetBoolean(4)));
        }

        return new Page<OrderItem>(items);
    }
}