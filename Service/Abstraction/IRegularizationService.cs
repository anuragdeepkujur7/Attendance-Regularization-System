using Microsoft.AspNetCore.Mvc;
using Project_6_final.DTOs;
using Project_6_final.Models;

namespace Project_6_final.Service.Abstraction
{
    public interface IRegularizationService
    {
        Task<Attendance> GetAttendanceByDateAsync(Guid userId, DateOnly date);
       
        Task<bool> UpdateRequestStatus(Guid empId, Guid requestId, bool isApproved);
        Task<IEnumerable<Regularizationrequest>> GetRequestsByUserAsync(Guid userId);
    }
}
