namespace Microsoft.eShopOnDapr.Services.Catalog.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly CatalogDbContext _context;

    public CatalogController(CatalogDbContext context)
    {
        _context = context;
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    [HttpGet("brands")]
    [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
    public Task<List<CatalogBrand>> CatalogBrandsAsync() =>
        _context.CatalogBrands.ToListAsync();

    [HttpGet("types")]
    [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
    public Task<List<CatalogType>> CatalogTypesAsync() =>
        _context.CatalogTypes.ToListAsync();

    [HttpGet("items/by_ids")]
    [ProducesResponseType(typeof(List<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<List<CatalogItem>>> ItemsAsync([FromQuery] string ids)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));
            if (numIds.All(nid => nid.Ok))
            {
                var idsToSelect = numIds.Select(id => id.Value);

                var items = await _context.CatalogItems
                    .Where(ci => idsToSelect.Contains(ci.Id))
                    .Select(item => new ItemViewModel(
                        item.Id,
                        item.Name,
                        item.Price,
                        item.PictureFileName))
                    .ToListAsync();

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
        var query = (IQueryable<CatalogItem>)_context.CatalogItems;

        if (typeId > -1)
        {
            query = query.Where(ci => ci.CatalogTypeId == typeId);
        }

        if (brandId > -1)
        {
            query = query.Where(ci => ci.CatalogBrandId == brandId);
        }

        var totalItems = await query
            .LongCountAsync();

        var itemsOnPage = await query
            .OrderBy(item => item.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .Select(item => new ItemViewModel(
                item.Id,
                item.Name,
                item.Price,
                item.PictureFileName))
            .ToListAsync();

        return new PaginatedItemsViewModel(pageIndex, pageSize, totalItems, itemsOnPage);
    }
}
