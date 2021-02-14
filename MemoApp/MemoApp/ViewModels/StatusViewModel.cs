using System.ComponentModel.DataAnnotations;

namespace MemoApp.ViewModels
{
    public class StatusViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
