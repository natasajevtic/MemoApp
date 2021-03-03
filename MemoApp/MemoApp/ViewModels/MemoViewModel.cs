using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MemoApp.ViewModels
{
    public class MemoViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        [MaxLength(50, ErrorMessage = "The Title must be less than 50 characters long.")]
        public string Title { get; set; }
        public string Note { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public string UserId { get; set; }
        public StatusViewModel Status { get; set; }
        public List<TagViewModel> Tags { get; set; }
        public string TagString
        {
            get
            {
                //creating a string of memo tag names from the collection
                return string.Join(' ', Tags.Select(t => t.Name));
            }
            set
            {
                //if the user added memo tags, splitting that entry by a space, creating tags
                //and adding them to the collection
                if (!String.IsNullOrEmpty(value))
                {
                    var tagArray = value.Split(' ');
                    foreach (var tag in tagArray)
                    {
                        if (!String.IsNullOrWhiteSpace(tag))
                        {
                            var tagViewModel = new TagViewModel()
                            {
                                Name = tag,
                                MemoId = Id
                            };
                            Tags.Add(tagViewModel);
                        }
                    }
                }
            }
        }

        public MemoViewModel()
        {
            Tags = new List<TagViewModel>();
        }       
    }
}
