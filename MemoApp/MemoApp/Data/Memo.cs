using System;
using System.Collections.Generic;

#nullable disable

namespace MemoApp.Data
{
    public partial class Memo
    {
        public Memo()
        {
            Tags = new HashSet<Tag>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int StatusId { get; set; }
        public string UserId { get; set; }

        public virtual Status Status { get; set; }
        public virtual AspNetUser User { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
