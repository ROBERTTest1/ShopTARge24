namespace ShopTARge24.Core.Dto
{
    public class FileToApiDto
    {
        public Guid Id { get; set; }
        public string ExistingFilePath { get; set; } = string.Empty;
        public string ImageTitle { get; set; } = string.Empty;
        public Guid? SpaceshipId { get; set; }
        public Guid? RealEstateId { get; set; }
        public Guid? KindergartenId { get; set; }
    }
}