using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemoApp.ViewModels
{
    public class MemoViewModel
    {
        public long Id { get; set; }
        [Required, MaxLength(50)]
        public string Title { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }      
        public string UserId { get; set; }
        public StatusViewModel Status { get; set; }
        public ICollection<TagViewModel> Tags { get; set; }
        public string TagString { get; set; }
    }
}
