namespace Microsoft.eShopOnDapr.Services.Catalog.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly ICatalogReader _catalogReader;

    public CatalogController(ICatalogReader catalogReader)
    {
        _catalogReader = catalogReader;
    }

    [HttpGet("brands")]
    [ProducesResponseType(typeof(List<IdNameViewModel>), (int)HttpStatusCode.OK)]
    public Task<List<IdNameViewModel>> CatalogBrandsAsync() =>
        _catalogReader.ReadBrandsAsync();

    [HttpGet("types")]
    [ProducesResponseType(typeof(List<IdNameViewModel>), (int)HttpStatusCode.OK)]
    public Task<List<IdNameViewModel>> CatalogTypesAsync() =>
        _catalogReader.ReadTypesAsync();

    [HttpGet("items/by_ids")]
    [ProducesResponseType(typeof(List<ItemViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<List<ItemViewModel>>> ItemsAsync([FromQuery] string ids)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));
            if (numIds.All(nid => nid.Ok))
            {
                var idsToSelect = numIds.Select(id => id.Value);

                var items = await _catalogReader.ReadAsync(idsToSelect);

                return Ok(items);
            }
        }

        return BadRequest("Ids value is invalid. Must be comma-separated list of numbers.");
    }

    [HttpGet("items/by_page")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel), (int)HttpStatusCode.OK)]
    public async Task<PaginatedItemsViewModel> ItemsAsync(
        [FromQuery] int typeId = -1,
        [FromQuery] int brandId = -1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 0)
    {
        var result = await _catalogReader.ReadAsync(typeId, brandId, pageSize, pageIndex);

        return new PaginatedItemsViewModel(pageIndex, pageSize, result.TotalItems, result.ItemsOnPage);
    }
}
