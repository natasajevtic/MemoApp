﻿using AutoMapper;
using MemoApp.Constants;
using MemoApp.Data;
using MemoApp.Services;
using MemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> Index()
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
                return View(_mapper.Map<List<Memo>, List<MemoViewModel>>(memoModelList));
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return RedirectToPage("/Error");
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
                        TempData["Message"] = "The memo is saved!";
                        return RedirectToAction("Details", new { id = memoId });
                    }
                    return RedirectToPage("/Error");
                }
                return View(memoViewModel);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return RedirectToPage("/Error");
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
                return RedirectToPage("/NotFound");
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return RedirectToPage("/Error");
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
                return RedirectToPage("/NotFound");
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return RedirectToPage("/Error");
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
                        TempData["Message"] = "The memo is updated!";
                        return RedirectToAction("Details", new { id = memoModel.Id });
                    }
                    return RedirectToPage("/Error");
                }
                return View(memoViewModel);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return RedirectToPage("/Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminRole)]
        public IActionResult Delete(long? id)
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
                    return View(_mapper.Map<Memo, MemoViewModel>(memoModel));
                }
                return RedirectToPage("/NotFound");
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return RedirectToPage("/Error");
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
                    TempData["Message"] = "The memo with id " + id + " is deleted!";
                    return RedirectToAction("Index");
                }                
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
            }
            return RedirectToPage("/Error");
        }
    }
}
