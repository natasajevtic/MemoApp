using AutoMapper;
using MemoApp.Common.Message;
using MemoApp.Data;
using MemoApp.Helper;
using MemoApp.Services;
using MemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<MemoController> _localizer;
        private readonly ISettingsService _settingsService;

        public MemoController(IMemoService memoService, IMapper mapper, UserManager<IdentityUser> userManager, IStringLocalizer<MemoController> localizer, ISettingsService settingsService)
        {
            _memoService = memoService;
            _mapper = mapper;
            _userManager = userManager;
            _localizer = localizer;
            _settingsService = settingsService;
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
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                //if the user is an admin, getting memos of all users
                if (User.IsInRole("Admin"))
                {
                    memoModelList = _memoService.GetAllMemos().Value;
                }
                //if the user is not admin, getting only his memos
                else
                {                    
                    memoModelList = _memoService.GetUserMemos(user.Id).Value;
                }
                var memoViewModelList = _mapper.Map<List<Memo>, List<MemoViewModel>>(memoModelList);
                foreach (var memo in memoViewModelList)
                {
                   memo.CreatedAt = _settingsService.ConvertUTCtoLocalDateTimeString(DateTime.Parse(memo.CreatedAt), user.Id);
                   memo.UpdatedAt = memo.CreatedAt;
                }
                return Json(memoViewModelList);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
            }
        }

        [NoDirectAccess]
        [HttpGet]
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
                        return Json(new { isValid = true, message = _localizer.GetString(Message.AddedSuccessfully) });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
                }
                return View(memoViewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
            }
        }

        [NoDirectAccess]
        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return BadRequest(_localizer.GetString(Message.BadRequest));
                }

                var memoModel = new Memo();
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (User.IsInRole("Admin"))
                {
                    memoModel = _memoService.GetMemoById(id.Value).Value;
                }
                else
                {                    
                    memoModel = _memoService.GetUserMemoById(user.Id, id.Value).Value;
                }
                if (memoModel != null)
                {
                    var memoViewModel = _mapper.Map<Memo, MemoViewModel>(memoModel);
                    memoViewModel.CreatedAt = _settingsService.ConvertUTCtoLocalDateTimeString(DateTime.Parse(memoViewModel.CreatedAt), user.Id);
                    memoViewModel.UpdatedAt = memoViewModel.CreatedAt;
                    return View(memoViewModel);
                }
                return NotFound(_localizer.GetString(Message.NotFound));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
            }
        }

        [NoDirectAccess]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(long? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return BadRequest(_localizer.GetString(Message.BadRequest));
                }
                var memoModel = _memoService.GetMemoById(id.Value).Value;
                if (memoModel != null)
                {
                    var viewModel = _mapper.Map<Memo, MemoViewModel>(memoModel);
                    return View(viewModel);
                }
                return NotFound(_localizer.GetString(Message.NotFound));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                        return Json(new { isValid = true, message = _localizer.GetString(Message.UpdatedSuccessfully) });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
                }
                return View(memoViewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
            }
        }        

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(long id)
        {
            try
            {
                var isDeleted = _memoService.DeleteMemo(id);
                if (isDeleted.Value == true)
                {
                    return Json(new { success = true, message = _localizer.GetString(Message.DeletedSuccessfully) });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, _localizer.GetString(Message.SomethingWrongError));
        }        
    }
}
