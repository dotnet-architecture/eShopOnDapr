using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Models;
using Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Services;

namespace Microsoft.eShopOnDapr.Web.Shopping.HttpAggregator.Controllers
{
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
            var basket = new BasketData();

            if (data.Items != null && data.Items.Any())
            {
                var catalogItems = await _catalog.GetCatalogItemsAsync(data.Items.Select(x => x.ProductId));

                // group by product id to avoid duplicates
                var itemsCalculated = data
                        .Items
                        .GroupBy(x => x.ProductId, x => x, (k, i) => new { productId = k, items = i })
                        .Select(groupedItem =>
                        {
                            var item = groupedItem.items.First();
                            item.Quantity = groupedItem.items.Sum(i => i.Quantity);
                            return item;
                        });

                foreach (var bitem in itemsCalculated)
                {
                    var catalogItem = catalogItems.SingleOrDefault(ci => ci.Id == bitem.ProductId);
                    if (catalogItem == null)
                    {
                        return BadRequest($"Basket refers to a non-existing catalog item ({bitem.ProductId})");
                    }

                    basket.Items.Add(new BasketDataItem()
                    {
                        ProductId = catalogItem.Id,
                        ProductName = catalogItem.Name,
                        PictureFileName = catalogItem.PictureFileName,
                        UnitPrice = catalogItem.Price,
                        Quantity = bitem.Quantity
                    });
                }
            }

            await _basket.UpdateAsync(basket, authorization.Substring("Bearer ".Length));

            return basket;
        }
    }
}