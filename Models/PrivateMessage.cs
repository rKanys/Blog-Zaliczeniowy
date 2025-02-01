namespace Blog_Zaliczeniowy.Models
{
	public class PrivateMessage
	{
		public int Id { get; set; }
		public string SenderId { get; set; }
		public ApplicationUser Sender { get; set; }
		public string RecipientId { get; set; }
		public ApplicationUser Recipient { get; set; }
		public string Content { get; set; }
		public DateTime SentAt { get; set; }
		public bool IsRead { get; set; }
	}
}
