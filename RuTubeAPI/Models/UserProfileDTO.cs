using System.Collections.Generic;

namespace RuTubeAPI.Models
{
	public class UserProfileDTO
	{
        public int    ID    { get; set; }

		public string Name { get; set; }

        public string Email { get; set; }

		public List<VideoDTO> FavoriteVideos { get; set; }

        public List<UserDTO> CloseFriends { get; set; }
	}
}
