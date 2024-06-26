using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VVA.ITS.WebApp.Data;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VVA.ITS.WebApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<AppUser>> GetUsersByEmail(string email)
        {
			//if (email.IsNullOrEmpty()) { return Enumerable.Empty<AppUser>(); }
            return await this.context.users
                                      .Where(p => p.Email.Contains(email))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersByFullName(string fullName)
        {
			//if (fullName.IsNullOrEmpty()) { return Enumerable.Empty<AppUser>(); }
			return await this.context.users
                          .Where(p => p.fullName.Contains(fullName))
                          .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersByName(string userName)
        {
			//if (userName.IsNullOrEmpty()) { return Enumerable.Empty<AppUser>(); }
			return await this.context.users
                          .Where(p => p.UserName.Contains(userName))
                          .ToListAsync();
        }

		public async Task<IEnumerable<AppUser>> GetUsersByBirthday(DateTime fromDate, DateTime toDate)
		{
			//if (fromDate == DateTime.MinValue && toDate == DateTime.MinValue) { return Enumerable.Empty<AppUser>(); }
			return await this.context.users
						  .Where(p => p.dateOfBirth >= fromDate && p.dateOfBirth <= toDate)
						  .ToListAsync();
		}

		public async Task<AppUser> GetUsersByBirthday(DateTime birthday)
		{
			if (birthday == DateTime.MinValue) { return null; }
			return await this.context.users.FirstOrDefaultAsync(p => p.dateOfBirth >= birthday && p.dateOfBirth <= birthday);
		}

		public async Task<IEnumerable<AppUser>> GetUsersByIdentityNumber(string identityNumber)
		{
			//if (identityNumber.IsNullOrEmpty()) { return Enumerable.Empty<AppUser>(); }
			return await this.context.users
						  .Where(p => p.identityNumber.Contains(identityNumber))
						  .ToListAsync();
		}

		public async Task<IEnumerable<AppUser>> GetUsersByPhoneNumber(string phoneNumber)
		{
			//if (phoneNumber.IsNullOrEmpty()) { return Enumerable.Empty<AppUser>(); }
			return await this.context.users
						  .Where(p => p.PhoneNumber.Contains(phoneNumber))
						  .ToListAsync();
		}

		public async Task<IEnumerable<AppUser>> GetUsersByAddress(string address)
		{
			//if (address.IsNullOrEmpty()) { return Enumerable.Empty<AppUser>(); }
			return await this.context.users
						  .Where(p => p.address.Contains(address))
						  .ToListAsync();
		}
	}
}
