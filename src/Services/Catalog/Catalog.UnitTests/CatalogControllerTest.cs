namespace Catalog.UnitTests;

public class CatalogControllerTest
{
    private readonly DbContextOptions<CatalogDbContext> _dbOptions;

    public CatalogControllerTest()
    {
        _dbOptions = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: "in-memory")
            .Options;

        using var dbContext = new CatalogDbContext(_dbOptions);
        dbContext.AddRange(GetFakeCatalog());
        dbContext.SaveChanges();
    }

    [Fact]
    public async Task Get_catalog_items_success()
    {
        //Arrange
        var brandFilterApplied = 1;
        var typesFilterApplied = 2;
        var pageSize = 4;
        var pageIndex = 1;

        var expectedItemsInPage = 2;
        var expectedTotalItems = 6;

        var catalogContext = new CatalogDbContext(_dbOptions);

        //Act
        var catalogController = new CatalogController(catalogContext);
        var page = await catalogController.ItemsAsync(typesFilterApplied, brandFilterApplied, pageSize, pageIndex);

        //Assert
        Assert.Equal(expectedTotalItems, page.Count);
        Assert.Equal(pageIndex, page.PageIndex);
        Assert.Equal(pageSize, page.PageSize);
        Assert.Equal(expectedItemsInPage, page.Items.Count());
    }

    private static List<CatalogItem> GetFakeCatalog()
    {
        return new List<CatalogItem>()
        {
            new(1, "fakeItemA", 1m, "fakeItemA.png", 2, 1, 10),
            new(2, "fakeItemB", 1m, "fakeItemB.png", 2, 1, 10),
            new(3, "fakeItemC", 1m, "fakeItemC.png", 2, 1, 10),
            new(4, "fakeItemD", 1m, "fakeItemD.png", 2, 1, 10),
            new(5, "fakeItemE", 1m, "fakeItemE.png", 2, 1, 10),
            new(6, "fakeItemF", 1m, "fakeItemF.png", 2, 1, 10),
        };
    }
}