using MemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Roles()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                return Json(roles);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {                
                var users = await _userManager.Users.ToListAsync();
                var userRolesViewModel = new List<UserRolesViewModel>();
                foreach (var user in users)
                {
                    var viewModel = new UserRolesViewModel
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Username = user.UserName,
                        Roles = await _userManager.GetRolesAsync(user)
                    };
                    userRolesViewModel.Add(viewModel);
                }
                return Json(userRolesViewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
