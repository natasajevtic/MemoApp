using MemoApp.Data;
using MemoApp.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace MemoApp.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IOptions<PersonSettingsModel> _seed;
        private readonly MemoEntities _entities;

        public SettingsService(IOptions<PersonSettingsModel> seed, MemoEntities entities)
        {
            _seed = seed;
            _entities = entities;
        }

        public DateTime ConvertLocalToUTCDate(DateTime dateTime, string personId)
        {
            var personSettings = GetPersonSetting(personId);            
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(personSettings.ZoneName));
        }

        public DateTime ConvertUTCtoLocalDate(DateTime dateTime, string personId)
        {            
            var personSettings = GetPersonSetting(personId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(personSettings.ZoneName));
        }

        public string ConvertUTCtoLocalDateString(DateTime dateTime, string personId)
        {
            var personSettings = GetPersonSetting(personId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(personSettings.ZoneName))
                .ToString(personSettings.DateFormat);
        }

        public string ConvertUTCtoLocalDateTimeString(DateTime dateTime, string personId)
        {
            var personSettings = GetPersonSetting(personId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(personSettings.ZoneName))
                .ToString(string.Join(' ', personSettings.DateFormat, personSettings.TimeFormat));            
        }

        public string ConvertUTCtoLocalTimeString(DateTime dateTime, string personId)
        {
            var personSettings = GetPersonSetting(personId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(personSettings.ZoneName))
                .ToString(personSettings.TimeFormat);           
        }

        public PersonSettingsModel GetPersonSetting(string personId)
        {
            var model = new PersonSettingsModel()
            {
                ZoneName = _seed.Value.ZoneName,
                DateFormat = _seed.Value.DateFormat,
                TimeFormat = _seed.Value.TimeFormat,
                Culture = _seed.Value.Culture
            };
            var personSetting = _entities.Settings.Where(s => s.UserId == personId).FirstOrDefault();
            if (personSetting != null)
            {
                model.Id = personSetting.Id;
                model.UserId = personSetting.UserId;
                model.ZoneName = personSetting.ZoneName;
                model.DateFormat = personSetting.DateFormat;
                model.TimeFormat = personSetting.TimeFormat;
                model.Culture = personSetting.Culture;
            }
            return model;
        }
    }
}
