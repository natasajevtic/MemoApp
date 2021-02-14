using System;
using System.Collections.Generic;

#nullable disable

namespace MemoApp.Data
{
    public partial class Status
    {
        public Status()
        {
            Memos = new HashSet<Memo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Memo> Memos { get; set; }
    }
}
