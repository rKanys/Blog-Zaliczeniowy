using System.ComponentModel.DataAnnotations;

namespace Blog_Zaliczeniowy.Models
{
	public class EditProfileViewModel
	{
		public string UserId { get; set; }
		[Required]
		[StringLength(30)]
		[Display(Name = "Nazwa użytkownika")]
		public string Nickname { get; set; }
		[Display(Name = "O mnie")]
		public string AboutMe { get; set; }
	}
}
