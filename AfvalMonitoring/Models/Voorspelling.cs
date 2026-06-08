using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AfvalMonitoring.Models;

public class Voorspelling
{
    [Key]
    public int Id { get; set; }
    [NotNull]
    public DateTime StartDate { get; set; }
    [NotNull]
    public DateTime EndDate { get; set; }
    public string StreetName { get; set; }
    public int? UserId { get; set; }
    public int Biodegradable { get; set; }
    public int Paper { get; set; }
    public int Metal { get; set; }
    public int Plastic { get; set; }
    public int Glass { get; set; }
    public int Cloth { get; set; }
    public int All { get; set; }
}