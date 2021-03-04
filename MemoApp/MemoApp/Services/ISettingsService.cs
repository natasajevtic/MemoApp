using MemoApp.Models;
using System;

namespace MemoApp.Services
{
    public interface ISettingsService
    {
        PersonSettingsModel GetPersonSetting(string personId);
        string ConvertUTCtoLocalDateTimeString(DateTime dateTime, string personId);
        string ConvertUTCtoLocalDateString(DateTime dateTime, string personId);
        string ConvertUTCtoLocalTimeString(DateTime dateTime, string personId);
        DateTime ConvertUTCtoLocalDate(DateTime dateTime, string personId);
        DateTime ConvertLocalToUTCDate(DateTime dateTime, string personId);        
    }
}
