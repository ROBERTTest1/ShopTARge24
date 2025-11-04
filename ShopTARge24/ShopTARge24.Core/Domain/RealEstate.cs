using System.ComponentModel.DataAnnotations;

namespace ShopTARge24.Core.Domain
{
    public class RealEstate
    {
        [Key]
        public Guid Id { get; set; }
        public double? Area { get; set; }
        public string? Location { get; set; }
        public int? RoomNumber { get; set; }
        public string? BuildingType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}