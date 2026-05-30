using System.Net;
using AfvalMonitoring.Models;
using AfvalMonitoring.Repositories.DataController;
using Microsoft.AspNetCore.Mvc;

namespace AfvalMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IDataRepository _repository;
    
    public DataController(IDataRepository repository)
    {
        _repository = repository;
    }

    [HttpPost(Name = "InsertData")]
    public async Task<ActionResult> UploadObject(DataObject dataObject)
    {
        await _repository.SaveDataObject(dataObject);
        return Ok();
    }

    [HttpPost("bulk", Name = "BulkInsert")]
    public async Task<ActionResult> BulkUpload(BulkUpload bulkUpload)
    {
        await _repository.SaveDataObjects(bulkUpload.dataObjects);
        return Ok();
    }

    [HttpGet("{id}", Name = "GetDataObjectById")]
    public async Task<ActionResult<DataObject>> GetDataObject(Guid id)
    {
        var dataObject = await _repository.GetDataObject(id);
        return Ok(dataObject);
    }

    [HttpGet]
    public async Task<List<DataObject>> GetDataObjects()
    {
        List<DataObject> dataObjects = await _repository.GetDataObjects();
        return dataObjects;
    }
    
}

public class BulkUpload()
{
    public List<DataObject> dataObjects { get; set; }
}

public class SearchSettings()
{
    public int Page = 1;
    public int PageSize = 10;
}