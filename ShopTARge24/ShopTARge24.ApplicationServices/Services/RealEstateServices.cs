using Microsoft.EntityFrameworkCore;
using ShopTARge24.Core.Domain;
using ShopTARge24.Core.Dto;
using ShopTARge24.Core.ServiceInterface;
using ShopTARge24.Data;


namespace ShopTARge24.ApplicationServices.Services
{
    public class RealEstateServices : IRealEstateServices
    {
        private readonly ShopTARge24Context _context;

        public RealEstateServices
            (
                ShopTARge24Context context
            )
        {
            _context = context;
        }

        public async Task<RealEstate> Create(RealEstateDto dto)
        {
            RealEstate domain = new RealEstate();

            domain.Id = Guid.NewGuid();
            domain.Area = dto.Area;
            domain.Location = dto.Location;
            domain.RoomNumber = dto.RoomNumber;
            domain.BuildingType = dto.BuildingType;
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;

            await _context.RealEstates.AddAsync(domain);
            await _context.SaveChangesAsync();

            return domain;
        }

        public async Task<RealEstate> Update(RealEstateDto dto)
        {
            var existingDomain = await _context.RealEstates
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (existingDomain == null)
            {
                return null;
            }

            existingDomain.Area = dto.Area;
            existingDomain.Location = dto.Location;
            existingDomain.RoomNumber = dto.RoomNumber;
            existingDomain.BuildingType = dto.BuildingType;
            existingDomain.ModifiedAt = DateTime.Now;

            _context.RealEstates.Update(existingDomain);
            await _context.SaveChangesAsync();

            return existingDomain;  
        }

        public async Task<RealEstate> DetailAsync(Guid id)
        {
            var result = await _context.RealEstates
                .FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }

        public async Task<RealEstate> Delete(Guid id)
        {
            var result = await _context.RealEstates
                .FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return null;
            }

            _context.RealEstates.Remove(result);
            await _context.SaveChangesAsync();

            return result;
        }
    }
}