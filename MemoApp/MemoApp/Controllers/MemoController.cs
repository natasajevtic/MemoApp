using AutoMapper;
using MemoApp.Data;
using MemoApp.Services;
using MemoApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.Controllers
{
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
                        var viewModel = _mapper.Map<Memo, MemoViewModel>(memoModel);
                        return RedirectToAction("Details", new { id = viewModel.Id });
                    }

                    return RedirectToPage("/Error");
                }

                return View();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetBaseException().Message}");
                return RedirectToPage("/Error");
            }
        }

        [HttpGet]
        public IActionResult Details(int? id)
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
                Log.Error($"{ex.GetBaseException().Message}");
                return RedirectToPage("/Error");
            }
        }
    }
}
