namespace CarTransportDashboard.Models
{
    public class TransportJob
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public JobStatus Status { get; set; }

    // Foreign Keys
    public int? AssignedVehicleId { get; set; }
    public Vehicle? AssignedVehicle { get; set; }

    public string? AssignedDriverId { get; set; }
    public ApplicationUser? AssignedDriver { get; set; }
}

}