using IncidentAPI_Louay_Ben_Ali.Models;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAPI_Louay_Ben_Ali.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentsController : ControllerBase
{
    private static readonly List<Incident> _incidents = new();
    private static int _nextId = 1;
    private static readonly string[] AllowedSeverities = { "LOW", "MEDIUM", "HIGH", "CRITICAL" };
    private static readonly string[] AllowedStatuses = { "OPEN", "IN_PROGRESS", "RESOLVED" };

    // POST: api/incidents/create-incident
    [HttpPost("create-incident")]
    public IActionResult CreateIncident([FromBody] Incident incident)
    {
        // Validate Title
        if (string.IsNullOrWhiteSpace(incident.Title) || incident.Title.Length > 30)
        {
            return BadRequest("Title is required and must not exceed 30 characters.");
        }

        // Validate Description
        if (string.IsNullOrWhiteSpace(incident.Description) || incident.Description.Length > 200)
        {
            return BadRequest("Description is required and must not exceed 200 characters.");
        }

        // Validate Severity
        if (string.IsNullOrWhiteSpace(incident.Severity) || !AllowedSeverities.Contains(incident.Severity.ToUpper()))
        {
            return BadRequest($"Severity must be one of: {string.Join(", ", AllowedSeverities)}");
        }

        // Set defaults - Status initialized to OPEN, CreatedAt set to current UTC time
        incident.Id = _nextId++;
        incident.Status = "OPEN";
        incident.CreatedAt = DateTime.UtcNow;
        incident.Severity = incident.Severity.ToUpper();

        _incidents.Add(incident);
        return Ok(incident);
    }

    // GET: api/incidents/get-all
    [HttpGet("get-all")]
    public IActionResult GetAllIncidents()
    {
        return Ok(_incidents);
    }

    // GET: api/incidents/getbyid/{id}
    [HttpGet("getbyid/{id}")]
    public IActionResult GetIncidentById(int id)
    {
        var incident = _incidents.FirstOrDefault(i => i.Id == id);

        if (incident == null)
            return NotFound($"Incident with ID {id} not found.");

        return Ok(incident);
    }

    // PUT: api/incidents/update-status/{id}
    [HttpPut("update-status/{id}")]
    public IActionResult UpdateIncidentStatus(int id, [FromBody] string status)
    {
        // Validate Status
        if (string.IsNullOrWhiteSpace(status) || !AllowedStatuses.Contains(status.ToUpper()))
        {
            return BadRequest($"Status must be one of: {string.Join(", ", AllowedStatuses)}");
        }

        var incident = _incidents.FirstOrDefault(i => i.Id == id);

        if (incident == null)
            return NotFound($"Incident with ID {id} not found.");

        incident.Status = status.ToUpper();
        return Ok(incident);
    }

    // DELETE: api/incidents/delete-incident/{id}
    [HttpDelete("delete-incident/{id}")]
    public IActionResult DeleteIncident(int id)
    {
        var incident = _incidents.FirstOrDefault(i => i.Id == id);

        if (incident == null)
            return NotFound($"Incident with ID {id} not found.");

        // Rule: CRITICAL incidents cannot be deleted if OPEN
        if (incident.Severity == "CRITICAL" && incident.Status == "OPEN")
        {
            return BadRequest("Cannot delete a CRITICAL incident that is still OPEN.");
        }

        _incidents.Remove(incident);
        return NoContent();
    }

    // GET: api/incidents/filter-by-status/{status}
    [HttpGet("filter-by-status/{status}")]
    public IActionResult FilterByStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return BadRequest("Status filter cannot be empty.");
        }

        var filtered = _incidents
            .Where(i => i.Status.Contains(status.ToUpper(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(filtered);
    }

    // GET: api/incidents/filter-by-severity/{severity}
    [HttpGet("filter-by-severity/{severity}")]
    public IActionResult FilterBySeverity(string severity)
    {
        if (string.IsNullOrWhiteSpace(severity))
        {
            return BadRequest("Severity filter cannot be empty.");
        }

        var filtered = _incidents
            .Where(i => i.Severity.Contains(severity.ToUpper(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(filtered);
    }

    // GET: api/incidents/filter-status-async/{status}
    [HttpGet("filter-status-async/{status}")]
    public async Task<IActionResult> FilterByStatusAsync(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return BadRequest("Status filter cannot be empty.");
        }

        var filtered = await Task.FromResult(_incidents
            .Where(i => i.Status.Contains(status.ToUpper(), StringComparison.OrdinalIgnoreCase))
            .ToList());

        return Ok(filtered);
    }

    // GET: api/incidents/filter-severity-async/{severity}
    [HttpGet("filter-severity-async/{severity}")]
    public async Task<IActionResult> FilterBySeverityAsync(string severity)
    {
        if (string.IsNullOrWhiteSpace(severity))
        {
            return BadRequest("Severity filter cannot be empty.");
        }

        var filtered = await Task.FromResult(_incidents
            .Where(i => i.Severity.Contains(severity.ToUpper(), StringComparison.OrdinalIgnoreCase))
            .ToList());

        return Ok(filtered);
    }

    // GET: api/incidents/search-status-async/{status}
    [HttpGet("search-status-async/{status}")]
    public async Task<IActionResult> SearchByStatusAsync(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return BadRequest("Status search term cannot be empty.");
        }

        await Task.Delay(100); // Simulate async operation

        var results = _incidents
            .Where(i => i.Status.Contains(status.ToUpper(), StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.CreatedAt)
            .ToList();

        return Ok(results);
    }

    // GET: api/incidents/search-severity-async/{severity}
    [HttpGet("search-severity-async/{severity}")]
    public async Task<IActionResult> SearchBySeverityAsync(string severity)
    {
        if (string.IsNullOrWhiteSpace(severity))
        {
            return BadRequest("Severity search term cannot be empty.");
        }

        await Task.Delay(100); // Simulate async operation

        var results = _incidents
            .Where(i => i.Severity.Contains(severity.ToUpper(), StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.CreatedAt)
            .ToList();

        return Ok(results);
    }
}
