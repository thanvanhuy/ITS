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
using System.ComponentModel.DataAnnotations;
using VVA.ITS.WebApp.Data;

namespace VVA.ITS.WebApp.Controllers
{
    public class RoleController : Controller
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
		public RoleController(UserManager<AppUser> userManager,
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
			RoleViewModel model = new RoleViewModel
            {
                roles = await PaginatedList<IdentityRole>.CreateAsync(this.roleManager.Roles.AsNoTracking(), pageNumber ?? 1, pageSize),
                userManager = this.userManager
            };
			return View(model);
		}


		private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([Required] string roleName)
        {
            IdentityRole role = await this.roleManager.FindByNameAsync(roleName);
            // Phân quyền đã tồn tại => không được tạo
            if (role != null)
            {
                ModelState.AddModelError("", "Phân quyền đã tồn tại, vui lòng nhập tên khác");
            } else
            {
                IdentityResult result = await this.roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded) return RedirectToAction("Index", "Role", new { ToastrMessage = "Tạo thành công phân quyền " + roleName, type = ToastrMessageType.Success, pageNumber = 1 });
				//return await Index("Phân quyền " + roleName + " đã được tạo", 1);
				else Errors(result);
            }

            return View("Index", this.roleManager.Roles);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            IdentityRole role = await this.roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                string roleName = role.Name;
                // Remove all users from role
                IEnumerable<AppUser> users = await this.userManager.Users.ToListAsync();
                foreach (AppUser user in users)
                {
					if (await this.userManager.IsInRoleAsync(user, role.Name))
						await this.userManager.RemoveFromRoleAsync(user, role.Name);
				}

