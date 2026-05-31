using Project_6_final.DTOs;
using Project_6_final.Models;

namespace Project_6_final.Data.Repository.Abstraction
{
    public interface IRegularizationRepository
    {
        Task<Attendance> GetAttendanceByDateAsync(Guid userId, DateOnly date);
        
        Task<bool> UpdateRequestStatus(Guid empId, Guid requestId, bool isApproved); // scaffold the error
        Task<IEnumerable<Regularizationrequest>> GetRequestsByUserAsync(Guid userId);  

        Task<string> AdminRegularization(Guid userId, Guid requestId, AdminRegularizationDTO adminRegularizationDTO); //Done



        
    }
}
