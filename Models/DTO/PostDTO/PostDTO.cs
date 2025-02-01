using System.ComponentModel.DataAnnotations;

namespace Blog_Zaliczeniowy.Models.DTO.PostDTO
{
	public class PostDTO
	{
		[Required(ErrorMessage = "Pole tytułu musi być uzupełnione")]
		[StringLength(40)]
		[Display(Name = "Tytuł")]
		public string Title { get; set; }

		[Required]
		[StringLength(8000)]
		[Display(Name = "Zawartość")]
		public string Content { get; set; }
		public string? userID { get; set; }
	}
}
