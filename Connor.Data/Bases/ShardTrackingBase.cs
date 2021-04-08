using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connor.Data.Bases
{
    public class ShardTrackingBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrackingId { get; set; }
        [Required, MaxLength(100)]
        public string Field { get; set; }
        public long NextId { get; set; }
    }
}
