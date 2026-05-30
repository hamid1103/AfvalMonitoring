using System.ComponentModel.DataAnnotations;
using AfvalMonitoring.Models;
using Microsoft.EntityFrameworkCore;

namespace AfvalMonitoring.Data;

public class ExampleDBContext(DbContextOptions<ExampleDBContext> options) : DbContext(options)
{
    public DbSet<ExampleObject> ExampleObjects { get; set; }
}