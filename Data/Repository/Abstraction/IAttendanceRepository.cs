using Project_6_final.DTOs;
using Project_6_final.Models;

namespace Project_6_final.Data.Repository.Abstraction
{
    public interface IAttendanceRepository
    {
       
        Task<IEnumerable<Attendance>> GetUserReport(Guid? userId, int month);//Done
        Task AddAttendance(Attendance attendance);
        Task<AttendanceDetailsDTO> GetMemberAttendanceById(Guid memberId);
        Task AddMemberRegularizationRequest(Regularizationrequest regularizationrequest); //inuse
        Task<bool> PostRegularizeRequest(Guid? empId, Regularizationrequest request);
        Task<Attendance> GetAttendanceById(Guid attendanceId);
        Task<Attendance?> GetAttendanceByDateAsync(Guid userId, DateOnly attendanceDate);
        Task<bool> AddAttendanceAsync(Attendance attendance);
        
        Task<List<Attendance>> GetAttendanceById(Guid? employeeId); // GetUsersAttendance

        
        Task UpdateAttendance(Guid attendanceId, AttendanceUpdateDTO attendanceUpdateDTO); // removed bool

        Task<bool> AttendancePunchIn(Guid memberId, Attendance attendance);

        Task<IEnumerable<Attendance>> GetAllUsersAttendanceAsync();


        //*********ReguLarization*********//
        Task<bool> PostRegularizationRequestRepo(Guid? userId, Regularizationrequest request); // done
        Task<bool> UpdateRequestStatus(Guid empId, Guid requestId, bool isApproved); 
        Task<IEnumerable<Regularizationrequest>> GetRequestsByUserAsync(Guid userId);

        Task<string> AdminRegularization(Guid userId, Guid requestId, AdminRegularizationDTO adminRegularizationDTO); //Done
        
        Task<IEnumerable<AttendanceDetailsDTO>> GetAttendance();

        Task<MemberAttendanceReportDTO?> GetMemberAttendanceReport(AttendanceReportDTO attendanceReportDTO);

        




    }
}
