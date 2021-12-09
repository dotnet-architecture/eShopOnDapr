using Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.Model;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Data
{
    public interface ICatalogReader
    {
        Task<(long TotalItems, List<ItemViewModel> ItemsOnPage)> ReadAsync(int typeId = -1, int brandId = -1, int pageSize = 10, int pageIndex = 0);
        Task<List<ItemViewModel>> ReadAsync(IEnumerable<int> productIds);
        Task<int?> ReadAvailableStockAsync(int productId);
        Task<List<IdNameViewModel>> ReadBrandsAsync();
        Task<List<IdNameViewModel>> ReadTypesAsync();
    }

    public class CatalogReader : ICatalogReader
    {
        private readonly CatalogDbContext _context;

        public CatalogReader(CatalogDbContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<int?> ReadAvailableStockAsync(int productId)
        {
            var item = await _context.CatalogItems
                .FirstOrDefaultAsync(ci => ci.Id == productId);

            return item?.AvailableStock;
        }

        public Task<List<ItemViewModel>> ReadAsync(IEnumerable<int> productIds)
        {
            return _context.CatalogItems
                .Where(ci => productIds.Contains(ci.Id))
                .Select(item => new ItemViewModel(
                    item.Id,
                    item.Name,
                    item.Price,
                    item.PictureFileName))
                .ToListAsync();
        }

        public async Task<(long TotalItems, List<ItemViewModel> ItemsOnPage)> ReadAsync(int typeId = -1, int brandId = -1, int pageSize = 10, int pageIndex = 0)
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

            return (totalItems, itemsOnPage);
        }

        public async Task<List<IdNameViewModel>> ReadBrandsAsync() => (await _context.CatalogBrands.ToListAsync()).Select(i => new IdNameViewModel(i.Id, i.Name)).ToList();
        public async Task<List<IdNameViewModel>> ReadTypesAsync() => (await _context.CatalogTypes.ToListAsync()).Select(i => new IdNameViewModel(i.Id, i.Name)).ToList();

    }
}
