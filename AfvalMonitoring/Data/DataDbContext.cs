using AfvalMonitoring.Models;
using Microsoft.EntityFrameworkCore;

namespace AfvalMonitoring.Data;

public class DataDbContext(DbContextOptions<DataDbContext> options) : DbContext(options)
{
    public DbSet<DataObject> DataObjects { get; set; }
}