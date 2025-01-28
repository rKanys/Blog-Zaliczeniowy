using Microsoft.AspNetCore.Identity;

namespace Blog_Zaliczeniowy.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Nickname { get; set; }
	}
}
