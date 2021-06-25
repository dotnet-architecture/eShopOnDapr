// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net.Http;
// using System.Net.Http.Json;
// using System.Threading.Tasks;

// namespace eShopOnDapr.BlazorClient.Basket
// {
//     public interface IBasketStateProvider
//     {
//         IEnumerable<BasketItems> Items { get; }

//         event EventHandler ItemsChanged;

//         void AddItem(int productId);

//         void Refresh();
//     }

//     public class RemoteB : IBasketService
//     {

//     }


//     public class LocalBasket
//     {
//         public string BuyerId { get; set; }

//         public List<BasketItem> Items { get; set; } = new();

//         public int TotalItemCount => Items.Sum(item => item.Quantity);

//         public event EventHandler ItemsChanged;

//         public void AddItem(int productId)
//         {
//             // How to check auth programmatically
//             // https://www.youtube.com/watch?v=N8YoJRV19rw

//             var existingItem = Items.Find(item => item.ProductId == productId);
//             if (existingItem != null)
//             {
//                 existingItem.Quantity += 1;
//             }
//             else
//             {
//                 Items.Add(new BasketItem
//                 {
//                     ProductId = productId,
//                     Quantity = 1
//                 });
//             }

//             OnItemsChanged(EventArgs.Empty);

//             SaveBasket();
//         }

//         public Task SwitchStateProvider(IBasketStateProvider provider)
//         {

//         }

//         private void OnItemsChanged(EventArgs e)
//             => ItemsChanged?.Invoke(this, e);
//     }
// }
