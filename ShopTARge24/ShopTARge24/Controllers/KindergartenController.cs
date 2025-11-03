using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using ShopTARge24.Core.Domain;
using ShopTARge24.Core.Dto;
using ShopTARge24.Core.ServiceInterface;
using ShopTARge24.Data;
using ShopTARge24.Models.Kindergarten;
using Microsoft.EntityFrameworkCore;


namespace ShopTARge24.Controllers
{
    public class KindergartenController : Controller
    {
        private readonly ShopTARge24Context _context;
        private readonly IKindergartenServices _kindergartenServices;
        private readonly IFileServices _fileServices;

        public KindergartenController
            (
                ShopTARge24Context context,
                IKindergartenServices kindergartenServices,
                IFileServices fileServices
            )
        {
            _context = context;
            _kindergartenServices = kindergartenServices;
            _fileServices = fileServices;
        }

        public IActionResult Index()
        {
            var result = _context.Kindergartens
                .Select(x => new KindergartenIndexViewModel
                {
                    Id = x.Id,
                    GroupName = x.GroupName,
                    KindergartenName = x.KindergartenName,
                    ChildrenCount = x.ChildrenCount,
                    TeacherName = x.TeacherName,
                    ImagePath = x.ImagePath,
                });

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            KindergartenCreateUpdateViewModel result = new();

            return View("CreateUpdate", result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KindergartenCreateUpdateViewModel vm)
        {
            var dto = new KindergartenDto()
            {
                Id = vm.Id,
                GroupName = vm.GroupName,
                ChildrenCount = vm.ChildrenCount,
                KindergartenName = vm.KindergartenName,
                TeacherName = vm.TeacherName,
                Files = vm.ImageFiles,
                CreatedAt = vm.CreatedAt,
                UpdatedAt = vm.UpdatedAt
            };

            var result = await _kindergartenServices.Create(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Use FileServices to handle file uploads
            await _fileServices.FilesToApi(dto, result);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var kindergarten = await _kindergartenServices.DetailAsync(id);

            if (kindergarten == null)
            {
                return NotFound();
            }

            var vm = new KindergartenCreateUpdateViewModel();

            vm.Id = kindergarten.Id;
            vm.GroupName = kindergarten.GroupName;
            vm.ChildrenCount = kindergarten.ChildrenCount;
            vm.KindergartenName = kindergarten.KindergartenName;
            vm.TeacherName = kindergarten.TeacherName;
            vm.ImagePath = kindergarten.ImagePath;
            vm.ImagePaths = await _context.Set<KindergartenImage>()
                .Where(x => x.KindergartenId == id)
                .Select(x => x.ImagePath)
                .ToListAsync();
            vm.CreatedAt = kindergarten.CreatedAt;
            vm.UpdatedAt = kindergarten.UpdatedAt;

            return View("CreateUpdate", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(KindergartenCreateUpdateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Reload existing data when validation fails
                if (vm.Id.HasValue)
                {
                    var kindergarten = await _kindergartenServices.DetailAsync(vm.Id.Value);
                    if (kindergarten != null)
                    {
                        vm.ImagePath = kindergarten.ImagePath;
                        vm.ImagePaths = await _context.Set<KindergartenImage>()
                            .Where(x => x.KindergartenId == vm.Id.Value)
                            .Select(x => x.ImagePath)
                            .ToListAsync();
                    }
                }
                return View("CreateUpdate", vm);
            }

            var dto = new KindergartenDto()
            {
                Id = vm.Id,
                GroupName = vm.GroupName,
                ChildrenCount = vm.ChildrenCount,
                KindergartenName = vm.KindergartenName,
                TeacherName = vm.TeacherName,
                Files = vm.ImageFiles,
                CreatedAt = vm.CreatedAt,
                UpdatedAt = DateTime.Now
            };

            var result = await _kindergartenServices.Update(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Use FileServices to handle file uploads
            await _fileServices.FilesToApi(dto, result);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var kindergarten = await _context.Kindergartens.FirstOrDefaultAsync(x => x.Id == id);

            if (kindergarten == null)
            {
                return NotFound();
            }

            var vm = new KindergartenDeleteViewModel();

            vm.Id = kindergarten.Id;
            vm.GroupName = kindergarten.GroupName;
            vm.ChildrenCount = kindergarten.ChildrenCount;
            vm.KindergartenName = kindergarten.KindergartenName;
            vm.TeacherName = kindergarten.TeacherName;
            vm.ImagePath = kindergarten.ImagePath;
            vm.ImagePaths = await _context.Set<KindergartenImage>()
                .Where(x => x.KindergartenId == id)
                .Select(x => x.ImagePath)
                .ToListAsync();
            vm.CreatedAt = kindergarten.CreatedAt;
            vm.UpdatedAt = kindergarten.UpdatedAt;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var kindergarten = await _kindergartenServices.Delete(id);

            if (kindergarten == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            //kasutada service classi meetodit, et info kätte saada
            var kindergarten = await _context.Kindergartens.FirstOrDefaultAsync(x => x.Id == id);

            if(kindergarten == null)
            {
                return NotFound();
            }

            //toimub viewModeliga mappimine
            var vm = new KindergartenDetailsViewModel();

            vm.Id = kindergarten.Id;
            vm.GroupName = kindergarten.GroupName;
            vm.ChildrenCount = kindergarten.ChildrenCount;
            vm.KindergartenName = kindergarten.KindergartenName;
            vm.TeacherName = kindergarten.TeacherName;
            vm.ImagePath = kindergarten.ImagePath;
            vm.CreatedAt = kindergarten.CreatedAt;
            vm.UpdatedAt = kindergarten.UpdatedAt;
            vm.ImagePath = kindergarten.ImagePath;
            vm.ImagePaths = await _context.Set<KindergartenImage>()
                .Where(x => x.KindergartenId == id)
                .Select(x => x.ImagePath)
                .ToListAsync();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(Guid kindergartenId, string imagePath)
        {
            // Find and delete the image record
            var imageRecord = await _context.Set<KindergartenImage>()
                .FirstOrDefaultAsync(x => x.KindergartenId == kindergartenId && x.ImagePath == imagePath);

            if (imageRecord != null)
            {
                _context.Set<KindergartenImage>().Remove(imageRecord);
                await _context.SaveChangesAsync();

                // Delete the physical file
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                    Console.WriteLine($"Successfully deleted file: {physicalPath}");
                }
                else
                {
                    Console.WriteLine($"File not found: {physicalPath}");
                }

                // Check if this was the primary image and update the main record
                var kindergarten = await _context.Kindergartens.FirstOrDefaultAsync(x => x.Id == kindergartenId);
                if (kindergarten != null && kindergarten.ImagePath == imagePath)
                {
                    // Get the next available image or set to null
                    var remainingImages = await _context.Set<KindergartenImage>()
                        .Where(x => x.KindergartenId == kindergartenId)
                        .Select(x => x.ImagePath)
                        .ToListAsync();
                    
                    kindergarten.ImagePath = remainingImages.FirstOrDefault();
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Update), new { id = kindergartenId });
        }

    }
}