using System.ComponentModel.DataAnnotations;

namespace Blog_Zaliczeniowy.Models
{
	public class EditUserViewModel
	{

		public string UserId { get; set; }
		[Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
		[Display(Name = "Nazwa użytkownika")]
		[StringLength(30)]
		public string Nickname { get; set; }

		[Required(ErrorMessage = "Adres e-mail jest wymagany")]
		[Display(Name = "Adres e-mail")]
		[EmailAddress(ErrorMessage = "Błędny format zapisu")]
		public string Email { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
	}
}
