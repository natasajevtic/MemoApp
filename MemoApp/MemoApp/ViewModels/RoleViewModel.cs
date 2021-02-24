using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemoApp.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required, MaxLength(256)]
        public string Name { get; set; }
        public List<string> Users { get; set; }

        public RoleViewModel()
        {
            Users = new List<string>();
        }
    }
}
