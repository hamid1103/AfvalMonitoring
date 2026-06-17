using AfvalMonitoring.Models;

namespace AfvalMonitoring.Utils;

public class GoogleUtils : IGoogleUtils
{
    private GoogleClient _googleClient;

    public GoogleUtils(string apiKey)
    {
        
    }
    public async Task<DataObject> AddLocationAddress(DataObject dataObject)
    {
        if (_googleClient == null)
            return dataObject;
        return dataObject;
    }
}