using Microsoft.EntityFrameworkCore;
using ShopTARge24.ApplicationServices.Services;
using ShopTARge24.Core.Domain;
using ShopTARge24.Core.Dto;
using ShopTARge24.Data;
using Xunit;

namespace ShopTARge24.Tests;

public class KindergartenServicesTests
{
    private static ShopTARge24Context CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ShopTARge24Context>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ShopTARge24Context(options);
    }

    [Fact]
    public async Task Create_WithValidDto_CreatesKindergarten()
    {
        await using var context = CreateInMemoryContext();
        var service = new KindergartenServices(context);

        var dto = new KindergartenDto
        {
            GroupName = "Lilled",
            ChildrenCount = 15,
            KindergartenName = "Päikese lasteaed",
            TeacherName = "Mari Maasikas",
            ImagePath = "/images/test.jpg"
        };

        var result = await service.Create(dto);

        Assert.NotNull(result);
        Assert.NotNull(result.Id);
        Assert.Equal("Lilled", result.GroupName);
        Assert.Equal(15, result.ChildrenCount);
        Assert.Equal("Päikese lasteaed", result.KindergartenName);
        Assert.Equal("Mari Maasikas", result.TeacherName);

        var saved = await context.Kindergartens.FirstOrDefaultAsync(k => k.Id == result.Id);
        Assert.NotNull(saved);
        Assert.Equal("Lilled", saved.GroupName);
    }

    [Fact]
    public async Task Create_WithValidDto_SetsCreatedAtAndUpdatedAt()
    {
        await using var context = CreateInMemoryContext();
        var service = new KindergartenServices(context);

        var dto = new KindergartenDto
        {
            GroupName = "Mesilased",
            ChildrenCount = 12,
            KindergartenName = "Kollane maja",
            TeacherName = "Jaan Jõul"
        };

        var result = await service.Create(dto);

        Assert.NotNull(result.CreatedAt);
        Assert.NotNull(result.UpdatedAt);
        Assert.True(result.CreatedAt.Value > DateTime.MinValue);
        Assert.True(result.UpdatedAt.Value > DateTime.MinValue);
        Assert.True((result.UpdatedAt!.Value - result.CreatedAt!.Value).TotalSeconds < 1);
    }

    [Fact]
    public async Task DetailAsync_WithExistingId_ReturnsKindergarten()
    {
        await using var context = CreateInMemoryContext();
        var service = new KindergartenServices(context);

        var id = Guid.NewGuid();
        var kindergarten = new Kindergarten
        {
            Id = id,
            GroupName = "Karud",
            ChildrenCount = 18,
            KindergartenName = "Metsa maja",
            TeacherName = "Tiina Tamm",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        context.Kindergartens.Add(kindergarten);
        await context.SaveChangesAsync();

        var result = await service.DetailAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Karud", result.GroupName);
        Assert.Equal(18, result.ChildrenCount);
        Assert.Equal("Metsa maja", result.KindergartenName);
    }

    [Fact]
    public async Task DetailAsync_WithNonExistingId_ReturnsNull()
    {
        await using var context = CreateInMemoryContext();
        var service = new KindergartenServices(context);

        var result = await service.DetailAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_WithExistingKindergarten_UpdatesAndReturns()
    {
        await using var context = CreateInMemoryContext();
        var service = new KindergartenServices(context);

        var id = Guid.NewGuid();
        context.Kindergartens.Add(new Kindergarten
        {
            Id = id,
            GroupName = "Vanasti",
            ChildrenCount = 10,
            KindergartenName = "Vana nimi",
            TeacherName = "Vana õpetaja",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();

        var dto = new KindergartenDto
        {
            Id = id,
            GroupName = "Uus grupp",
            ChildrenCount = 20,
            KindergartenName = "Uus nimi",
            TeacherName = "Uus õpetaja",
            ImagePath = "/new/image.jpg"
        };

        var result = await service.Update(dto);

        Assert.NotNull(result);
        Assert.Equal("Uus grupp", result.GroupName);
        Assert.Equal(20, result.ChildrenCount);
        Assert.Equal("Uus nimi", result.KindergartenName);
        Assert.Equal("Uus õpetaja", result.TeacherName);
        Assert.Equal("/new/image.jpg", result.ImagePath);

        var updated = await context.Kindergartens.AsNoTracking().FirstAsync(k => k.Id == id);
        Assert.Equal("Uus grupp", updated.GroupName);
        Assert.Equal("Uus nimi", updated.KindergartenName);
    }

    [Fact]
    public async Task Delete_WithExistingId_RemovesKindergarten()
    {
        await using var context = CreateInMemoryContext();
        var service = new KindergartenServices(context);

        var id = Guid.NewGuid();
        context.Kindergartens.Add(new Kindergarten
        {
            Id = id,
            GroupName = "Kustutada",
            ChildrenCount = 5,
            KindergartenName = "Kustutav",
            TeacherName = "Õpetaja",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();

        var result = await service.Delete(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);

        var deleted = await context.Kindergartens.FindAsync(id);
        Assert.Null(deleted);
    }
}
