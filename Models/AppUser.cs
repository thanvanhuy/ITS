using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VVA.ITS.WebApp.Models
{
    public class AppUser: IdentityUser
    {
        public string? fullName { get; set; } // Họ và tên
        public bool? gender { get; set; } // Giới tính
        public DateTime? dateOfBirth { get; set; } // Ngày sinh
        public string? identityNumber { get; set; } // Số CCCD
        public string? address { get; set; } // Địa chỉ
        public string? profileURL { get; set; } // Hình ảnh chân dung
        public ICollection<IdentityRole>? roles { get; set; } // Một user có thể có nhiều phân quyền
        public bool isActive { get; set; } = false; // Kích hoạt tài khoản
    }
}
