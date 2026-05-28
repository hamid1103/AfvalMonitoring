using AfvalMonitoring.Data;
using AfvalMonitoring.Models;
using Microsoft.EntityFrameworkCore;

namespace AfvalMonitoring.Repositories.DataController;

public class DataDbContextRepository : IDataRepository
{
    private readonly DataDbContext _db;

    public DataDbContextRepository(DataDbContext _db)
    {
        this._db = _db;
    }

    public async Task<List<DataObject>> GetDataObjects()
    {
        List<DataObject> objects = await _db.DataObjects.Take(5).ToListAsync();
        return objects;
    }

    public Task<DataObject> GetDataObject(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task SaveDataObject(DataObject obj)
    {
        throw new NotImplementedException();
    }

    public Task UpdateDataObject(DataObject obj)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDataObject(Guid id)
    {
        throw new NotImplementedException();
    }
}