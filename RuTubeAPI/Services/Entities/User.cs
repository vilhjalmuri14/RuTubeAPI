using System.ComponentModel.DataAnnotations.Schema;

namespace RuTubeAPI.Services.Models.Entities
{
	[Table("Users")]
	public class User
	{
		public int    ID    { get; set; }

		public string Name  { get; set; }

        public string Password  { get; set; }

        public string Token  { get; set; }

		public string Email { get; set; }
	}
}
