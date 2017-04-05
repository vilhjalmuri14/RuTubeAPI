using System.ComponentModel.DataAnnotations.Schema;

namespace RuTubeAPI.Services.Models.Entities
{
	[Table("FavoriteVideosByUser")]
	public class FavoriteVideosByUser
	{
        public int    ID    { get; set; }

		public int    UserID    { get; set; }

		public int    VideoID  { get; set; }
	}
}
