using System;

namespace ShopTARge24.Core.Domain
{
    public class KindergartenImage
    {
        public Guid Id { get; set; }
        public Guid KindergartenId { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


