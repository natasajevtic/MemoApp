using AutoMapper;
using MemoApp.Constants;
using MemoApp.Data;
using MemoApp.Services;
using MemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemoApp.Controllers
{
    [Authorize]
    public class MemoController : Controller
    {
        private readonly IMemoService _memoService;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public MemoController(IMemoService memoService, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _memoService = memoService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetMemos()
        {
            try
            {
                var memoModelList = new List<Memo>();
                //if the user is an admin, getting memos of all users
                if (User.IsInRole(Roles.AdminRole))
                {
                    memoModelList = _memoService.GetAllMemos().Value;
                }
                //if the user is not admin, getting only his memos
                else
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    memoModelList = _memoService.GetUserMemos(user.Id).Value;
                }
                return Json(_mapper.Map<List<Memo>, List<MemoViewModel>>(memoModelList));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public IActionResult Create()
        {
            var memoViewModel = new MemoViewModel();
            return View(memoViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MemoViewModel memoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var memoModel = _mapper.Map<MemoViewModel, Memo>(memoViewModel);

                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    memoModel.UserId = user.Id;

                    var memoId = _memoService.AddMemo(memoModel).Value;
                    if (memoId > 0)
                    {                        
                        return Json(new { isValid = true, message = "The memo is saved!" });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                return View(memoViewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return BadRequest();
                }

                var memoModel = new Memo();
                if (User.IsInRole(Roles.AdminRole))
                {
                    memoModel = _memoService.GetMemoById(id.Value).Value;
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    memoModel = _memoService.GetUserMemoById(user.Id, id.Value).Value;
                }
                if (memoModel != null)
                {
                    return View(_mapper.Map<Memo, MemoViewModel>(memoModel));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminRole)]
        public IActionResult Edit(long? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return BadRequest();
                }
                var memoModel = _memoService.GetMemoById(id.Value).Value;
                if (memoModel != null)
                {
                    var viewModel = _mapper.Map<Memo, MemoViewModel>(memoModel);
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
        [Authorize(Roles = Roles.AdminRole)]
        public IActionResult Edit(MemoViewModel memoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var memoModel = _mapper.Map<MemoViewModel, Memo>(memoViewModel);
                    var updatedModel = _memoService.UpdateMemo(memoModel);
                    if (updatedModel.Succeeded)
                    {                        
                        return Json(new { isValid = true, message = "The memo is updated!" });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                return View(memoViewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }        

        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public IActionResult Delete(long id)
        {
            try
            {
                var isDeleted = _memoService.DeleteMemo(id);
                if (isDeleted.Value == true)
                {
                    return Json(new { success = true, message = "The memo is deleted." });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }
}
