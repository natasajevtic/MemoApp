using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemoApp.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [MaxLength(256, ErrorMessage = "The Name must be less than 256 characters long.")]
        public string Name { get; set; }
        public List<string> Users { get; set; }

        public RoleViewModel()
        {
            Users = new List<string>();
        }
    }
}
