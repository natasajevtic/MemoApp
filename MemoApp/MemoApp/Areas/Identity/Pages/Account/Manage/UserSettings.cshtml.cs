using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using MemoApp.Services;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MemoApp.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;

namespace MemoApp.Areas.Identity.Pages.Account.Manage
{
    public class UserSettingsModel : PageModel
    {
        private readonly IOptions<RequestLocalizationOptions> _locOptions;
        private readonly ISettingsService _serviceSettings;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMemoService _memoService;
        private readonly IMapper _mapper;
        private readonly List<string> Dates = new List<string>() { "dd.MM.yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };
        private readonly List<string> Times = new List<string>() { "HH:mm", "HH:mm tt" };

        [BindProperty(SupportsGet = true)]
        public PersonSettingsModel Settings { get; set; }
        public List<SelectListItem> Zones { get; set; }
        public List<SelectListItem> Cultures { get; set; }
        public List<SelectListItem> DateFormats { get; set; }
        public List<SelectListItem> TimeFormats { get; set; }
        
        public UserSettingsModel(IOptions<RequestLocalizationOptions> locOptions, ISettingsService serviceSettings, UserManager<IdentityUser> userManager, IMemoService memoService, IMapper mapper)
        {
            _locOptions = locOptions;
            _serviceSettings = serviceSettings;
            _userManager = userManager;
            _memoService = memoService;
            _mapper = mapper;
            Zones = TimeZoneInfo.GetSystemTimeZones().Select(z => new SelectListItem { Value = z.Id, Text = z.Id }).ToList();
            Cultures = _locOptions.Value.SupportedUICultures.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }).ToList();
            DateFormats = Dates.Select(d => new SelectListItem { Value = d, Text = d }).ToList();
            TimeFormats = Times.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                Settings = _serviceSettings.GetPersonSetting(user.Id);
                return Page();
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<PersonSettingsModel, Setting>(Settings);
                if (Settings.Id > 0)
                {
                    _memoService.UpdateSettings(model);
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    model.UserId = user.Id;
                    _memoService.AddSettings(model);
                }

                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Settings.Culture)),
                    new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

                return RedirectToAction("Index", "Memo");
            }
            return Page();            
        }
    }
}
