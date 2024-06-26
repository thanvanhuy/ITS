using System.ComponentModel.DataAnnotations;

namespace VVA.ITS.WebApp.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Tên tài khoản hoặc địa chỉ email")]
        [Required(ErrorMessage = "Chưa nhập tài khoản")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public bool remember { get; set; } = false;
    }
}
