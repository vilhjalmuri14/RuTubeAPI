using System.ComponentModel.DataAnnotations.Schema;

namespace RuTubeAPI.Services.Models.Entities
{
    [Table("kings")]
    public class kings
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Info { get; set; }

    }
}
