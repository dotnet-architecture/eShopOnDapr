// TODO
//namespace Microsoft.eShopOnDapr.Services.Ordering.API.ViewModel;

//public record OrderViewModel(
//    int OrderNumber,
//    DateTime Date,
//    string Status,
//    string Description,
//    string Street,
//    string City,
//    string Country,
//    List<OrderItemViewModel> OrderItems,
//    decimal Subtotal, // TODO Remove subtotal
//    decimal Total)
//{
//    public static OrderViewModel FromOrder(Order order) => new OrderViewModel(
//        order.OrderNumber,
//        order.OrderDate,
//        order.OrderStatus,
//        order.Description,
//        order.Address.Street,
//        order.Address.City,
//        order.Address.Country,
//        order.OrderItems
//            .Select(OrderItemViewModel.FromOrderItem)
//            .ToList(),
//        order.GetTotal(),
//        order.GetTotal());
//}
