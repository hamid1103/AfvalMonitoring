using System.ComponentModel.DataAnnotations;

namespace AfvalMonitoring.Models;

public class DataObject
{
    [Key]
    public Guid Id { get; set; }
    
    
}