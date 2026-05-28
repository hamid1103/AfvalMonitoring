using AfvalMonitoring.Data;
using AfvalMonitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AfvalMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly ExampleDBContext _exampleDbContext;

    public ExampleController(ExampleDBContext _exampleDbContext)
    {
        this._exampleDbContext = _exampleDbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExampleObject>>> GetAll()
    {
        //var exampleObjects = await _exampleRepo.SelectAsync();
        var exampleObjects = await _exampleDbContext.ExampleObjects.OrderByDescending(o=>o.Id).Take(10).ToListAsync();
        return Ok(exampleObjects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExampleObject>> GetById(Guid id)
    {
        //var obj = await _exampleRepo.SelectAsync(id);
        var obj = await _exampleDbContext.ExampleObjects.FirstOrDefaultAsync(m =>m.Id == id);
        return Ok(new { Id = id, Name = $"Example {obj.Id} = {obj.Name}:{obj.Number}" });
    }

    [HttpPost]
    public async Task<ActionResult> AddObject(ExampleObject obj)
    {
        obj.Id = Guid.NewGuid();
        //await _exampleRepo.InsertAsync(obj);
        await _exampleDbContext.ExampleObjects.AddAsync(obj);
        await _exampleDbContext.SaveChangesAsync();
        return Ok();
    }
}