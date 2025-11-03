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
            vm.CreatedAt = kindergarten.CreatedAt;
            vm.UpdatedAt = kindergarten.UpdatedAt;

            // Load current images
            var images = await _context.FileToApis
                .Where(x => x.KindergartenId == id)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
            vm.ImageIds = images.Select(i => i.Id).ToList();
            vm.ImageTitles = images.Select(i => i.ImageTitle).ToList();
            vm.ImagePaths = images.Select(i => $"/files/api/{i.Id}").ToList();

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
                        var images = await _context.FileToApis
                            .Where(x => x.KindergartenId == vm.Id.Value)
                            .OrderBy(x => x.CreatedAt)
                            .ToListAsync();
                        vm.ImageIds = images.Select(i => i.Id).ToList();
                        vm.ImageTitles = images.Select(i => i.ImageTitle).ToList();
                        vm.ImagePaths = images.Select(i => $"/files/api/{i.Id}").ToList();
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

        [HttpPost]
        public async Task<IActionResult> DeleteImage(Guid kindergartenId, Guid imageId)
        {
            var image = await _context.FileToApis.FirstOrDefaultAsync(x => x.Id == imageId && x.KindergartenId == kindergartenId);
            if (image != null)
            {
                _context.FileToApis.Remove(image);
                await _context.SaveChangesAsync();

                // Update kindergarten thumbnail if needed
                var remaining = await _context.FileToApis
                    .Where(x => x.KindergartenId == kindergartenId)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();
                var kg = await _context.Kindergartens.FirstOrDefaultAsync(x => x.Id == kindergartenId);
                if (kg != null)
                {
                    kg.ImagePath = remaining.Any() ? $"/files/api/{remaining.First().Id}" : null;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Update", new { id = kindergartenId });
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

            // Load all images for gallery
            var images = await _context.FileToApis
                .Where(x => x.KindergartenId == id)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
            vm.ImagePaths = images.Select(i => $"/files/api/{i.Id}").ToList();

            return View(vm);
        }

    }
}