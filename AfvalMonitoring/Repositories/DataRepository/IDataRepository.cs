using AfvalMonitoring.Models;

namespace AfvalMonitoring.Repositories.DataController;

public interface IDataRepository
{
    public Task<List<DataObject>> GetDataObjects();
    public Task<DataObject?> GetDataObject(Guid id);
    public Task SaveDataObject(DataObject obj);
    public Task SaveDataObjects(List<DataObject> objs);
    public Task UpdateDataObject(DataObject obj);
    public Task DeleteDataObject(Guid id);
}