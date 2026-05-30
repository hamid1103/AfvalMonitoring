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

    public async Task<DataObject?> GetDataObject(Guid id)
    {
        DataObject? dataObject = await _db.DataObjects.FindAsync(id);
        return dataObject;
    }

    public async Task SaveDataObject(DataObject obj)
    {
        if (obj.Id == Guid.Empty)
        {
            obj.Id = Guid.NewGuid();
        }
        await _db.DataObjects.AddAsync(obj);
        await _db.SaveChangesAsync();
    }

    public async Task SaveDataObjects(List<DataObject> objs)
    {
        foreach (var obj in objs)
        {
            if (obj.Id == Guid.Empty)
            {
                obj.Id = Guid.NewGuid();
            }
            await _db.DataObjects.AddAsync(obj);
        }
        await _db.SaveChangesAsync();
    }

    public async Task UpdateDataObject(DataObject obj)
    {
        _db.DataObjects.Update(obj);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteDataObject(Guid id)
    {
        _db.DataObjects.Remove(await _db.DataObjects.FindAsync(id));
        
        await _db.SaveChangesAsync();
    }
}