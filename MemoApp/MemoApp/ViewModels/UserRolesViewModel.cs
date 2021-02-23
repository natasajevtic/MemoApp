using System.Collections.Generic;

namespace MemoApp.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
