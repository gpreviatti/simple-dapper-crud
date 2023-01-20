using System.Data;
using Core.Interfaces;
using Dapper;

namespace Infra.Data.Repository;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly IDbConnection _connection;

    public BaseRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task Create(T entity)
    {
        var sql = $"INSERT INTO {typeof(T).Name} ({string.Join(",", typeof(T).GetProperties().Select(p => p.Name))}) VALUES (@{string.Join(",@", typeof(T).GetProperties().Select(p => p.Name))})";

        await _connection.ExecuteAsync(sql, entity);
    }

    public async Task<T> Read(string id)
    {
        var sql = $"SELECT * FROM {typeof(T).Name} WHERE Id = @Id";
        return await _connection.QuerySingleAsync<T>(sql, new { Id = id });
    }

    public async Task Update(T entity)
    {
        var sql = $"UPDATE {typeof(T).Name} SET {string.Join(",", typeof(T).GetProperties().Where(p => p.Name != "Id").Select(p => $"{p.Name} = @{p.Name}"))} WHERE Id = @Id";
        await _connection.ExecuteAsync(sql, entity);
    }

    public async Task Delete(string id)
    {
        var sql = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
