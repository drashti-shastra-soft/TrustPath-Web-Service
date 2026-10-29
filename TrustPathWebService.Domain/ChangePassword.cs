using System;
using System.Linq;

namespace TrustPathWebService.Domain
{
    public class ChangePassword
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
