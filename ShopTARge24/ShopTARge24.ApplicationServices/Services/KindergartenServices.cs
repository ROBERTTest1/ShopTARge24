using Microsoft.EntityFrameworkCore;
using ShopTARge24.Core.Domain;
using ShopTARge24.Core.Dto;
using ShopTARge24.Core.ServiceInterface;
using ShopTARge24.Data;
using Microsoft.Extensions.Logging;


namespace ShopTARge24.ApplicationServices.Services
{
    public class KindergartenServices : IKindergartenServices
    {
        private readonly ShopTARge24Context _context;

        public KindergartenServices
            (
                ShopTARge24Context context
            )
        {
            _context = context;
        }

        public async Task<Kindergarten> Create(KindergartenDto dto)
        {
            Kindergarten kindergarten = new Kindergarten();

            kindergarten.Id = Guid.NewGuid();
            kindergarten.GroupName = dto.GroupName;
            kindergarten.ChildrenCount = dto.ChildrenCount;
            kindergarten.KindergartenName = dto.KindergartenName;
            kindergarten.TeacherName = dto.TeacherName;
            kindergarten.ImagePath = dto.ImagePath;
            kindergarten.CreatedAt = DateTime.Now;
            kindergarten.UpdatedAt = DateTime.Now;

            await _context.Kindergartens.AddAsync(kindergarten);
            await _context.SaveChangesAsync();

            return kindergarten;
        }

        public async Task<Kindergarten> Update(KindergartenDto dto)
        {
            // Find the existing kindergarten
            var kindergarten = await _context.Kindergartens.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (kindergarten == null)
            {
                return null;
            }

            // Update the existing kindergarten
            kindergarten.GroupName = dto.GroupName;
            kindergarten.ChildrenCount = dto.ChildrenCount;
            kindergarten.KindergartenName = dto.KindergartenName;
            kindergarten.TeacherName = dto.TeacherName;
            // Preserve existing image if none provided via DTO (image updates are handled by FileServices)
            if (!string.IsNullOrWhiteSpace(dto.ImagePath))
            {
                kindergarten.ImagePath = dto.ImagePath;
            }
            kindergarten.UpdatedAt = DateTime.Now;

            // Save the changes (force track as modified to avoid silent no-op)
            _context.Entry(kindergarten).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return kindergarten;
        }

        public async Task<Kindergarten> DetailAsync(Guid id)
        {
            var result = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }

        public async Task<Kindergarten> Delete(Guid id)
        {
            //leida ülesse konkreetne soovitud rida, mida soovite kustutada
            var result = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == id);


            //kui rida on leitud, siis eemaldage andmebaasist
            if (result != null)
            {
                // Delete images from FileToApis table
                var fileToApiImages = await _context.FileToApis
                    .Where(x => x.KindergartenId == id)
                    .ToListAsync();
                
                if (fileToApiImages.Any())
                {
                    _context.FileToApis.RemoveRange(fileToApiImages);
                }

                // Delete images from KindergartenImages table (legacy)
                var kindergartenImages = await _context.Set<KindergartenImage>()
                    .Where(x => x.KindergartenId == id)
                    .ToListAsync();
                
                if (kindergartenImages.Any())
                {
                    _context.Set<KindergartenImage>().RemoveRange(kindergartenImages);
                }

                // Delete the kindergarten itself
                _context.Kindergartens.Remove(result);
                
                // Save all changes in one transaction
                await _context.SaveChangesAsync();
            }

            return result;
        }
    }
}
