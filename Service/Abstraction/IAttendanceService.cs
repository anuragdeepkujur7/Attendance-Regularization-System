using Project_6_final.DTOs;
using Project_6_final.Helper;
using Project_6_final.Models;

namespace Project_6_final.Service.Abstraction
{
    public interface IAttendanceService
    {
        //Task AddAttendanceAsync(AttendanceCreateDto attendanceCreateDto);
        //Task<bool> AddAttendanceAsync(); // Done
        Task<bool> AddAttendanceAsync(Guid userId);
        Task<bool> AddMemberAttendance();
        Task<AttendanceDetailsDTO> GetMemberAttendance();
        Task<bool> AddMemberRegularizationRequest(Guid attendanceId, AddRegularizationDTO addRegularizationDTO); //pri
        Task<bool> PostRegularizeRequest(Guid? empId, AddRegularizationDTO requestDTO); //ved
        Task<IEnumerable<AttendanceDTO>> GetAllUsersAttendanceAsync();   // Done
        //Task<User> GetMemberAttendance(); //Done
        //Task<AttendanceDTO> GetMemberAttendance(); //done
       // Task<List<UserAttendanceDTO>?> GetUsersAttendance(Guid? id); // done

        //Task<IEnumerable<AttendanceDTO>> GetUserReport(int? userId,int month); //here Done 
        Task<List<AttendanceDTO>> GetUserReport(int? id, int month);
        //Task UpdateAttendanceAsync(AttendanceUpdateDto attendanceDto);
        //Task UpdateAttendanceAsync(int attendanceId, AttendanceDTO attendanceDto);

        Task UpdateMemberAttendance(Guid attendanceId, AttendanceUpdateDTO attendanceUpdateDTO);// removed <bool,bool> done

        //Task<bool> UpdateRequestStatus(int empid, Guid requestId, bool isApproved);
        Task<bool> AttendancePunchIn();

        //regu
        Task<Attendance> GetAttendanceByDateAsync(Guid userId, DateOnly date);
        Task<bool> PostRegularizeRequest(Guid? userId, RegularizationDTO requestDTO);
        Task<bool> UpdateRequestStatus(Guid empId, Guid requestId, bool isApproved);
        Task<IEnumerable<Regularizationrequest>> GetRequestsByUserAsync(Guid userId);
        Task<RegularizationResult> AddRegularizationRequestForMember(Guid attendanceId, AddRegularizationDTO addRegularizationDTO);
        Task<IEnumerable<AttendanceDetailsDTO>> GetAttendaceDetails();
        Task<RegularizationResultDTO> AdminRegularization(Guid requestId, AdminRegularizationDTO adminRegularizationDTO);

        Task<MemberAttendanceReportDTO?> GetMemberAttendanceReportUser(UserAttendanceReportDTO userAttendanceReportDTO);




    }
}
