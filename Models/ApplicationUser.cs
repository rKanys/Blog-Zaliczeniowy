using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Blog_Zaliczeniowy.Models
{
	public class ApplicationUser : IdentityUser
	{
		[Required]
		[StringLength(30)]
		[Display(Name ="Nazwa użytkownika")]
		public string Nickname { get; set; }

		[Display(Name = "Data rejestracji")]
		public DateTime RegistrationDate { get; set; }

		[Display(Name = "Ostatnie logowanie")]
		public DateTime? LastLoginDate { get; set; }

		[Display(Name = "Ilość logowań")]
		public int LoginCount { get; set; }

		[Display(Name = "O mnie:")]
		[StringLength(2000)]
		public string AboutMe { get; set; }

		[Display(Name = "Status":)]
		public bool IsDeleted { get; set; } = false; 
	}
}
