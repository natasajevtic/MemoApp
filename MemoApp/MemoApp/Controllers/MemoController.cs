using AutoMapper;
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
using System.Linq;
using System.Security.Claims;
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
                    var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    memoModelList = _memoService.GetUserMemos(id).Value;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MemoViewModel memoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if the user added memo tags, splitting that entry by a space, creating tags
                    //and adding them to the collection
                    if (!String.IsNullOrWhiteSpace(memoViewModel.TagString))
                    {
                        memoViewModel.Tags = new List<TagViewModel>();
                        var tagArray = memoViewModel.TagString.Split(' ');
                        foreach (var tag in tagArray)
                        {
                            if (!String.IsNullOrWhiteSpace(tag))
                            {
                                var tagViewModel = new TagViewModel()
                                {
                                    Name = tag
                                };
                                memoViewModel.Tags.Add(tagViewModel);
                            }
                        }
                    }

                    var memoModel = _mapper.Map<MemoViewModel, Memo>(memoViewModel);

                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    memoModel.UserId = user.Id;

                    long memoId = _memoService.AddMemo(memoModel).Value;

                    if (memoId > 0)
                    {                        
                        return RedirectToAction("Details", new { id = memoId });
                    }
                    return RedirectToPage("/Error");
                }
                return View();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}");
                return RedirectToPage("/Error");
            }
        }

        [HttpGet]
        public IActionResult Details(long? id)
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
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    memoModel = _memoService.GetUserMemoById(userId, id.Value).Value;
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
                    //creating a string of memo tags from the collection
                    viewModel.TagString = String.Join(' ', viewModel.Tags.Select(t => t.Name).ToArray());
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
                    //if the user added memo tags, splitting that entry by a space, creating tags
                    //and adding them to the collection
                    if (!String.IsNullOrWhiteSpace(memoViewModel.TagString))
                    {
                        memoViewModel.Tags = new List<TagViewModel>();
                        var tagArray = memoViewModel.TagString.Split(' ');
                        foreach (var tag in tagArray)
                        {
                            if (!String.IsNullOrWhiteSpace(tag))
                            {
                                var tagViewModel = new TagViewModel()
                                {
                                    Name = tag,
                                    MemoId = memoViewModel.Id
                                };
                                memoViewModel.Tags.Add(tagViewModel);
                            }
                        }
                    }
                    var memoModel = _mapper.Map<MemoViewModel, Memo>(memoViewModel);
                    var updatedModel = _memoService.UpdateMemo(memoModel);
                    if (updatedModel.Succeeded)
                    {                        
                        return RedirectToAction("Details", new { id = memoModel.Id });
                    }
                    return RedirectToPage("/Error");
                }
                return View();
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
