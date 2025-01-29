namespace Blog_Zaliczeniowy.Models
{
	public class EditUserViewModel
	{
		public string UserId { get; set; }
		public string Nickname { get; set; }
		public string Email { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
	}
}
