using System.ComponentModel.DataAnnotations;

namespace ShopTARge24.Core.Domain
{
    public class FileToApi
    {
        [Key]
        public Guid Id { get; set; }
        public string ExistingFilePath { get; set; } = string.Empty;
        public string ImageTitle { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = new byte[0];
        public Guid? SpaceshipId { get; set; }
        public Guid? RealEstateId { get; set; }
        public Guid? KindergartenId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
