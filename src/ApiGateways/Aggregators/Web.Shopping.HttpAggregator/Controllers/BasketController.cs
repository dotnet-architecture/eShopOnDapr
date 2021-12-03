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
    public async Task<ActionResult<BasketData>> UpdateAllBasketAsync(
        [FromBody] UpdateBasketRequest data,
        [FromHeader] string authorization)
    {
        BasketData basket;

        if (data.Items is null || !data.Items.Any())
        {
            basket = new();
        }
        else
        {
            // Get the item details from the catalog API.
            var catalogItems = await _catalog.GetCatalogItemsAsync(
                data.Items.Select(x => x.ProductId));
            
            if (catalogItems == null)
            {
                return BadRequest(
                    "Catalog items were not available for the specified items in the basket.");
            }

            // Check item availability and prices; store results in basket object.
            basket = CreateValidatedBasket(data.Items, catalogItems);
        }

        // Save the updated shopping basket.
        await _basket.UpdateAsync(basket, authorization.Substring("Bearer ".Length));

        return basket;
    }

    private BasketData CreateValidatedBasket(
        IEnumerable<UpdateBasketRequestItemData> basketItems,
        IEnumerable<CatalogItem> catalogItems)
    {
        var basket = new BasketData();

        var itemsCalculated = basketItems.GroupBy(
            x => x.ProductId,
            x => x,
            (k, i) => new UpdateBasketRequestItemData(k, i.Sum(j => j.Quantity)));

        foreach (var bitem in itemsCalculated)
        {
            var catalogItem = catalogItems.SingleOrDefault(ci => ci.Id == bitem.ProductId);
            if (catalogItem is not null)
            {
                basket.Items.Add(new BasketDataItem(
                    catalogItem.Id,
                    catalogItem.Name,
                    catalogItem.Price,
                    bitem.Quantity,
                    catalogItem.PictureFileName));
            }
        }

        return basket;
    }
}
