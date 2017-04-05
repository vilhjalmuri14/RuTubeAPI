using System.ComponentModel.DataAnnotations.Schema;

namespace RuTubeAPI.Services.Models.Entities
{
	[Table("Videos")]
	public class Video
	{
		public int    ID    { get; set; }

		public string Title  { get; set; }

        public string Description  { get; set; }
	}
}