using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnDapr.Services.Catalog.API.Model;
using Microsoft.eShopOnDapr.Services.Catalog.API.ViewModel;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogDbContext _context;

        public CatalogController(CatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet("brands")]
        [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogBrand>>> CatalogBrandsAsync()
        {
            return await _context.CatalogBrands.ToListAsync();
        }

        [HttpGet("types")]
        [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogType>>> CatalogTypesAsync()
        {
            return await _context.CatalogTypes.ToListAsync();
        }

        [HttpGet("items/by_ids")]
        [ProducesResponseType(typeof(List<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ItemsAsync([FromQuery] string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));
                if (numIds.All(nid => nid.Ok))
                {
                    var idsToSelect = numIds.Select(id => id.Value);

                    var items = await _context.CatalogItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

                    return Ok(items);
                }
            }

            return BadRequest("ids value invalid. Must be comma-separated list of numbers");
        }        

        [HttpGet("items/by_page")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsAsync(
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
                .ToListAsync();

            return new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
        }
    }
}