				// Delete role
				IdentityResult result = await this.roleManager.DeleteAsync(role);
                if (result.Succeeded) return RedirectToAction("Index", "Role", new { ToastrMessage = "Xóa thành công phân quyền " + roleName, type = ToastrMessageType.Success, pageNumber = 1 });
				//return RedirectToAction("Index");
				else Errors(result);
            }
            else ModelState.AddModelError("", "Không tìm thấy phân quyền");
            return View("Index", this.roleManager.Roles);
        }

        public async Task<IActionResult> Update(string roleID)
        {
            IdentityRole role = await this.roleManager.FindByIdAsync(roleID);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            foreach (AppUser user in this.userManager.Users)
            {                
                var list = await this.userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            EditRoleViewModel model = new EditRoleViewModel
            {
                role = role,
                members = members,
                nonMembers = nonMembers
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Xóa tài khoản không còn thuộc phân quyền (Xóa trước khi thêm)
                foreach (AppUser user in await this.userManager.Users.ToListAsync())
                {
                    var check = model.deleteIDs?.FirstOrDefault(x => x.Equals(user.Id));
                    if (check == null && await this.userManager.IsInRoleAsync(user, model.roleName))
                    {
                        IdentityResult result = await this.userManager.RemoveFromRoleAsync(user, model.roleName);
                        if (!result.Succeeded) Errors(result);
                    }
                }

                // Cách cũ: cho người dùng check vào list để tạo danh sách xóa
                //foreach (string userID in model.deleteIDs ?? new string[] { })
                //{
                //    AppUser user = await this.userManager.FindByIdAsync(userID);
                //    if (user != null)
                //    {
                //        IdentityResult result = await this.userManager.RemoveFromRoleAsync(user, model.roleName);
                //        if (!result.Succeeded) Errors(result);
                //    }
                //}

                // Thêm mới tài khoản vào phân quyền
                // Trong trường hợp string[] addIDs là rỗng ==> tạo một string[] rỗng mới
                foreach (string userID in model.addIDs ?? new string[] { })
                {
                    AppUser user = await this.userManager.FindByIdAsync(userID);
                    if (user != null)
                    {
                        IdentityResult result = await this.userManager.AddToRoleAsync(user, model.roleName);
                        if (!result.Succeeded) Errors(result);
                    }
                }

                return RedirectToAction("Index", "Role", new { ToastrMessage = "Cập nhật thành công phân quyền " + model.roleName, type = ToastrMessageType.Success, pageNumber = 1 });
				//RedirectToAction("Index", "Role");
			}
            else return await Update(model.roleID);
        }

        [HttpPost]
        public async Task<JsonResult> SearchRoles(IFormCollection formCollection)
        {
            JsonResponseViewModel viewModel = new JsonResponseViewModel();
            try
            {
                string roleInfo = formCollection["roleInfo"].ToString();
                List<IdentityRole> roles = new List<IdentityRole>();
                IdentityRole role = await this.roleManager.FindByNameAsync(roleInfo);
                if (role != null) roles.Add(role);
                else
                {
                    role = await this.roleManager.FindByIdAsync(roleInfo);
                    if (role != null) roles.Add(role);
                }

                if (role == null)
                {
                    IEnumerable<IdentityRole> role1 = await this.userRoleRepository.GetRolesByName(roleInfo);
                    IEnumerable<IdentityRole> role2 = await this.userRoleRepository.GetRolesById(roleInfo);
                    roles = role1.Union(role2).ToList();
                }
                if (roles != null)
                {
                    string htmlTable = string.Empty;
                    if (roles.Count == 0) htmlTable += "<tr><td colspan=\"4\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
                    else
                    {
                        int count = 0;
                        foreach (IdentityRole _role in roles)
                        {
                            htmlTable += "<tr>";                            
                            htmlTable += "<td>" + (++count) + ".</td>";
                            htmlTable += "<input type=\"hidden\" value=\"@role.Id\"/>";
                            htmlTable += "<td class=\"role-name\">" + _role.Name + "</td>";
                            bool havingUsers = false;
                            string ulUsers = "<nav><ul class=\"nav nav-pills nav-sidebar flex-column\">";
                            foreach (AppUser user in this.userManager.Users)
                            {
                                if (await this.userManager.IsInRoleAsync(user, _role.Name))
                                {
                                    ulUsers += "<li class=\"nav-item\">";
                                    //ulUsers += "<a class=\"nav-link\" asp-controller=\"Account\" asp-action=\"Update\" asp-route-userID=\""+user.Id+"\">";
                                    ulUsers += "<a class=\"nav-link\" href=\"\\Account\\Update?userID=" + user.Id + "\">";
                                    //Ex: https://localhost:7104/Account/Update?userID=2bcb30c7-916b-439c-8d12-737e802d9017
                                    ulUsers += "<i class=\"nav-icon fas fa-user-alt\"></i>";
                                    ulUsers += "<p>" + user.fullName + " - " + user.UserName + "</p>";
                                    ulUsers += "</a>";
                                    ulUsers += "</li>";
                                    havingUsers = true;
                                }
                            }
                            ulUsers += "</ul></nav>";
                            if (!havingUsers)
                            {
                                ulUsers = "<nav><ul class=\"nav nav-pills nav-sidebar flex-column\">";
                                ulUsers += "<li class=\"nav-item\">";
                                ulUsers += "<a href=\"#\" class=\"nav-link\">";
                                ulUsers += "<i class=\"nav-icon fas fa-user-alt\"></i>";
                                ulUsers += "<p>Chưa có</p>";
                                ulUsers += "</a>";
                                ulUsers += "</li>";
                                ulUsers += "</ul></nav>";
                            }

                            htmlTable += "<td i-role=\""+_role.Id + "\">"+ulUsers+"</td>";
                            htmlTable += "<td style=\"text-align:center\">";
                            htmlTable += "<div class=\"btn-group btn-group-sm\">";
                            htmlTable += "<a class=\"btn btn-sm btn-primary btn-edit-role\" title=\"Điều chỉnh thông tin phân quyền\"><i class=\"far fa-edit\"></i></a>";
                            htmlTable += "<a class=\"btn btn-sm btn-danger btn-delete-role\" title=\"Xóa phân quyền\"><i class=\"far fa-trash-alt\"></i></a>";
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
                    viewModel.responseMessage = "<tr><td colspan=\"4\" style=\"text-align:center\">Không có dữ liệu trong hệ thống</td></tr>";
                }
            }
            catch (Exception Ex)
            {
                if (this.logger == null) this.logger = new Logger();
                logger.Write("[HttpPost] RoleController.SearchRoles()", Ex.Message, Logger.LogType.ERROR);
            }
            return Json(viewModel);
        }

    }
}
