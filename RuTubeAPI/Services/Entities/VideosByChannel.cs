using System.ComponentModel.DataAnnotations.Schema;

namespace RuTubeAPI.Services.Models.Entities
{
	[Table("VideosByChannel")]
	public class VideosByChannel
	{
        public int    ID    { get; set; }

		public int    VideoID    { get; set; }

		public int    ChannelID  { get; set; }
	}
}
