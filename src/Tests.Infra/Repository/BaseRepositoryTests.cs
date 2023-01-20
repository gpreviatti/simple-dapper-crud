using Core.Entity;
using Core.Interfaces;
using Dapper;
using Infra.Data.Repository;
using Microsoft.Data.Sqlite;

namespace Tests.Infra.Repository;

public class BaseRepositoryTests : IDisposable
{
    private readonly string _connectionString = "Data Source=TestDB.sqlite";
    private readonly IBaseRepository<Person> _repository;
    private Person entity = new(Guid.NewGuid().ToString(), "Test");

    public BaseRepositoryTests()
    {
        using var connection = new SqliteConnection(_connectionString);

        _repository = new BaseRepository<Person>(connection);

        connection.Execute($@"CREATE TABLE IF NOT EXISTS {nameof(Person)} (
            Id VARCHAR(100) NOT NULL,
            Name VARCHAR(100) NOT NULL,
            Email VARCHAR(100) NULL
        );");
    }

    [Fact(DisplayName = nameof(Create_ShouldInsertRecordIntoDatabase))]
    public async Task Create_ShouldInsertRecordIntoDatabase()
    {
        // Arrange

        // Act
        await _repository.Create(entity);

        using var connection = new SqliteConnection(_connectionString);
        var result = await connection.QueryFirstAsync<Person>($"SELECT * FROM {nameof(Person)} WHERE Id = @Id", new { entity.Id });

        // Assert
        Assert.Equal(entity.Name, result.Name);
    }

    [Fact(DisplayName = nameof(Read_ShouldReturnRecordFromDatabase))]
    public async Task Read_ShouldReturnRecordFromDatabase()
    {
        // Arrange
        await _repository.Create(entity);

        // Act
        var result = await _repository.Read(entity.Id);

        // Assert
        Assert.Equal(entity.Name, result.Name);
    }

    [Fact(DisplayName = nameof(Update_ShouldUpdateRecordInDatabase))]
    public async Task Update_ShouldUpdateRecordInDatabase()
    {
        // Arrange
        await _repository.Create(entity);
        entity.Name = "Test2";

        // Act
        await _repository.Update(entity);

        using var connection = new SqliteConnection(_connectionString);
        var result = await connection.QueryFirstAsync<Person>($"SELECT * FROM {nameof(Person)} WHERE Id = @Id", new { entity.Id });

        // Assert
        Assert.Equal(entity.Name, result.Name);
    }

    [Fact(DisplayName = nameof(Delete_ShouldDeleteRecordFromDatabase))]
    public async Task Delete_ShouldDeleteRecordFromDatabase()
    {
        // Arrange
        await _repository.Create(entity);

        // Act
        await _repository.Delete(entity.Id);

        using var connection = new SqliteConnection(_connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<Person>($"SELECT * FROM {nameof(Person)} WHERE Id = @Id", new { entity.Id });

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        using var connection = new SqliteConnection(_connectionString);

        connection.Execute($"DELETE FROM {nameof(Person)}");
    }
}