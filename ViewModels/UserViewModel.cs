using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using VVA.ITS.WebApp.Models;

namespace VVA.ITS.WebApp.ViewModels
{
    public class UserViewModel
    {
		[Display(Name = "Mã tài khoản")]
		public string? userID { get; set; }

		[Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string userName { get; set; }

        [Display(Name ="E-mail đăng nhập")]
		[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail không hợp lệ")]
		[Required(ErrorMessage = "Vui lòng nhập địa chỉ E-mail")]
        public string email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string confirmPassword { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập đầy đủ họ và tên")]
        public string? fullName { get; set; } // Họ và tên

        [Display(Name = "Giới tính")]
        public bool? gender { get; set; } // Giới tính

        [Display(Name = "Ngày sinh")]
        public DateTime? dateOfBirth { get; set; } // Ngày sinh

        [Display(Name = "Số CMND/CCCD")]
        public string? identityNumber { get; set; } // Số CCCD

        [Display(Name = "Địa chỉ")]
        public string? address { get; set; } // Địa chỉ

        [Display(Name = "Số điện thoại")]
        public string? phoneNumber { get; set; }

        [Display(Name = "Hình đại diện")]
        public string? profileURL { get; set; } // Hình đại diện

        [Display(Name = "Vai trò")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng chọn vai trò của người dùng")]
		public string[] roleIDs { get; set; }

        public bool isActive { get; set; } = false;

        [Display(Name = "Ảnh đại diện")]
        public IFormFile? profileImage { get; set; }

        public IEnumerable<IdentityRole>? roles { get; set; } // Danh sách các vai trò của người dùng

        public void GetData(AppUser user)
        {
            this.userID = user.Id;
            this.userName = user.UserName;
			this.email = user.Email;
            this.password = user.PasswordHash;
            this.confirmPassword = user.PasswordHash;
            this.fullName = user.fullName;
            this.gender = user.gender;
            this.dateOfBirth = user.dateOfBirth;
            this.identityNumber = user.identityNumber;
            this.address = user.address;
            this.phoneNumber = user.PhoneNumber;
            this.profileURL = user.profileURL;
            this.profileImage = null;
            this.isActive = user.isActive;
		}

        public void GetData(AppUser user, IEnumerable<IdentityRole> roles)
        {
            this.roles = roles;
            this.GetData(user);
        }
	}
}
