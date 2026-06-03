using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AfvalMonitoring.Models;

public class Analyses
{
    [Key]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    [NotNull]
    public int TotaleAfval { get; set; }
    public int Biodegradable { get; set; }
    public int Paper { get; set; }
    public int Metal { get; set; }
    public int Plastic { get; set; }
    public int Glass { get; set; }
    public int Cloth { get; set; }
}