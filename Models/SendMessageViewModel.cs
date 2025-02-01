using System.ComponentModel.DataAnnotations;

namespace Blog_Zaliczeniowy.Models
{
	public class SendMessageViewModel
	{
		[Required(ErrorMessage = "Odbiorca musi być sprecyzowany")]
		public string RecipientId { get; set; }

		[Required(ErrorMessage = "Wiadomość nie może być pusta")]
		[StringLength(4000)]
		[Display(Name = "Wiadomość")]
		public string Content { get; set; }
	}
}
