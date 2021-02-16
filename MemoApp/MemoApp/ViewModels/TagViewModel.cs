using System.ComponentModel.DataAnnotations;

namespace MemoApp.ViewModels
{
    public class TagViewModel
    {
        public long Id { get; set; }

        [MaxLength(50, ErrorMessage = "Tag length must be less than 50.")]
        public string Name { get; set; }
        public long MemoId { get; set; }
    }
}
