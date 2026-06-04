using AfvalMonitoring.Models;

namespace AfvalMonitoring.Utils;

public interface IGoogleUtils
{
    public Task<DataObject> AddLocationAddress(DataObject dataObject);
}