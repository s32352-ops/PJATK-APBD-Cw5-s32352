using Microsoft.AspNetCore.Mvc;
using WebApplication3.DTO;
using WebApplication3.Services;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await patientService.GetAllAsync(search, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBed(string pesel, [FromBody] BedAssignmentRequest request, CancellationToken cancellationToken)
    {
        var (patientFound, wardFound, bedTypeFound, assignmentId) = await patientService.AssignBedAsync(pesel, request, cancellationToken);
        if (!patientFound)
            return NotFound($"Patient with '{pesel}' not found.");

        if (!wardFound)
            return NotFound($"Ward '{request.Ward}' was not found.");

        if (!bedTypeFound)
            return NotFound($"Bed type '{request.BedType}' was not found.");

        if (assignmentId is null)
            return NotFound($"No bed of type '{request.BedType}' in ward '{request.Ward}' ");
                

        return Created();
    }
}
