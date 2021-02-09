using System;
using System.Collections.Generic;

#nullable disable

namespace MemoApp.Data
{
    public partial class Setting
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string ZoneName { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string Culture { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
