using System.ComponentModel.DataAnnotations;

namespace ShopTARge24.Core.Domain
{
    public class FileToDatabase
    {
        [Key]
        public Guid Id { get; set; }
        public string ImageTitle { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = new byte[0];
        public Guid? RealEstateId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}