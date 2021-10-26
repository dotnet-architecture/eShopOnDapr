namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly ICatalogService _catalog;
    private readonly IBasketService _basket;

    public BasketController(ICatalogService catalogService, IBasketService basketService)
    {
        _catalog = catalogService;
        _basket = basketService;
    }

    [HttpPost]
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(BasketData), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BasketData>> UpdateAllBasketAsync([FromBody] UpdateBasketRequest data, [FromHeader] string authorization)
    {
        var basket = new BasketData();

        if (data.Items != null && data.Items.Any())
        {
            var catalogItems = await _catalog.GetCatalogItemsAsync(data.Items.Select(x => x.ProductId));

            if (catalogItems == null)
            {
                return BadRequest("Catalog items were not available for the specified items in the basket.");
            }

            var itemsCalculated = data.Items
                    .GroupBy(x => x.ProductId, x => x, (k, i) => new UpdateBasketRequestItemData(k, i.Sum(j => j.Quantity)));

            foreach (var bitem in itemsCalculated)
            {
                var catalogItem = catalogItems.SingleOrDefault(ci => ci.Id == bitem.ProductId);

                if (catalogItem == null)
                {
                    return BadRequest($"Basket refers to a non-existing catalog item ({bitem.ProductId})");
                }

                basket.Items.Add(new BasketDataItem(
                    catalogItem.Id, 
                    catalogItem.Name, 
                    catalogItem.Price, 
                    bitem.Quantity, 
                    catalogItem.PictureFileName));
            }
        }

        await _basket.UpdateAsync(basket, authorization.Substring("Bearer ".Length));

        return basket;
    }
}
