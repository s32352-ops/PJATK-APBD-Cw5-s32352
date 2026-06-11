using WebApplication3.DTO;

namespace WebApplication3.Services;

public interface IPatientService
{
    Task<List<PatientResponse>> GetAllAsync(string? search, CancellationToken cancellationToken);
    Task<(bool patientFound, bool wardFound, bool bedTypeFound, int? assignmentId)> AssignBedAsync(string pesel, BedAssignmentRequest dto, CancellationToken cancellationToken);
}