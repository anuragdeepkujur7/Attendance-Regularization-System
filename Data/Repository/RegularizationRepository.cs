using Microsoft.EntityFrameworkCore;
using Project_6_final.Data.Repository.Abstraction;
using Project_6_final.DTOs;
using Project_6_final.Models;
using System;

namespace Project_6_final.Data.Repository
{
    public class RegularizationRepository: IRegularizationRepository
    {
        private readonly AttendanceContext _context;

        public RegularizationRepository(AttendanceContext context)
        {
            _context = context;
        }

        public async Task<Attendance> GetAttendanceByDateAsync(Guid userId, DateOnly date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == userId && a.AttendanceDate == date);
        }


        
        public async Task<bool> UpdateRequestStatus(Guid empId, Guid requestId, bool isApproved)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == empId);
                var existingRequest = await _context.Regularizationrequests.FirstOrDefaultAsync(a => a.RequestId == requestId && a.UserId == user.UserId && a.Status.ToLower() == "pending");

                if (existingRequest == null)
                {
                    throw new InvalidOperationException("Request has already been regularized");
                }
                if (isApproved)
                {
                    existingRequest.Status = "Approved";
                }
                else
                {
                    existingRequest.Status = "Denied";
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        public async Task<IEnumerable<Regularizationrequest>> GetRequestsByUserAsync(Guid userId)
        {
            return await _context.Regularizationrequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Attendance)
                .ToListAsync();
        }
        public async Task<string> AdminRegularization(Guid userId, Guid requestId, AdminRegularizationDTO adminRegularizationDTO)
        {
            try
            {
                var request = await _context.Regularizationrequests.FirstOrDefaultAsync(rr => rr.RequestId == requestId);
                var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.AttendanceId == request.AttendanceId);
                if (attendance.Status == "Absent")
                {
                    if (adminRegularizationDTO.Status == "Approved")
                    {
                        attendance.Status = "Regularized";
                    }

                    else if (adminRegularizationDTO.Status == "Denied")
                    {
                        attendance.Status = "Absent";
                    }
                    request.Status = adminRegularizationDTO.Status;
                    request.CreatedOn = DateTime.Now;
                    request.UpdatedOn = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return request.Status;
                }
                else
                {
                    throw new InvalidOperationException();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                throw ex;
            }
        }
        
    
    }
}
