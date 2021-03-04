using System.ComponentModel.DataAnnotations;

namespace MemoApp.Models
{
    public class PersonSettingsModel
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "The field Time zone is required.")]
        public string ZoneName { get; set; }
        [Required(ErrorMessage = "The field Date format is required.")]
        public string DateFormat { get; set; }
        [Required(ErrorMessage = "The field Time format is required.")]
        public string TimeFormat { get; set; }
        [Required(ErrorMessage = "The field Culture is required.")]
        public string Culture { get; set; }        
    }
}
