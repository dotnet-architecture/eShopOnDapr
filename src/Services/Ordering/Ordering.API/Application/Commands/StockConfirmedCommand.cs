using System.Runtime.Serialization;
using MediatR;

namespace Ordering.API.Application.Commands
{
    public class StockConfirmedCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }

        public StockConfirmedCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}