using Microsoft.AspNetCore.Http;
using Project_6_final.Data.Repository;
using Project_6_final.Data.Repository.Abstraction;
using Project_6_final.DTOs;
using Project_6_final.Models;
using Project_6_final.Service.Abstraction;
using System.Security.Claims;
using Project_6_final.Helper;
using System.Text.RegularExpressions;

namespace Project_6_final.Service.Implementation
{
    public class AttendanceService : IAttendanceService

    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public AttendanceService(IAttendanceRepository attendanceRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            
        }

        // need to be channged
        public bool GetMemberAttendanceReport(AttendanceReportDTO attendanceReportDTO)
        {
            return true;
        }

        /*public async Task<bool> AddAttendanceAsync()
        {
            try
            {
              
                Guid userId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Attendance attendance = new Attendance();
                attendance.UserId = userId;
                Guid attendanceId = Guid.NewGuid();
                attendance.AttendanceId = attendanceId;
                attendance.AttendanceDate = DateOnly.FromDateTime(DateTime.Now);
                //attendance.Status = "Present";
                attendance.CreatedOn = DateTime.Now;
                attendance.UpdatedOn = DateTime.Now;
                attendance.LoginTime = DateTime.Now;
                attendance.LogoutTime = DateTime.Now;
                var res = await _attendanceRepository.AttendancePunchIn(userId, attendance);
                if (res)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);

            }
            return false;
        }*/
        public async Task<bool> AddMemberAttendance()   // in use
        {
            try
            {
                
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimValue))
                {
                    throw new UnauthorizedAccessException("User is not logged in.");
                }

