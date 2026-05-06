using AfvalMonitoring.Models;

namespace AfvalMonitoring.Repositories;

public interface IExampleRepo
{
    Task InsertAsync(ExampleObject exampleObject);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ExampleObject>> SelectAsync();
    Task<ExampleObject?> SelectAsync(Guid id);
    Task UpdateAsync(ExampleObject exampleObject);
}