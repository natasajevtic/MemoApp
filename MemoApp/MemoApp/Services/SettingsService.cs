using MemoApp.Data;
using MemoApp.Models;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
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
                .ToString(personSettings.DateFormat, CultureInfo.InvariantCulture);
        }

        public string ConvertUTCtoLocalDateTimeString(DateTime dateTime, string personId)
        {
            var personSettings = GetPersonSetting(personId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(personSettings.ZoneName))
                .ToString(string.Join(' ', personSettings.DateFormat, personSettings.TimeFormat), CultureInfo.InvariantCulture);            
        }

        public string ConvertUTCtoLocalTimeString(DateTime dateTime, string personId)
        {
            var personSettings = GetPersonSetting(personId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(personSettings.ZoneName))
                .ToString(personSettings.TimeFormat, CultureInfo.InvariantCulture);           
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
            var personSettings = _entities.Settings.Where(s => s.UserId == personId).FirstOrDefault();
            if (personSettings != null)
            {
                model.Id = personSettings.Id;
                model.UserId = personSettings.UserId;
                model.ZoneName = personSettings.ZoneName;
                model.DateFormat = personSettings.DateFormat;
                model.TimeFormat = personSettings.TimeFormat;
                model.Culture = personSettings.Culture;
            }
            return model;
        }
    }
}
