using AfvalMonitoring.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AfvalMonitoring.Repositories;

public class SQLExampleRepo : IExampleRepo
{
    private readonly string sqlConnectionString;

    public SQLExampleRepo(string sqlConnectionString)
    {
        this.sqlConnectionString = sqlConnectionString;
    }

    public async Task InsertAsync(ExampleObject exampleObject)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync(
                "INSERT INTO [ExampleObject] (Id, Name, Number) VALUES (@Id, @Name, @Number)", exampleObject);
        }
    }

    public async Task<ExampleObject?> SelectAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QuerySingleOrDefaultAsync<ExampleObject>(
                "SELECT * FROM [ExampleObject] WHERE Id = @Id", new { id });
        }
    }

    public async Task<IEnumerable<ExampleObject>> SelectAsync()
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<ExampleObject>("SELECT * FROM [ExampleObject]");
        }
    }

    public async Task UpdateAsync(ExampleObject exampleObject)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("UPDATE [ExampleObject] SET " +
                                             "Name = @Name, " +
                                             "Number = @Number " +
                                             "WHERE Id = @Id", exampleObject);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("DELETE FROM [ExampleObject] WHERE Id = @Id", new { id });
        }
    }
}