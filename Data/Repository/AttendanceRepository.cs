using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project_6_final.Data.Repository.Abstraction;
using Project_6_final.DTOs;
using Project_6_final.Models;
using System;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Project_6_final.Data.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AttendanceContext _context;

        public AttendanceRepository(AttendanceContext context)
        {
            _context = context;
        }

      



        public async Task<IEnumerable<Attendance>> GetUserReport(Guid? userId, int month)
        {
            /*return await _context.Attendances
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .ToListAsync();*/
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == userId);
                var attendance = await _context.Attendances.Where(mem => mem.UserId == user.UserId).Select(mem => mem).ToListAsync();
                if (attendance != null)
                {
                    return attendance;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task AddAttendance(Attendance attendance)
        {
            var res = await _context.Attendances.ToListAsync();
            var existingAttendance = res.FirstOrDefault(a => a.UserId == attendance.UserId && a.AttendanceDate == DateOnly.FromDateTime(DateTime.Now));

            if (existingAttendance != null)
            {
                Attendance existing = await GetAttendanceById(existingAttendance.AttendanceId);
                existing.CreatedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return;
            }

            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }  

        public async Task AddMemberRegularizationRequest(Regularizationrequest regularizationrequest)  
        {
            try
            {
                Attendance attendance = await GetAttendanceById(regularizationrequest.AttendanceId);
                var request = await _context.Regularizationrequests.FirstOrDefaultAsync(rr => rr.AttendanceId == regularizationrequest.AttendanceId);

                if (regularizationrequest.UserId == Guid.Empty)
                {
                    if (request == null && attendance.Status == "Absent")
                    {
                        regularizationrequest.UserId = attendance.UserId;
                        await _context.Regularizationrequests.AddAsync(regularizationrequest);
                        await _context.SaveChangesAsync();
                        return;
                    }
                    throw new InvalidOperationException("Request cannot be added for an invalid member.");
                }

                if (request == null && attendance.Status == "Absent" && attendance.UserId == regularizationrequest.UserId)
                {
                    await _context.Regularizationrequests.AddAsync(regularizationrequest);
                }
                else if (request != null)
                {
                    request.Reason = regularizationrequest.Reason;
                    request.CreatedOn = DateTime.Now;
                }
                else
                {
                    throw new InvalidOperationException("Request cannot be added for this attendance.");
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        
        public async Task<bool> PostRegularizeRequest(Guid? empId, Regularizationrequest request)
        {
            try
            {
                if (empId == null)
                {
                    var existingRequest = await _context.Regularizationrequests
                    .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.CreatedOn == request.CreatedOn);

                    if (existingRequest != null)
                    {
                        throw new InvalidOperationException($"You have already applied for a regularization request for the date ");//{request.RequestedDate.ToString("yyyy-MM-dd")}.");
                    }

                    var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Status == "Absent");

                    if (attendance == null)
                    {
                        throw new InvalidOperationException($"Attendance not found for the date ");
                    }
                    request.AttendanceId = attendance.AttendanceId;
                    await _context.Regularizationrequests.AddAsync(request);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var user = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == empId);
                    var existingRequest = await _context.Regularizationrequests
                    .FirstOrDefaultAsync(r => r.UserId == user.UserId && r.CreatedOn == request.CreatedOn);

                    if (existingRequest != null)
                    {
                        throw new InvalidOperationException($"You have already applied for a regularization request for the date");// {request.RequestedDate.ToString("yyyy-MM-dd")}.");
                    }

                    var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.UserId == user.UserId && a.Status == "Absent");

                    if (attendance == null)
                    {
                        throw new InvalidOperationException($"Attendance not found for the date ");//{attendance.AttendanceDate.ToString("yyyy-MM-dd")}.");
                    }

                    request.UserId = user.UserId;
                    request.AttendanceId = attendance.AttendanceId;
                    await _context.Regularizationrequests.AddAsync(request);
                    await _context.SaveChangesAsync();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        public async Task<Attendance> GetAttendanceById(Guid attendanceId)
        {
            var res = await _context.Attendances.FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
            return res;
        }
        public async Task<AttendanceDetailsDTO> GetMemberAttendanceById(Guid memberId)
        {
            var attendance = await _context.Attendances
            .GroupBy(a => a.UserId)
            .Select(group => new AttendanceDetailsDTO
            {
                MemberId = group.Key,
                FirstName = group.First().User.FirstName,
                LastName = group.First().User.LastName,
                Gender = group.First().User.Gender,
                Attendances = group.Select(a => new AttendanceRecordDTO
                {
                    AttendanceId = a.AttendanceId,
                    AttendanceDate = a.AttendanceDate,
                    CreatedOn = a.LoginTime,
                    Status = a.Status,


                }).ToList()
            }).FirstOrDefaultAsync(a => a.MemberId == memberId);


            return attendance;
        }
        public async Task<Attendance?> GetAttendanceByDateAsync(Guid userId, DateOnly attendanceDate)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == userId && a.AttendanceDate == attendanceDate);
        }

        public async Task<bool> AddAttendanceAsync(Attendance attendance)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Attendances.AddAsync(attendance);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error adding attendance: {ex.Message}");
                    return false;
                }
            }
        }
        
        public async Task UpdateAttendance(Guid attendanceId, AttendanceUpdateDTO attendanceUpdateDTO)
        {
            var res = await _context.Attendances.FirstOrDefaultAsync(c => c.AttendanceId == attendanceId);
            res.AttendanceDate = attendanceUpdateDTO.AttendanceDate;
            res.Status = attendanceUpdateDTO.Status;
            await _context.SaveChangesAsync();
            //return true;
        }

        /*public async Task AddAttendanceUser(Attendance attendance)
        {
          
            var res = await _context.Attendances.ToListAsync();
            var existingAttendance = res.FirstOrDefault(a => a.UserId == attendance.UserId&& a.AttendanceDate == DateOnly.FromDateTime(DateTime.Now));
            if (existingAttendance != null)
            {
                Attendance existing = await GetAttendanceById(existingAttendance.AttendanceId);
                existing.CreatedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                return;
            }
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();

        }*/
        public async Task<bool> AttendancePunchIn(Guid memberId, Attendance attendance)
        {
            try
            {
                var sameDayAttendance = await _context.Attendances.FirstOrDefaultAsync(mem => mem.UserId == memberId && mem.AttendanceDate == attendance.AttendanceDate);
                if (sameDayAttendance != null)
                {

                    await _context.SaveChangesAsync();
                    return true;
                }
                await _context.Attendances.AddAsync(attendance);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<Attendance>> GetAttendanceById(Guid? employeeId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == employeeId);
                var attendance = await _context.Attendances.Where(mem => mem.UserId == user.UserId && (mem.Status == "Half-day" || mem.Status == "Absent")).Select(mem => mem).ToListAsync();
                if (attendance != null)
                {
                    return attendance;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Attendance>> GetAllUsersAttendanceAsync()
        {
            return await _context.Attendances.Include(a => a.User).ToListAsync();
        }


        /***********Regularization**************************/



        public async Task<bool> PostRegularizationRequestRepo(Guid? empId, Regularizationrequest request)
        {
            try
            {
                if (empId == null)
                {
                    var existingRequest = await _context.Regularizationrequests
                        .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.CreatedOn == request.CreatedOn);

                    if (existingRequest != null)
                    {
                        throw new InvalidOperationException($"You have already applied for a regularization request for the date"); //{request.RequestedDate.ToString("yyyy-MM-dd")}.");
                    }

                    var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId);

                    if (attendance == null)
                    {
                        throw new InvalidOperationException($"Attendance not found for this date");//{attendance.AttendanceDate.ToString("yyyy-MM-dd")}.");
                    }
                    request.AttendanceId = attendance.AttendanceId;
                    await _context.Regularizationrequests.AddAsync(request);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var user = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == empId);
                    var existingRequest = await _context.Regularizationrequests
                    .FirstOrDefaultAsync(r => r.UserId == user.UserId);//&& r.da == request.RequestedDate);

                    if (existingRequest != null)
                    {
                        throw new InvalidOperationException($"You have already applied for a regularization request for the date");// {request.RequestedDate.ToString("yyyy-MM-dd")}.");
                    }

                    var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.UserId == user.UserId);//&& a.AttendanceDate== request.RequestedDate && (a.Status == "Half-Day" || a.Status == "Absent"));

                    if (attendance == null)
                    {
                        throw new InvalidOperationException($"Attendance not found for this date");// {attendance.AttendanceDate.ToString("yyyy-MM-dd")}.");
                    }

                    request.UserId = user.UserId;
                    request.AttendanceId = attendance.AttendanceId;
                    await _context.Regularizationrequests.AddAsync(request);
                    await _context.SaveChangesAsync();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<bool> UpdateRequestStatus(Guid empId, Guid requestId, bool isApproved)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == empId);
                var existingRequest = await _context.Regularizationrequests.FirstOrDefaultAsync(a => a.RequestId == requestId && a.UserId == user.UserId);// && a.Status.ToLower() == "pending");

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
        /* public async Task<string> AdminRegularization(Guid userId, Guid requestId, AdminRegularizationDTO adminRegularizationDTO)
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
         }*/

        //21-1
        public async Task<IEnumerable<AttendanceDetailsDTO>> GetAttendance()
        {
            var groupedAttendance = _context.Attendances
            .GroupBy(a => a.UserId)
            .Select(group => new AttendanceDetailsDTO
            {
                MemberId = group.Key,
                FirstName = group.First().User.FirstName,
                LastName = group.First().User.LastName,
                Gender = group.First().User.Gender,
                Attendances = group.Select(a => new AttendanceRecordDTO
                {
                    AttendanceId = a.AttendanceId,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status,
                    CreatedOn = a.CreatedOn
                }).ToList()
            });

            return await groupedAttendance.ToListAsync();
        }
        public async Task<string> AdminRegularization(Guid userId, Guid requestId, AdminRegularizationDTO adminRegularizationDTO)
        {
            try
            {
                var request = await _context.Regularizationrequests.FirstOrDefaultAsync(rr => rr.RequestId == requestId);
                if (request == null) throw new KeyNotFoundException("Regularization request not found.");

                var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.AttendanceId == request.AttendanceId);
               
                if (attendance.Status != "Absent" )
                {
                    if (attendance.Status != "Regularized")
                    {
                        throw new InvalidOperationException("Attendance record is invalid or not marked as Regularized or Absent.");
                    }
                }

                if (adminRegularizationDTO.Status == "Approved")
                {
                    attendance.Status = "Regularized";
                    await _context.SaveChangesAsync();
                }
                else if (adminRegularizationDTO.Status == "Denied")
                {
                    attendance.Status = "Absent";
                }
                else
                {
                    throw new ArgumentException("Invalid status type.");
                }

                request.Status = adminRegularizationDTO.Status;
                request.RequestedBy = userId;
                request.UpdatedOn=DateTime.UtcNow;

                //request.ReviewedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return request.Status;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                throw;
            }
        }

       
        public async Task<MemberAttendanceReportDTO?> GetMemberAttendanceReport(AttendanceReportDTO attendanceReportDTO)
        {
            var report = await _context.Attendances
                .Where(a => a.UserId == attendanceReportDTO.MemberId && a.AttendanceDate.Value.Month == attendanceReportDTO.Month && a.AttendanceDate.Value.Year == attendanceReportDTO.Year)
                .GroupBy(a => a.UserId)
                .Select(g => new MemberAttendanceReportDTO
                {
                    MemberId = g.Key,
                   FirstName=g.First().User.FirstName,
                    LastName = g.First().User.LastName,    
                    Gender = g.First().User.Gender,       
                    ReportId = Guid.NewGuid(),
                    Month = attendanceReportDTO.Month,
                    Year = attendanceReportDTO.Year,
                    TotalPresent = g.Count(a => a.Status == "Present"),
                    TotalAbsent = g.Count(a => a.Status == "Absent"),
                    TotalLeaves = g.Count(a => a.Status == "Leave"),
                    TotalRegularized = g.Count(a => a.Regularizationrequests.Any()), 
                    GeneratedAt = DateTime.UtcNow
                })
                .FirstOrDefaultAsync();

            return report; 
        }





    }
}
