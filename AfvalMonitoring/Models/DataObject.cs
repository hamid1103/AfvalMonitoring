using System.ComponentModel.DataAnnotations;

namespace AfvalMonitoring.Models;

public class DataObject
{
    [Key]
    public Guid? Id { get; set; }
    public string Label { get; set; }
    public float? Confidence { get; set; }
    public string? LocatieCoordinaten { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocatieAdres { get; set; }
    public DateTime Tijd { get; set; }
    public int CameraID {get; set;}
    public float? BoundingBoxRB {get; set;}
    public float? BoundingBoxLB {get; set;}
    public float? BoundingBoxCenterX {get; set;}
    public float? BoundingBoxCenterY {get; set;}
}