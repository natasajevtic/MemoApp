using System.ComponentModel.DataAnnotations;

namespace MemoApp.ViewModels
{
    public class TagViewModel
    {
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        public long MemoId { get; set; }
    }
}
