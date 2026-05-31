using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_6_final.Data.Repository;
using Project_6_final.Data.Repository.Abstraction;
using Project_6_final.DTOs;
using Project_6_final.Models;
using Project_6_final.Service.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Project_6_final.Service.Implementation
{
    public class RegularizationService: IRegularizationService
    {
        private readonly IRegularizationRepository _regularizationRepository;
        public IHttpContextAccessor httpContextAccessor { get; }
        public RegularizationService(IRegularizationRepository regularizationRepository)
        {
            _regularizationRepository = regularizationRepository;
        }
       
        public async Task<bool> UpdateRequestStatus(Guid empId, Guid requestId, bool isApproved)
            {
                try
                {
                    var res = await _regularizationRepository.UpdateRequestStatus(empId, requestId, isApproved);

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
            return await _regularizationRepository.GetAttendanceByDateAsync(userId, date);
        }
       
    
        public async Task<IEnumerable<Regularizationrequest>> GetRequestsByUserAsync(Guid userId)
        {
            return await _regularizationRepository.GetRequestsByUserAsync(userId);
        }
    }
}
