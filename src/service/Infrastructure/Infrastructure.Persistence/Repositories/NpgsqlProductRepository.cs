using Application.Abstractions.Persistence;
using Application.Abstractions.Persistence.Queries;
using Application.Abstractions.Persistence.Repositories;
using Application.Exceptions;
using Application.Models.Products;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Persistence.Repositories;

public class NpgsqlProductRepository : IProductRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlProductRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateAsync(string name, decimal price, CancellationToken ct)
    {
        const string sql = """
                           insert into products (product_name, product_price) 
                           values (:name, :price)
                           returning product_id;
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("price", price);

        object? id = await command.ExecuteScalarAsync(ct);
        if (id == null)
            throw new CreateEntityException(nameof(Product));
        return (long)id;
    }

    public async Task<Page<Product>> SearchAsync(
        ProductQuery query,
        PaginationParams parameters,
        CancellationToken ct)
    {
        const string sql = """
                           select product_id, product_name, product_price
                           from products
                           where
                               (cardinality(:ids) = 0 or product_id = any (:ids))
                               and (:min is null or product_price > :min)
                               and (:max is null or product_price < :max)
                               and (:name is null or product_name ilike :name)
                           order by product_id
                           limit :lim offset :offs
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
        {
            Value = query.Ids ?? [],
        });
        command.Parameters.Add(new NpgsqlParameter("min", NpgsqlDbType.Money)
        {
            Value = query.MinPrice.HasValue ? query.MinPrice.Value : DBNull.Value,
        });
        command.Parameters.Add(new NpgsqlParameter("max", NpgsqlDbType.Money)
        {
            Value = query.MaxPrice.HasValue ? query.MaxPrice.Value : DBNull.Value,
        });
        command.Parameters.Add(new NpgsqlParameter("name", NpgsqlDbType.Text)
        {
            Value = string.IsNullOrWhiteSpace(query.NameSubstring) ? DBNull.Value : query.NameSubstring,
        });
        command.Parameters.AddWithValue("lim", parameters.PageSize);
        command.Parameters.AddWithValue("offs", (parameters.PageNumber - 1) * parameters.PageSize);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        var items = new List<Product>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new Product(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetDecimal(2)));
        }

        return new Page<Product>(items);
    }
}