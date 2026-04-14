namespace IncidentAPI_Louay_Ben_Ali.Models;

public class Incident
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Severity { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public Incident()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
