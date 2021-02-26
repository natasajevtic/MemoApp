using AutoMapper;
using MemoApp.Helper;
using MemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
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
                return Json(_mapper.Map<List<IdentityRole>, List<RoleViewModel>>(roles));
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

        [NoDirectAccess]
        [HttpGet]
        public async Task<IActionResult> EditUserRole(string id)
        {            
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var viewModelList = new List<EditUserRolesViewModel>();
                    foreach (var role in _roleManager.Roles)
                    {
                        var viewModel = new EditUserRolesViewModel
                        {
                            RoleId = role.Id,
                            RoleName = role.Name
                        };
                        if (await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            viewModel.IsSelected = true;
                        }
                        else
                        {
                            viewModel.IsSelected = false;
                        }
                        viewModelList.Add(viewModel);
                    }
                    return View(viewModelList);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(List<EditUserRolesViewModel> viewModel, string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var userRoles = await _userManager.GetRolesAsync(user);
                var resultOfRemovingRoles = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!resultOfRemovingRoles.Succeeded)
                {
                    foreach (var error in resultOfRemovingRoles.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(viewModel);
                }
                var resultOfAddingRoles = await _userManager.AddToRolesAsync(user, viewModel.Where(r => r.IsSelected == true).Select(r => r.RoleName));
                if (!resultOfAddingRoles.Succeeded)
                {
                    foreach (var error in resultOfAddingRoles.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(viewModel);
                }
                return Json(new { isValid = true, message = "The role is changed!" });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [NoDirectAccess]
        [HttpGet]
        public async Task<IActionResult> DetailsRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var viewModel = _mapper.Map<IdentityRole, RoleViewModel>(role);
                    foreach (var user in _userManager.Users)
                    {
                        if (await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            viewModel.Users.Add(user.UserName);
                        }
                    }
                    return View(viewModel);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var users = await _userManager.GetUsersInRoleAsync(role.Name);
                    if (users.Any())
                    {
                        return Json(new { success = true, message = "The role cannot be deleted. There are users in this role." });
                    }
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return Json(new { success = true, message = "The role is deleted!" });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [NoDirectAccess]
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    return View(_mapper.Map<IdentityRole, RoleViewModel>(role));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = await _roleManager.FindByIdAsync(viewModel.Id);
                    if (role != null)
                    {
                        role.Name = viewModel.Name;
                        var result = await _roleManager.UpdateAsync(role);
                        if (result.Succeeded)
                        {
                            return Json(new { isValid = true, message = "The role is updated!" });
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(viewModel);
                    }
                    return NotFound();
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [NoDirectAccess]
        [HttpGet]
        public IActionResult CreateRole()
        {
            var viewModel = new RoleViewModel();
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    var result = await _roleManager.CreateAsync(new IdentityRole(viewModel.Name));
                    if (result.Succeeded)
                    {
                        return Json(new { isValid = true, message = "The role is created!" });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View(viewModel);
                    }
                }
                return View(viewModel);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
