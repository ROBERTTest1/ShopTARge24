using System;
using Microsoft.AspNetCore.Http;

namespace ShopTARge24.Models.Kindergarten
{
    public class KindergartenCreateUpdateViewModel
    {
        public Guid? Id { get; set; }
        public string? GroupName { get; set; }
        public int? ChildrenCount { get; set; }
        public string? KindergartenName { get; set; }
        public string? TeacherName { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? ImageFile { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
