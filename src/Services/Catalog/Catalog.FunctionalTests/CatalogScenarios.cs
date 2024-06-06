namespace Catalog.FunctionalTests;

public class CatalogScenarios : IClassFixture<CatalogWebApplicationFactory>
{
    private readonly CatalogWebApplicationFactory _factory;

    public CatalogScenarios(CatalogWebApplicationFactory factory)
        => _factory = factory;

    [Fact]
    public async Task Get_get_all_catalogitems_and_response_ok_status_code()
    {
        var response = await _factory.CreateClient()
            .GetAsync(Get.Items());

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_get_catalogitem_by_id_and_response_ok_status_code()
    {
        var response = await _factory.CreateClient()
            .GetAsync(Get.ItemByIds(1));

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_get_paginated_catalog_items_and_response_ok_status_code()
    {
        const bool paginated = true;
        var response = await _factory.CreateClient()
            .GetAsync(Get.Items(paginated));

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_get_filtered_catalog_items_and_response_ok_status_code()
    {
        var response = await _factory.CreateClient()
            .GetAsync(Get.Filtered(1, 1));

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_get_paginated_filtered_catalog_items_and_response_ok_status_code()
    {
        const bool paginated = true;
        var response = await _factory.CreateClient()
            .GetAsync(Get.Filtered(1, 1, paginated));

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_catalog_types_response_ok_status_code()
    {
        var response = await _factory.CreateClient()
            .GetAsync(Get.Types);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_catalog_brands_response_ok_status_code()
    {
        var response = await _factory.CreateClient()
            .GetAsync(Get.Brands);

        response.EnsureSuccessStatusCode();
    }
}