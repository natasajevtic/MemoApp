using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoApp.Models
{
    public class PersonSettingsModel
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string ZoneName { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string Culture { get; set; }        
    }
}
