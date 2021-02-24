using System.ComponentModel.DataAnnotations;

namespace MemoApp.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required, MaxLength(256)]
        public string Name { get; set; }
    }
}
