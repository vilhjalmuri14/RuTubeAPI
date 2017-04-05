using System.ComponentModel.DataAnnotations.Schema;

namespace RuTubeAPI.Services.Models.Entities
{
	[Table("UserFriends")]
	public class UserFriend
	{
		public int    ID    { get; set; }

		public int    UserID    { get; set; }

		public int    FriendID  { get; set; }
	}
}
