using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using VVA.ITS.WebApp.Interfaces;
using Microsoft.IdentityModel.Tokens;
using AppUtilObjectCore;
using System.Net;
using VVA.ITS.WebApp.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VVA.ITS.WebApp.Data;

//Reference: https://www.yogihosting.com/aspnet-core-identity-setup/
//Core Identity Role: https://www.yogihosting.com/aspnet-core-identity-roles/

namespace VVA.ITS.WebApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly SignInManager<AppUser> signInManager;
		private readonly IIdentityRoleRepository userRoleRepository;
		private readonly IUserRepository userRepository;
		private readonly IWebHostEnvironment webHostEnvironment;
		private readonly IPasswordHasher<AppUser> passwordHasher;
		private int pageSize;
		private Logger logger;

		public AccountController(UserManager<AppUser> userManager,
												RoleManager<IdentityRole> roleManager,
												SignInManager<AppUser> signInManager,
												IUserRepository userRepository,
												IIdentityRoleRepository userRoleRepository,
												IWebHostEnvironment webHostEnvironment,
												IPasswordHasher<AppUser> passwordHasher)
		{
			this.signInManager = signInManager;
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.userRepository = userRepository;
			this.userRoleRepository = userRoleRepository;
			this.webHostEnvironment = webHostEnvironment;
			this.passwordHasher = passwordHasher;
			this.pageSize = 15;
		}

		[AllowAnonymous]
		public IActionResult Login(string returnUrl)
		{
			var loginVM = new LoginViewModel();
			//loginVM.returnUrl = returnUrl;
			return View(loginVM);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (!ModelState.IsValid) return View(loginVM);
			AppUser user = await this.userManager.FindByNameAsync(loginVM.userName);
			if (user != null)
			{
				// User is found, check password
				var passwordCheck = await this.userManager.CheckPasswordAsync(user, loginVM.password);
				if (passwordCheck)
				{
					// Password is correct, sign out the previous session then sign in
					await this.signInManager.SignOutAsync();
					var result = await this.signInManager.PasswordSignInAsync(user, loginVM.password, false, false);
					if (result.Succeeded) return RedirectToAction("Thongke", "Dashboard"); //return Redirect(loginVM.returnUrl ?? "/");
				}
				// Password is incorrect
				TempData["Error"] = "Thông tin đăng nhập không chính xác. Vui lòng thử lại!";
				return View(loginVM);
			}
			// User not found
			TempData["Error"] = "Thông tin đăng nhập không chính xác. Vui lòng thử lại!";
			return View(loginVM);
		}

		public async Task<IActionResult> Index(string? ToastrMessage, ToastrMessageType type, int? pageNumber)
		{
			ViewData["ToastrMessage"] = (ToastrMessage.IsNullOrEmpty() ? string.Empty : ToastrMessage);
			switch (type)
			{
				case ToastrMessageType.Success: ViewData["ToastrMessageType"] = "success"; break;
				case ToastrMessageType.Error: ViewData["ToastrMessageType"] = "error"; break;
				case ToastrMessageType.Warning: ViewData["ToastrMessageType"] = "warning"; break;
				case ToastrMessageType.Info: ViewData["ToastrMessageType"] = "info"; break;
			}
			return View(await PaginatedList<AppUser>.CreateAsync(this.userManager.Users.AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		//      [HttpPost]
		//public async Task<JsonResult> Index(IFormCollection formCollection)
		//{
		//          JsonResponseViewModel viewModel = new JsonResponseViewModel();
		//          try
		//          {
		//              int pageNumber = AppUtilObjectCore.Utility.getInt(formCollection["pageNumber"]);
		//              PaginatedList<AppUser> users = await PaginatedList<AppUser>.CreateAsync(this.userManager.Users.AsNoTracking(), pageNumber, pageSize);
		//		if (users != null)
		//              {
		//			string htmlTable = string.Empty;
		//			if (users.Count == 0) htmlTable += "<tr><td colspan=\"6\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
		//                  else
		//                  {
		//				int count = 0;
		//				foreach (AppUser usr in users)
		//				{
		//					htmlTable += "<tr style=\"cursor:grab\">";
		//					htmlTable += "<td>" + (++count) + ".</td>";
		//					htmlTable += "<td>" + usr.fullName + "</td>";
		//					htmlTable += "<td>" + usr.UserName + "</td>";
		//					htmlTable += "<td>" + usr.Email + "</td>";
		//					htmlTable += "<td style=\"text-align:center\">";
		//					htmlTable += "<div class=\"custom-control custom-checkbox\">";

		//					if (usr.isActive) htmlTable += "<input id = \"" + usr.Id + "\" class=\"custom-control-input activate-user-control\" type=\"checkbox\" value=\"true\" checked>";
		//					else htmlTable += "<input id = \"" + usr.Id + "\" class=\"custom-control-input activate-user-control\" type=\"checkbox\" value=\"false\">";

		//					htmlTable += "<label for= \"" + usr.Id + "\" class= \"custom-control-label\" > Đã kích hoạt</label>";
		//					htmlTable += "</div>";
		//					htmlTable += "</td>";

		//					htmlTable += "<td style=\"text-align:center\">";
		//					htmlTable += "<div class=\"btn-group\">";
		//					htmlTable += "<a class=\"btn btn-sm btn-primary\" asp-action=\"Update\" asp-route-userID=\"" + usr.Id + "\" title=\"Chỉnh sửa thông tin người dùng\"><i class=\"fas fa-user-edit\"></i></a>";
		//					htmlTable += "<form asp-action=\"Delete\" asp-controller=\"Account\" asp-route-userID=\"" + usr.Id + "\" method=\"post\">";
		//					htmlTable += "<button type=\"submit\" class=\"btn btn-sm btn-danger\" title=\"Xóa người dùng\"><i class=\"fas fa-user-alt-slash\"></i></button>";
		//					htmlTable += "</form>";
		//					htmlTable += "</div>";
		//					htmlTable += "</td>";
		//					htmlTable += "</tr>";
		//				}
		//			}

		//			viewModel.responseCode = 0;
		//                  PaginationViewModel paginationViewModel = new PaginationViewModel
		//                  {
		//                      htmlTable = htmlTable,
		//                      preDisabled = !users.hasPreviousPage ? "disabled" : "",
		//                      nextDisabled = !users.hasNextPage ? "disabled" : "",
		//                      preNumber = (users.pageIndex - 1),
		//                      nextNumber = (users.pageIndex + 1)
		//			};
		//                  viewModel.responseMessage = JsonConvert.SerializeObject(paginationViewModel);
		//		}
		//		else
		//		{
		//			viewModel.responseCode = 1;
		//			viewModel.responseMessage = "<tr><td colspan=\"6\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
		//		}
		//	} catch (Exception Ex)
		//          {
		//		if (this.logger == null) this.logger = new Logger();
		//		logger.Write("[HttpPost] AccountController.Index()", Ex.Message, Logger.LogType.ERROR);
		//	}
		//	return Json(viewModel);
		//}


		[AllowAnonymous]
		public async Task<IActionResult> Register()
		{
			var response = new UserViewModel();
			response.roles = await this.userRoleRepository.GetAllRoles();
			return View(response);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(UserViewModel model)
		{
			// Check validation
			if (string.IsNullOrEmpty(model.userName))
				ModelState.AddModelError(nameof(model.userName), "Chưa nhập tên tài khoản");

			if (string.IsNullOrEmpty(model.password))
				ModelState.AddModelError(nameof(model.password), "Chưa nhập mật khẩu");

			if (string.IsNullOrEmpty(model.confirmPassword))
				ModelState.AddModelError(nameof(model.confirmPassword), "Chưa xác nhận mật khẩu");
			else if (!model.confirmPassword.Equals(model.password))
				ModelState.AddModelError(nameof(model.confirmPassword), "Mật khẩu không chính xác");

			if (string.IsNullOrEmpty(model.fullName))
				ModelState.AddModelError(nameof(model.fullName), "Vui lòng nhập đầy đủ họ và tên");

			if (model.roleIDs.IsNullOrEmpty())
				if (model.roleIDs.IsNullOrEmpty())
					ModelState.AddModelError(nameof(model.roleIDs), "Vui lòng chọn vai trò của người dùng");

			if (!ModelState.IsValid) return View(model);

			AppUser user = await this.userManager.FindByNameAsync(model.userName);
			if (user != null)
			{
				//TempData["Error"] = "Tài khoản này đã tồn tại";
				ModelState.AddModelError(nameof(model.userName), "Tài khoản này đã tồn tại. Vui lòng chọn tên đăng nhập khác!");
                model.roles = await this.userRoleRepository.GetAllRoles();
                return View(model);
			}

			AppUser newUser = new AppUser()
			{
				fullName = model.fullName,
				gender = model.gender,
				dateOfBirth = model.dateOfBirth,
				identityNumber = model.identityNumber,
				address = model.address,
				PhoneNumber = model.phoneNumber,
				UserName = model.userName,
				Email = model.email,
				isActive = false
			};

			// Lưu lại hình profile
			if (model.profileImage != null)
			{
				string filePath = string.Empty;
				string uniqueFileName = UploadFile(model, out filePath);
				newUser.profileURL = uniqueFileName;
			}

			var newUserResponse = await this.userManager.CreateAsync(newUser, model.password);

			if (newUserResponse.Succeeded)
			{
				foreach (string roleID in model.roleIDs)
				{
					IdentityRole role = await this.userRoleRepository.GetRoleByID(roleID);
					await this.userManager.AddToRoleAsync(newUser, role.Name);
				}
				//return await Index("Tạo thành công tài khoản " + newUser.UserName, ToastrMessageType.Success, 1);
				//return RedirectToAction("Index", "Account");
				return RedirectToAction("Index", "Account", new { ToastrMessage = "Tạo thành công tài khoản " + newUser.UserName, type = ToastrMessageType.Success, pageNumber = 1 });
			}
			else
			{
				foreach (IdentityError error in newUserResponse.Errors)
					ModelState.AddModelError("", error.Description);
                model.roles = await this.userRoleRepository.GetAllRoles(); 
				return View(model);
			}
		}

		public async Task<IActionResult> Logout()
		{
			await this.signInManager.SignOutAsync();
			return RedirectToAction("Login", "Account");
		}

		[AllowAnonymous]
		private string UploadFile(UserViewModel viewModel, out string filePath)
		{
			string uniqueFileName = string.Empty;
			filePath = string.Empty;

			if (viewModel.profileImage != null)
			{
				string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "ProfileImages");
				if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
				uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.profileImage.FileName;
				filePath = Path.Combine(uploadFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					viewModel.profileImage.CopyTo(fileStream);
				}
			}
			return uniqueFileName;
		}

		[HttpGet]
		// Cập nhật thông tin người dùng
		public async Task<IActionResult> Update(string userID)
		{
			AppUser user = await this.userManager.FindByIdAsync(userID);
			if (user != null)
			{
				IEnumerable<IdentityRole> roles = this.roleManager.Roles;
				UserViewModel model = new UserViewModel();
				model.GetData(user, roles);
				List<string> roleIDs = new List<string>();
				foreach (IdentityRole role in roles)
					if (await this.userManager.IsInRoleAsync(user, role.Name))
					{
						roleIDs.Add(role.Id);
						break;
					}
				model.roleIDs = roleIDs.ToArray();
				return View(model);
			}
			//else return await Index("Không tìm thấy tài khoản người dùng trong hệ thống", ToastrMessageType.Error,1);
            else return RedirectToAction("Index", "Account", new { ToastrMessage = "Không tìm thấy tài khoản người dùng trong hệ thống", type = ToastrMessageType.Error, pageNumber = 1});
        }

		[HttpPost]
		// Cập nhật thông tin người dùng
		public async Task<IActionResult> Update(UserViewModel model)
		{
			// Check validation
			if (string.IsNullOrEmpty(model.password))
				ModelState.AddModelError(nameof(model.password), "Chưa nhập mật khẩu");

			if (string.IsNullOrEmpty(model.confirmPassword))
				ModelState.AddModelError(nameof(model.confirmPassword), "Chưa xác nhận mật khẩu");
			else if (!model.confirmPassword.Equals(model.password))
				ModelState.AddModelError(nameof(model.confirmPassword), "Mật khẩu không chính xác");

			if (string.IsNullOrEmpty(model.fullName))
				ModelState.AddModelError(nameof(model.fullName), "Vui lòng nhập đầy đủ họ và tên");

			if (model.roleIDs.IsNullOrEmpty())
				ModelState.AddModelError(nameof(model.roleIDs), "Vui lòng chọn vai trò của người dùng");

			if (!ModelState.IsValid) return View(model);

			AppUser user = await this.userManager.FindByIdAsync(model.userID);
			if (user != null)
			{
				user.Email = model.email;
				if (!user.PasswordHash.Equals(model.password)) user.PasswordHash = this.passwordHasher.HashPassword(user, model.password);
				user.fullName = model.fullName;
				user.gender = model.gender;
				user.dateOfBirth = model.dateOfBirth;
				user.identityNumber = model.identityNumber;
				user.address = model.address;
				user.PhoneNumber = model.phoneNumber;

				// Lưu lại hình profile mới
				if (model.profileImage != null)
				{
					string filePath = string.Empty;
					string uniqueFileName = UploadFile(model, out filePath);
					user.profileURL = uniqueFileName;
				}

				// Xóa người dùng khỏi các roles cũ
				IList<string> roleNames = await this.userManager.GetRolesAsync(user);
				await this.userManager.RemoveFromRolesAsync(user, roleNames);

				// Thêm người dùng vào role mới
				foreach (string roleID in model.roleIDs)
				{
					IdentityRole role = await this.roleManager.FindByIdAsync(roleID);
					await this.userManager.AddToRoleAsync(user, role.Name);
				}

				user.isActive = model.isActive;
				IdentityResult result = await this.userManager.UpdateAsync(user);
				if (result.Succeeded) return RedirectToAction("Index", "Account", new { ToastrMessage = "Cập nhật tài khoản " + user.UserName + " thành công", type = ToastrMessageType.Success, pageNumber = 1 });
					//return RedirectToAction("Index", "Account");
					//return await Index("Tài khoản " + user.UserName + " đã được cập nhật", ToastrMessageType.Success, 1);
				
				else Errors(result);
			}
			else ModelState.AddModelError("", "Không tìm thấy tài khoản " + user.UserName);
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string userID)
		{
			AppUser user = await this.userManager.FindByIdAsync(userID);
			if (user != null)
			{
				IList<string> roleNames = await this.userManager.GetRolesAsync(user);
				IdentityResult result = await this.userManager.RemoveFromRolesAsync(user, roleNames);

				// Delete profile image (if exist)
				string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "ProfileImages");
				if (Directory.Exists(uploadFolder) && !user.profileURL.IsNullOrEmpty())
				{
					string filePath = Path.Combine(uploadFolder, user.profileURL);
					if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
				}

				if (result.Succeeded)
				{
					result = await this.userManager.DeleteAsync(user);
					if (result.Succeeded) return RedirectToAction("Index", "Account", new { ToastrMessage = "Xóa thành công tài khoản " + user.UserName, type = ToastrMessageType.Success, pageNumber = 1 });
					// return RedirectToAction("Index", "Account");
					//return await Index("Xóa thành công tài khoản " + user.UserName, ToastrMessageType.Success, 1);					
					else Errors(result);
				}
				else Errors(result);
			}
			else ModelState.AddModelError("", "Không tìm thấy tài khoản của người dùng");
			return View("Index", this.userManager.Users);
			//return await Index("Xóa bị lỗi! Không tìm thấy tài khoản người dùng trong hệ thống", ToastrMessageType.Error, 1);		
		}

		[HttpPost]
		public async Task<string> Activate(IFormCollection formCollection)
		{
			string userID = formCollection["userID"].ToString();
			string action = formCollection["action"].ToString();
			AppUser user = await this.userManager.FindByIdAsync(userID);
			if (user != null)
			{
				user.isActive = (action.Equals("activate") ? true : false);
				IdentityResult result = await this.userManager.UpdateAsync(user);
				if (result.Succeeded) return (user.isActive == true ? "activated" : "deactivated");
				else return "unchanged";
			}
			return "unknown";
		}

		[HttpPost]
		public async Task<JsonResult> SearchUsers(IFormCollection formCollection)
		{
			JsonResponseViewModel viewModel = new JsonResponseViewModel();
			try
			{
				string userInfo = formCollection["userInfo"].ToString();
				List<AppUser> users = new List<AppUser>();
				AppUser user = await this.userManager.FindByNameAsync(userInfo);
				if (user != null) users.Add(user);
				else
				{
					user = await this.userManager.FindByEmailAsync(userInfo);
					if (user != null) users.Add(user);
				}

				if (user == null)
				{
					IEnumerable<AppUser> user3 = await this.userRepository.GetUsersByName(userInfo);
					IEnumerable<AppUser> user4 = await this.userRepository.GetUsersByFullName(userInfo);
					IEnumerable<AppUser> user5 = await this.userRepository.GetUsersByEmail(userInfo);
					users = user3.Union(user4).Union(user5).ToList();
				}
				if (users != null)
				{
					string htmlTable = string.Empty;
					if (users.Count == 0) htmlTable += "<tr><td colspan=\"6\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
					else
					{
						int count = 0;
						foreach (AppUser usr in users)
						{
							htmlTable += "<tr style=\"cursor:grab\">";
							htmlTable += "<td>" + (++count) + ".</td>";
							htmlTable += "<td>" + usr.fullName + "</td>";
							htmlTable += "<td>" + usr.UserName + "</td>";
							htmlTable += "<td>" + usr.Email + "</td>";
							htmlTable += "<td style=\"text-align:center\">";
							htmlTable += "<div class=\"custom-control custom-checkbox\">";

							if (usr.isActive) htmlTable += "<input id = \"" + usr.Id + "\" class=\"custom-control-input activate-user-control\" type=\"checkbox\" value=\"true\" checked>";
							else htmlTable += "<input id = \"" + usr.Id + "\" class=\"custom-control-input activate-user-control\" type=\"checkbox\" value=\"false\">";

							htmlTable += "<label for= \"" + usr.Id + "\" class= \"custom-control-label\" > Đã kích hoạt</label>";
							htmlTable += "</div>";
							htmlTable += "</td>";

							htmlTable += "<td style=\"text-align:center\">";
							htmlTable += "<div class=\"btn-group\">";
							htmlTable += "<a class=\"btn btn-sm btn-primary\" asp-action=\"Update\" asp-route-userID=\"" + usr.Id + "\" title=\"Chỉnh sửa thông tin người dùng\"><i class=\"fas fa-user-edit\"></i></a>";
							htmlTable += "<form asp-action=\"Delete\" asp-controller=\"Account\" asp-route-userID=\"" + usr.Id + "\" method=\"post\">";
							htmlTable += "<button type=\"submit\" class=\"btn btn-sm btn-danger\" title=\"Xóa người dùng\"><i class=\"fas fa-user-alt-slash\"></i></button>";
							htmlTable += "</form>";
							htmlTable += "</div>";
							htmlTable += "</td>";
							htmlTable += "</tr>";
						}
					}
					viewModel.responseCode = 0;
					viewModel.responseMessage = htmlTable;
				}
				else
				{
					viewModel.responseCode = 1;
					viewModel.responseMessage = "<tr><td colspan=\"6\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
				}
			}
			catch (Exception Ex)
			{
				if (this.logger == null) this.logger = new Logger();
				logger.Write("[HttpPost] AccountController.SearchUsers()", Ex.Message, Logger.LogType.ERROR);
			}
			return Json(viewModel);
		}


		[HttpPost]
		public async Task<JsonResult> AdvancedSearchUsers(IFormCollection formCollection)
		{
			JsonResponseViewModel viewModel = new JsonResponseViewModel();
			try
			{
				DateTime fromBirthday = DateTime.Parse(formCollection["fromBirthday"]);
				DateTime toBirthday = DateTime.Parse(formCollection["toBirthday"]);
				string identityNumber = formCollection["identityNumber"].ToString();
				string phoneNumber = formCollection["phoneNumber"].ToString();
				string address = formCollection["address"].ToString();

				IEnumerable<AppUser> list1 = await this.userRepository.GetUsersByBirthday(fromBirthday, toBirthday);
				IEnumerable<AppUser> list2 = await this.userRepository.GetUsersByIdentityNumber(identityNumber);
				IEnumerable<AppUser> list3 = await this.userRepository.GetUsersByPhoneNumber(phoneNumber);
				IEnumerable<AppUser> list4 = await this.userRepository.GetUsersByAddress(address);
				List<AppUser> users = list1.Union(list2).Union(list3).Union(list4).ToList();

				if (users != null)
				{
					string htmlTable = string.Empty;
					if (users.Count == 0) htmlTable += "<tr><td colspan=\"6\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
					else
					{
						int count = 0;
						foreach (AppUser usr in users)
						{
							htmlTable += "<tr style=\"cursor:grab\">";
							htmlTable += "<td>" + (++count) + ".</td>";
							htmlTable += "<td>" + usr.fullName + "</td>";
							htmlTable += "<td>" + usr.UserName + "</td>";
							htmlTable += "<td>" + usr.Email + "</td>";
							htmlTable += "<td style=\"text-align:center\">";
							htmlTable += "<div class=\"custom-control custom-checkbox\">";

							if (usr.isActive) htmlTable += "<input id = \"" + usr.Id + "\" class=\"custom-control-input activate-user-control\" type=\"checkbox\" value=\"true\" checked>";
							else htmlTable += "<input id = \"" + usr.Id + "\" class=\"custom-control-input activate-user-control\" type=\"checkbox\" value=\"false\">";

							htmlTable += "<label for= \"" + usr.Id + "\" class= \"custom-control-label\" > Đã kích hoạt</label>";
							htmlTable += "</div>";
							htmlTable += "</td>";

							htmlTable += "<td style=\"text-align:center\">";
							htmlTable += "<div class=\"btn-group\">";
							htmlTable += "<a class=\"btn btn-sm btn-primary\" asp-action=\"Update\" asp-route-userID=\"" + usr.Id + "\" title=\"Chỉnh sửa thông tin người dùng\"><i class=\"fas fa-user-edit\"></i></a>";
							htmlTable += "<form asp-action=\"Delete\" asp-controller=\"Account\" asp-route-userID=\"" + usr.Id + "\" method=\"post\">";
							htmlTable += "<button type=\"submit\" class=\"btn btn-sm btn-danger\" title=\"Xóa người dùng\"><i class=\"fas fa-user-alt-slash\"></i></button>";
							htmlTable += "</form>";
							htmlTable += "</div>";
							htmlTable += "</td>";
							htmlTable += "</tr>";
						}
					}
					viewModel.responseCode = 0;
					viewModel.responseMessage = htmlTable;
				}
				else
				{
					viewModel.responseCode = 1;
					viewModel.responseMessage = "<tr><td colspan=\"6\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
				}
			}
			catch (Exception Ex)
			{
				if (this.logger == null) this.logger = new Logger();
				logger.Write("[HttpPost] AccountController.AdvancedSearchUsers()", Ex.Message, Logger.LogType.ERROR);
			}
			return Json(viewModel);
		}

		private void Errors(IdentityResult result)
		{
			foreach (IdentityError error in result.Errors)
				ModelState.AddModelError("", error.Description);
		}
	}
}
