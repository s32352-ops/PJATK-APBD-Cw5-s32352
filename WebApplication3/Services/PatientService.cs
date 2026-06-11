using Microsoft.EntityFrameworkCore;
using WebApplication3.DTO;
using WebApplication3.Infrastructure;
using WebApplication3.Models;

namespace WebApplication3.Services;

public class PatientService(Apbd2Context ctx) : IPatientService
{
    public async Task<List<PatientResponse>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        var query = ctx.Patients
            .Include(p => p.Admissions).ThenInclude(a => a.Ward)
            .Include(p => p.BedAssignments).ThenInclude(ba => ba.Bed).ThenInclude(b => b.BedType)
            .Include(p => p.BedAssignments).ThenInclude(ba => ba.Bed).ThenInclude(b => b.Room).ThenInclude(r => r.Ward)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, $"%{search}%") || EF.Functions.Like(p.LastName, $"%{search}%"));

        return await query
            .OrderBy(p => p.LastName)
            .Select(p => new PatientResponse
            {
                Pesel = p.Pesel,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Sex = p.Sex ? "Male" : "Female",
                Admissions = p.Admissions.Select(a => new AdmissionResponse
                {
                    Id = a.Id,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Ward = new WardResponse { Id = a.Ward.Id, Name = a.Ward.Name, Description = a.Ward.Description }
                }).ToList(),
                BedAssignments = p.BedAssignments.Select(ba => new BedAssignmentResponse
                {
                    Id = ba.Id,
                    From = ba.From,
                    To = ba.To,
                    Bed = new BedResponse
                    {
                        Id = ba.Bed.Id,
                        BedType = new BedTypeResponse { Id = ba.Bed.BedType.Id, Name = ba.Bed.BedType.Name, Description = ba.Bed.BedType.Description },
                        Room = new RoomResponse
                        {
                            Id = ba.Bed.Room.Id,
                            HasTv = ba.Bed.Room.HasTv,
                            Ward = new WardResponse { Id = ba.Bed.Room.Ward.Id, Name = ba.Bed.Room.Ward.Name, Description = ba.Bed.Room.Ward.Description }
                        }
                    }
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<(bool patientFound, bool wardFound, bool bedTypeFound, int? assignmentId)> AssignBedAsync(string pesel, BedAssignmentRequest dto, CancellationToken cancellationToken)
    {
        var patientExists = await ctx.Patients.AnyAsync(p => p.Pesel == pesel, cancellationToken);
        if (!patientExists)
            return (false, false, false, null);

        var ward = await ctx.Wards.FirstOrDefaultAsync(w => w.Name == dto.Ward, cancellationToken);
        if (ward is null)
            return (true, false, false, null);

        var bedType = await ctx.BedTypes.FirstOrDefaultAsync(bt => bt.Name == dto.BedType, cancellationToken);
        if (bedType is null)
            return (true, true, false, null);

        var to = dto.To ?? DateTime.MaxValue;

        var availableBed = await ctx.Beds
            .Where(b =>
                b.BedTypeId == bedType.Id &&
                b.Room.WardId == ward.Id &&
                !b.BedAssignments.Any(ba =>
                    ba.From < to &&
                    (ba.To == null || ba.To > dto.From)))
            .FirstOrDefaultAsync(cancellationToken);

        if (availableBed is null)
            return (true, true, true, null);

        var assignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = availableBed.Id,
            From = dto.From,
            To = dto.To
        };

        ctx.BedAssignments.Add(assignment);
        await ctx.SaveChangesAsync(cancellationToken);

        return (true, true, true, assignment.Id);
    }
}