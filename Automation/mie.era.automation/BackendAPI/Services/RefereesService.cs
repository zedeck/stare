using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BackendAPI.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class RefereesService
    {
        private readonly ERADBContext _context;

        public RefereesService(ERADBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<RefereeInfo>> GetRefereesWithCompletedReferencesAsync(CancellationToken cancellationToken = default)

        {
            try
            {
                var startDate = DateTime.Now.AddMonths(-1);
                var endDate = DateTime.Now;

                var result = await _context.RefereesInfo
                    .FromSqlRaw("EXEC USP_GetRefereeUpdate @StartDate, @EndDate, @p1, @p2",
                        new SqlParameter("@StartDate", startDate),
                        new SqlParameter("@EndDate", endDate),
                        new SqlParameter("@p1", 1),
                        new SqlParameter("@p2", 2))
                    .ToListAsync(cancellationToken).ConfigureAwait(true);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<List<RefereeInfo>> GetReferencesByFilterAsync(string filterText, CancellationToken cancellationToken = default)

        {
            try
            {
                var startDate = DateTime.Now.AddMonths(-1);
                var endDate = DateTime.Now;

                var result = await _context.RefereesInfo
                    .FromSqlRaw("EXEC usp_GetReferencesByFilter @RemoteKey",
                        new SqlParameter("@RemoteKey", filterText))
                    .ToListAsync(cancellationToken).ConfigureAwait(true);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public class RefereeInfo
        {

            public int? UserID { get; set; }
            public DateTime? RequestDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public string? Status { get; set; }
            public int? QuestionSetID { get; set; }
            public string? RemoteKey { get; set; }
            public int? RequestID { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public bool? IsActive { get; set; }
            public string? Relationship { get; set; }
            public string? CandidateName { get; set; }
            public string? CandidateSurname { get; set; }
            public DateTime? CandidateDOB { get; set; }
            public string? CandidateCell { get; set; }
            public string? CandidateEmail { get; set; }
            public string? CandidateId { get; set; }
            public string? CandidatePassport { get; set; }
            public string? SigEmail { get; set; }
            public string? SigName { get; set; }
            public string? TotalReminders { get; set; }
            public string? LastReminder { get; set; }
            public bool LastReminderDue { get; set; }
        }
    }
}
