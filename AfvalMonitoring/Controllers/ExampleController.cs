using AfvalMonitoring.Models;
using AfvalMonitoring.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AfvalMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IExampleRepo _exampleRepo;

    public ExampleController(IExampleRepo exampleRepo)
    {
        _exampleRepo = exampleRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExampleObject>>> GetAll()
    {
        var exampleObjects = await _exampleRepo.SelectAsync();
        return Ok(exampleObjects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExampleObject>> GetById(Guid id)
    {
        var obj = await _exampleRepo.SelectAsync(id);
        return Ok(new { Id = id, Name = "Student " + id });
    }

    [HttpPost]
    public async Task<ActionResult> AddObject(ExampleObject obj)
    {
        obj.Id = Guid.NewGuid();
        await _exampleRepo.InsertAsync(obj);
        return Ok();
    }
}