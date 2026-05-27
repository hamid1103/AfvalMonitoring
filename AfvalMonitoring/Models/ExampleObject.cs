using System.ComponentModel.DataAnnotations;

namespace AfvalMonitoring.Models;

public class ExampleObject
{
    [Key]
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public int Number { get; set; }
}