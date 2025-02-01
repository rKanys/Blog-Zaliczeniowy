using System.ComponentModel.DataAnnotations;

namespace Blog_Zaliczeniowy.Models
{
	public class EditPostViewModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Pole tytułu musi być uzupełnione")]
		[StringLength(40)]
		[Display(Name = "Tytuł")]
		public string Title { get; set; }

		[Required]
		[StringLength(8000)]
		[Display(Name = "Zawartość")]
		public string Content { get; set; }

		[Display(Name = "Użytkownik")]
		public string UserId { get; set; }
	}
}