                if (!Guid.TryParse(claimValue, out Guid userId))
                {
                    throw new FormatException("User ID claim is not in the correct format.");
                }

                
                Attendance attendance = new Attendance()
                {
                    AttendanceId = Guid.NewGuid(),
                    UserId = userId,
                    AttendanceDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = "Present",
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    LoginTime = DateTime.UtcNow,
                    LogoutTime= DateTime.UtcNow,
                };

                
                await _attendanceRepository.AddAttendance(attendance);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception)
            {
                return false; 
            }
        }

        //pri
        public async Task<bool> AddMemberRegularizationRequest(Guid attendanceId, AddRegularizationDTO addRegularizationDTO)
        {
            try
            {
                
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimValue) || !Guid.TryParse(claimValue, out Guid userId))
                {
                    return false; // User not logged in or invalid user ID
                }

                Regularizationrequest req = new Regularizationrequest
                {
                    RequestId = Guid.NewGuid(),
                    AttendanceId = attendanceId,
                    CreatedOn = DateTime.UtcNow, 
                    //UpdatedOn = DateTime.UtcNow,
                    Reason = addRegularizationDTO.Reason,
                    UserId = userId,
                    RequestedBy= userId,
                    //Status="Pending",
                    
                };

                try
                {
                    await _attendanceRepository.AddMemberRegularizationRequest(req);
                    return true; // Successfully added the request
                }
                catch
                {
                    return false; // Failed to add the request
                }
            }
            catch
            {
                return false; // Failed due to invalid user context or other reasons
            }
        }

        public async Task<bool> PostRegularizeRequest(Guid? empId, AddRegularizationDTO requestDTO)
        {
            try
            {
                if (empId == null)
                {
                    Guid userId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    Regularizationrequest request = new Regularizationrequest();
                    request.RequestId = Guid.NewGuid();
                    request.UserId = userId;
                    //request.CreatedOn = DateOnly.FromDateTime(Da);
                    
                    request.Reason = requestDTO.Reason;
                    request.Status = "Pending";
                    var res = await _attendanceRepository.PostRegularizeRequest(null, request);
                    if (res)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {

                    Regularizationrequest request = new Regularizationrequest();
                    request.RequestId = Guid.NewGuid();
                    //request.RequestedDate = DateOnly.FromDateTime(requestDTO.AttendanceDate);
                   
                    request.CreatedOn = DateTime.Now;
                   
                    request.Reason = requestDTO.Reason;
                    request.Status = "Pending";
                    var res = await _attendanceRepository.PostRegularizeRequest(empId, request);
                    if (res)
                    {
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<AttendanceDetailsDTO> GetMemberAttendance()
        {
            try
            {
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimValue))
                {
                    throw new InvalidOperationException("User ID claim is missing or invalid.");
                }

                if (!Guid.TryParse(claimValue, out Guid userId))
                {
                    throw new FormatException("User ID claim is not in the correct format.");
                }


                try
                {
                    var attendaces = await _attendanceRepository.GetMemberAttendanceById(userId);
                    return attendaces;
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred while getting member attendance details:");
                    return null;
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool> AddAttendanceAsync(Guid userId)
        {
            // Check if attendance for the given date already exists
            var attendanceDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var existingAttendance = await _attendanceRepository.GetAttendanceByDateAsync(userId, attendanceDate);

            if (existingAttendance != null)
            {
                throw new Exception("Attendance for this date already exists.");
            }

            var attendance = new Attendance
            {
                AttendanceId = Guid.NewGuid(),
                UserId = userId,
                AttendanceDate = attendanceDate,
                Status = "Present", // Default status is 'Present'
                LoginTime = DateTime.UtcNow, // Current UTC time as login time
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            return await _attendanceRepository.AddAttendanceAsync(attendance);
        }
        public async Task<List<AttendanceDTO>> GetUserReport(int? id,int month)   
        {
            //var attendanceRecords = await _attendanceRepository.GetAttendanceByUserIdAsync(id);

            // map to dto
            /*return attendanceRecords.Select(record => new AttendanceDTO
            {

                AttendanceId = record.AttendanceId,
                AttendanceDate = record.AttendanceDate,
                Status = record.Status,
                LoginTime = record.LoginTime,
                LogoutTime = record.LogoutTime
            });*/
            try
            {
                //var user = await userRepository.GetUserByEmail();
                if (id != null)
                {
                    var res = await _attendanceRepository.GetUserReport(null, month);

                    List<AttendanceDTO> userAttendance = new List<AttendanceDTO>();
                    foreach (var item in res)
                    {
                        userAttendance.Add(new AttendanceDTO()
                        {
                            UserId = item.User.UserId,
                            AttendanceDate = item.AttendanceDate,
                            FirstName = item.User.FirstName,
                            LastName = item.User.LastName,
                            LoginTime = item.LoginTime,
                            LogoutTime = item.LogoutTime,
                            Status = item.Status
                        });
                    }
                    return userAttendance;
                }
                else
                {
                    var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
                    var user = await _userRepository.GetUserByEmailAsync(userEmail);

                    var res = await _attendanceRepository.GetUserReport(user.UserId,month);
                    /* if (res.Count == 0)
                     {
                         return null;
                     }*/
                    List<AttendanceDTO> userAttendance = new List<AttendanceDTO>();
                    foreach (var item in res)
                    {
                        userAttendance.Add(new AttendanceDTO()
                        {
                            UserId = item.User.UserId,
                            AttendanceDate = item.AttendanceDate,
                            FirstName = item.User.FirstName,
                            LastName = item.User.LastName,
                            LoginTime = item.LoginTime,
                            LogoutTime = item.LogoutTime,
                            Status = item.Status
                        });
                    }
                    return userAttendance;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return null;
            }
        }
        
        

        /* public async Task<AttendanceDTO> GetMemberAttendance()
         {
             try
             {
                 var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                 if (string.IsNullOrEmpty(claimValue))
                 {
                     throw new InvalidOperationException("User ID claim is missing or invalid.");
                 }

                 if (!Guid.TryParse(claimValue, out Guid userId))
                 {
                     throw new FormatException("User ID claim is not in the correct format.");
                 }


                 try
                 {
                     var attend = await _userRepository.GetUserByIdAsync(userId);
                     //map attendance dto into user model
                     return attend;
                 }

                 catch (Exception ex)
                 {
                     Console.WriteLine("Error occurred while getting member attendance details:");
                     return null;
                 }



             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message);
                 return null;
             }
         }*/

        /* public async Task UpdateAttendanceAsync(int attendanceId, AttendanceDTO attendanceDto)
         {
             var attendance = await _attendanceRepository.GetAttendanceByUserIdAsync(attendanceId);
             if (attendance == null)
             {
                 throw new Exception("Attendance record not found");
             }

             attendance.Status = attendanceDto.Status;
             attendance.LoginTime = attendanceDto.LoginTime;
             attendance.LogoutTime = attendanceDto.LogoutTime;
             attendance.UpdatedOn = DateTime.UtcNow;

             await _attendanceRepository.UpdateAttendanceAsync(attendance);
         }*/

        /* public async Task<List<UserAttendanceDTO>?> GetUsersAttendance(Guid? id)
         {
             try
             {

                 if (id != null)
                 {
                     var res = await _attendanceRepository.GetAttendanceById(id);

                     List<UserAttendanceDTO> userAttendance = new List<UserAttendanceDTO>();
                     foreach (var item in res)
                     {
                         userAttendance.Add(new UserAttendanceDTO()
                         {
                             EmployeeId = item.User.UserId,
                             AttendanceDate = item.AttendanceDate,
                             FirstName = item.User.FirstName,
                             LastName = item.User.LastName,
                             LoginTime = item.LoginTime,
                             LogoutTime = item.LogoutTime,
                             Status = item.Status
                         });
                     }
                     return userAttendance;
                 }
                 else
                 {
                     var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
                     var user = await _userRepository.GetUserByEmailAsync(userEmail);
                     var res = await _attendanceRepository.GetAttendanceById(user.UserId);
                     if (res.Count == 0)
                     {
                         return null;
                     }
                     List<UserAttendanceDTO> userAttendance = new List<UserAttendanceDTO>();
                     foreach (var item in res)
                     {
                         userAttendance.Add(new UserAttendanceDTO()
                         {
                             EmployeeId = item.User.UserId,
                             AttendanceDate = item.AttendanceDate,
                             FirstName = item.User.FirstName,
                             LastName = item.User.LastName,
                             LoginTime = item.LoginTime,
                             LogoutTime = item.LogoutTime,
                             Status = item.Status
                         });
                     }
                     return userAttendance;
                 }

             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message);
                 Console.WriteLine(ex.InnerException);
                 return null;
             }
         }*/
        public async Task UpdateMemberAttendance(Guid attendanceId, AttendanceUpdateDTO attendanceUpdateDTO)
            {
            try
            {
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var claimValue2 = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(claimValue) && string.IsNullOrEmpty(claimValue2))
                {
                    throw new InvalidOperationException("User ID claim is missing or invalid.");
                }

                if (!int.TryParse(claimValue, out int userId))
                {
                    throw new FormatException("User ID claim is not in the correct format.");
                }

                try
                {
                    await _attendanceRepository.UpdateAttendance(attendanceId, attendanceUpdateDTO);
                    //return (true, true);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //return (false, true);
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //return (false, false);
            }
        }
       public async Task<IEnumerable<AttendanceDTO>> GetAllUsersAttendanceAsync()
            {   
                var attendanceRecords = await _attendanceRepository.GetAllUsersAttendanceAsync();
                return attendanceRecords.Select(a => new AttendanceDTO
                {
                AttendanceId = a.AttendanceId,
                UserId = a.UserId,
                AttendanceDate = a.AttendanceDate,
                Status = a.Status,
                LoginTime = a.LoginTime,
                LogoutTime = a.LogoutTime
                });
        }

       public async Task<bool> AttendancePunchIn()
        {
            try
            {
                Guid userId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Attendance attendance = new Attendance();
                attendance.UserId = userId;
                Guid attendanceId = Guid.NewGuid();
                attendance.AttendanceId = attendanceId;
                attendance.AttendanceDate = DateOnly.FromDateTime(DateTime.Now);
                //attendance.Status = "Present";
                attendance.CreatedOn = DateTime.Now;
                attendance.UpdatedOn = DateTime.Now;
                attendance.LoginTime = DateTime.Now;
                attendance.LogoutTime = DateTime.Now;
                var res = await _attendanceRepository.AttendancePunchIn(userId, attendance);
                if (res)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            return false;
        }


        public async Task<bool> PostRegularizeRequest(Guid? userId, RegularizationDTO requestDTO)
        {
            try
            {
                if (userId == null)
                {
                    Guid Id = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    Regularizationrequest request = new Regularizationrequest();
                    request.RequestId = Guid.NewGuid();
                    request.UserId = Id;
                    //request.CreatedOn = DateOnly.FromDateTime(requestDTO.Date);
                    request.CreatedOn = DateTime.Now;
                    request.UpdatedOn = DateTime.Now;
                    request.Reason = requestDTO.Reason;
                    request.Status = "Pending";
                    var res = await _attendanceRepository.PostRegularizationRequestRepo(null, request);
                    if (res)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {

                    Regularizationrequest request = new Regularizationrequest();
                    request.RequestId = Guid.NewGuid();
                    //request.RequestedDate = DateOnly.FromDateTime(requestDTO.AttendanceDate);
                    request.CreatedOn = DateTime.Now;
                    request.UpdatedOn = DateTime.Now;
                    request.Reason = requestDTO.Reason;
                    request.Status = "Pending";
                    var res = await _attendanceRepository.PostRegularizationRequestRepo(userId, request);
                    if (res)
                    {
                        return true;
                    }
                    return false;
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
                var res = await _attendanceRepository.UpdateRequestStatus(empId, requestId, isApproved);

                if (res != null)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        public async Task<Attendance> GetAttendanceByDateAsync(Guid userId, DateOnly date)
        {
            return await _attendanceRepository.GetAttendanceByDateAsync(userId, date);
        }
        public async Task<bool> SubmitRegularizationRequestAsync(Guid userId, RegularizationDTO request)
        {


            var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null)
            {
                throw new InvalidOperationException("No attendance record found for the specified user and date.");
            }

            // Create the RegularizationRequest entity
            Regularizationrequest req = new Regularizationrequest
            {
                RequestId = Guid.NewGuid(),
                UserId = userId,
                Reason = request.Reason,
                Status = "Pending",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            try
            {
                await _attendanceRepository.PostRegularizationRequestRepo(userId, req);
                return (true);
            }

            catch (Exception ex)
            {
                return (false);
            }

        }

        public async Task<IEnumerable<Regularizationrequest>> GetRequestsByUserAsync(Guid userId)
        {
            return await _attendanceRepository.GetRequestsByUserAsync(userId);
        }

        //pri
        public async Task<RegularizationResult> AddRegularizationRequestForMember(Guid attendanceId, AddRegularizationDTO addRegularizationDTO)
        {
            var result = new RegularizationResult();

            try
            {
                // Extract the user ID from claims
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimValue))
                {
                    throw new InvalidOperationException("User ID claim is missing or invalid.");
                }

                if (!Guid.TryParse(claimValue, out Guid userId))
                {
                    throw new FormatException("User ID claim is not in the correct format.");
                }

                result.IsLoggedIn = true;

                // Create a new regularization request
                Regularizationrequest req = new Regularizationrequest()
                {
                    RequestId = Guid.NewGuid(),
                    AttendanceId = attendanceId,
                    Reason = addRegularizationDTO.Reason,
                    RequestedBy = userId,
                    CreatedOn = DateTime.Now
                };

                try
                {
                    await _attendanceRepository.AddMemberRegularizationRequest(req);
                    result.IsSuccessful = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccessful = false;
                result.IsLoggedIn = false;
            }

            return result;
        }


        //pri
        public async Task<IEnumerable<AttendanceDetailsDTO>> GetAttendaceDetails()
        {
            try
            {
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimValue))
                {
                    throw new InvalidOperationException("User ID claim is missing or invalid.");
                }

                if (!Guid.TryParse(claimValue, out Guid userId))
                {
                    throw new FormatException("User ID claim is not in the correct format.");
                }


                try
                {
                    var attendaces = await _attendanceRepository.GetAttendance();
                    return attendaces;
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred while getting attendance details:");
                    return null;
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //pri
        public async Task<RegularizationResultDTO> AdminRegularization(Guid requestId, AdminRegularizationDTO adminRegularizationDTO)
        {
            var result = new RegularizationResultDTO();

            try
            {
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimValue))
                {
                    result.IsAuthenticated = false;
                    return result;
                }

                if (!Guid.TryParse(claimValue, out Guid userId))
                {
                    result.IsAuthenticated = false;
                    return result;
                }

                try
                {
                    var status = await _attendanceRepository.AdminRegularization(userId, requestId, adminRegularizationDTO);
                    result.IsSuccessful = true;
                    result.IsAuthenticated = true;
                    result.Status = status;
                }
                catch
                {
                    result.IsSuccessful = false;
                    result.IsAuthenticated = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccessful = false;
                result.IsAuthenticated = false;
            }

            return result;
        }
        //Ch
        public async Task<MemberAttendanceReportDTO?> GetMemberAttendanceReportUser(UserAttendanceReportDTO userAttendanceReportDTO)
        {
            try
            {
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimValue))
                {
                    throw new InvalidOperationException("User ID claim is missing or invalid.");
                }

                if (!Guid.TryParse(claimValue, out Guid userId))
                {
                    throw new FormatException("User ID claim is not in the correct format.");
                }

                var attendanceReportDTO = new AttendanceReportDTO
                {
                    MemberId = userId,
                    Year = userAttendanceReportDTO.Year,
                    Month = userAttendanceReportDTO.Month
                };

                var attendanceReport = await _attendanceRepository.GetMemberAttendanceReport(attendanceReportDTO);
                return attendanceReport;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }


    }
}
