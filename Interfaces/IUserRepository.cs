using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.Interfaces
{
    public interface IUserRepository
    {
        //Nhập tên người dùng/tên tài khoản/địa chỉ email
        Task<IEnumerable<AppUser>> GetUsersByFullName(string fullName);
        Task<IEnumerable<AppUser>> GetUsersByName(string userName);
        Task<IEnumerable<AppUser>> GetUsersByEmail(string email);
		Task<IEnumerable<AppUser>> GetUsersByBirthday(DateTime fromDate, DateTime toDate);
		Task<AppUser> GetUsersByBirthday(DateTime birthday);
		Task<IEnumerable<AppUser>> GetUsersByIdentityNumber(string identityNumber);
		Task<IEnumerable<AppUser>> GetUsersByPhoneNumber(string phoneNumber);
		Task<IEnumerable<AppUser>> GetUsersByAddress(string address);
	}
}
