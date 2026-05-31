using Microsoft.EntityFrameworkCore;
using Project_6_final.Data.Repository.Abstraction;
using Project_6_final.DTOs;
using Project_6_final.Models;
using System.Runtime.CompilerServices;

namespace Project_6_final.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AttendanceContext _context;

        public UserRepository(AttendanceContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUserAsync(User member)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    await _context.Users.AddAsync(member);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            }

        }
        public async Task<bool> CheckUserInDb(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        //latest
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        public async Task UpdateUserAsync2(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUserDetails()   // Done
        {
            
            List<User> res = await _context.Users.Include(a => a.Attendances).Include(member => member.Addresses)
                    .Include(member => member.Contacts).Include(member => member.Email).Include(member => member.Roles)
                    // add this later after scaffold .Include(member=>member.Regularizationrequest)

                    .ToListAsync();

            if (res.Count == 0)
            {
                return null;
            }
            return res;
        }

        /* public async Task<List<Attendance>> GetAttendanceReport(int employeeId,int month)
         {
             var member = await _context.Users.FirstOrDefaultAsync(m => m.UserId == employeeId);

             var attendanceReport = await _context.Attendances.Where(memAtt => memAtt.AttendanceDate.Value == month && memAtt.UserId == member.UserId).ToListAsync();
             *//*if (attendanceReport.Count == 0)
             {
                 return null;
             }

             return attendanceReport;
         }*/
        public async Task<MemberAttendanceReportDTO> GetMemberAttendanceReport(DateReportDTO dateReportDTO)
        {
            var attendanceReport = await _context.Monthlyattendances.Where(i => i.UserId == dateReportDTO.UserId &&
                                    i.Month == dateReportDTO.Month && i.Year == dateReportDTO.Year)
                .GroupBy(a => new
                {
                    a.UserId,
                    a.User.FirstName,
                    a.User.LastName,
                    a.User.Gender
                })
                .Select(group => new MemberAttendanceReportDTO
                {
                    //MemberId = group.Key.UserId,
                    FirstName = group.Key.FirstName,
                    LastName = group.Key.LastName,
                    Gender = group.Key.Gender,
                    //ReportId = group.First().ReportId,
                    Month = (int)group.First().Month,
                    Year = (int)group.First().Year,
                    TotalPresent = (int)group.Sum(a => a.TotalPresent),
                    TotalAbsent = (int)group.Sum(a => a.TotalAbsent),
                    TotalLeaves = (int)group.Sum(a => a.TotalLeave),
                    //TotalRegularized = (int)group.Sum(a => a.TotalRegularized),
                    GeneratedAt = DateTime.Now
                })
                .FirstOrDefaultAsync();
            return attendanceReport;
        }
        public async Task<User> GetUserByEmailAsync(string email) //in use
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UpdateUserAsync(Guid id, User member)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var res = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == member.UserId);
                    if (res == null)
                    {
                        Console.WriteLine($"user not found");
                        return false;
                    }
                    res.FirstName = member.FirstName ?? res.FirstName;
                    res.LastName = member.LastName ?? res.LastName;
                    res.DateOfBirth = member.DateOfBirth ?? res.DateOfBirth;
                    res.Gender = member.Gender ?? res.Gender;
                    //res.Email = member.Email ?? res.Email;
                    if (String.IsNullOrEmpty(member.Email))
                    {
                        member.Email = res.Email;
                    }
                    else
                    {

                        var existingMember = await _context.Users
                            .FirstOrDefaultAsync(mem => mem.Email == member.Email && mem.UserId != id);
                        if (existingMember != null)
                        {
                            Console.WriteLine("Duplicate email found.");
                            return false;
                        }
                        else
                        {
                            res.Email = member.Email;
                        }
                    }
                    var res1 = await _context.Users.FirstOrDefaultAsync(mem => mem.UserId == member.UserId);
                    if (res1.UserId == id)
                    {
                        Console.WriteLine("yes" + " " + res.UserId + " " + member.UserId);
                    }
                    else
                    {
                        return false;
                    }
                    //member.HashPassword = res.HashPassword;
                    //dbContext.Entry(dbContext.Members).CurrentValues.SetValues(member);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                   
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            }
        }


        public async Task<AttendanceDTO> GetUserByIdAsync(Guid? UserId)
        {
            var attendance = await _context.Attendances.GroupBy(a => a.UserId)
                .Select(group => new AttendanceDTO
                {
                    UserId = group.Key,
                    FirstName = group.First().User.FirstName,
                    LastName = group.First().User.LastName,
                    AttendanceDate = group.First().AttendanceDate,
                    Status = group.First().Status,
                    LoginTime = group.First().LoginTime,
                    AttendanceId = group.Key // create new dto for attendance record and then object??

                    /*Attendances = group.Select(a => new AttendanceRecordDTO
                    {
                        AttendanceId = a.AttendanceId,
                        Date = a.Date,
                        Status = a.Status,
                        CreatedAt = a.CreatedAt
                    }).ToList()*/
                }).FirstOrDefaultAsync(a => a.UserId == UserId);


            return attendance;
        }

        public async Task<List<User>> GetUserDataAsync(Guid? id)
        {
            try
            {
                var res = await _context.Users.Include(x => x.Addresses).Include(x => x.Contacts).Include(x => x.Educations).Where(x => x.UserId == id).ToListAsync();
                return res; ;//.Include(x => x.Employments).Where(x => x.UserId == id).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return null;
            }
        }
        public async Task<bool> UpdatePasswordAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task InvalidateTokenAsync(string token, Guid userId, DateTime expiration)
        {
            var invalidToken = new Invalidtoken
            {
                Token = token,
                Expiration = expiration,
                UserId = userId
            };
            _context.Invalidtokens.Add(invalidToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenInvalidatedAsync(string token)
        {
            return await _context.Invalidtokens.AnyAsync(it => it.Token == token && it.Expiration > DateTime.UtcNow);
        }
        public async Task<bool> IsTokenValidAsync(string token)

        {
            var invalidated = await _context.Invalidtokens
                .AnyAsync(t => t.Token == token && t.Expiration > DateTime.UtcNow);
            return !invalidated;

        }

        public async Task<bool> InvalidateToken(Guid id)
        {
            try
            {
                var resp = await _context.Invalidtokens.FirstOrDefaultAsync(session => session.Expiration > DateTime.UtcNow && session.UserId == id);
                if (resp == null)
                {
                    return false;
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

        //CONTACT
        public async Task<bool> AddContact(Contact contact)
        {
            try
            {
                await _context.Contacts.AddAsync(contact);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contact: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateContact(Contact contact)
        {
            try
            {

                var existingContact = await _context.Contacts
                    .FirstOrDefaultAsync(c => c.ContactId == contact.ContactId && c.UserId == contact.UserId);

                if (existingContact == null)
                {
                    return false;
                }

                existingContact.Phone = contact.Phone;


                var result = await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating contact: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> AddAddress(Address address)
        {
            try
            {

                await _context.Addresses.AddAsync(address);


                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding address: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateAddress(Address address)
        {
            try
            {
                
                var existingAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == address.AddressId && a.UserId == address.UserId);

                if (existingAddress == null)
                {
                    return false;
                }

                
                existingAddress.Street = address.Street;
                existingAddress.City = address.City;
                existingAddress.State = address.State;
                existingAddress.PostalCode = address.PostalCode;
                existingAddress.Country = address.Country;


                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating address: {ex.Message}");
                return false;
            }
        }

        public async Task<User> GetMemberById(Guid id)
        {
           
            var res = await _context.Users
            .Include(ma => ma.Addresses).ThenInclude(a => a.AddressId)
            .Include(m => m.Contacts)
            .Include(m => m.Roles)
            .FirstOrDefaultAsync(m => m.UserId == id);
            return res;
        }
        public async Task<UserProfileDTO?> GetUserProfile(Guid userId)
        {

            var user = await _context.Users
                .Include(u => u.Contacts)
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return null;
            }

            
            return new UserProfileDTO
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                //Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Contacts = user.Contacts != null ? new ContactDTO
                {
                    Phone = user.Contacts.First().Phone,
                } : null,
                Addresses = user.Addresses?.Select(a => new AddressDTO
                {
                    Street = a.Street,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country
                }).ToList()
            };

        }

    }
}