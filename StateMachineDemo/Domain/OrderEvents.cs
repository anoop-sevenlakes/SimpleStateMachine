namespace StateMachineDemo.Domain
{
    public class OrderEvents
    {
        public const string CreditCardApproved = "CreditCardApprovedEvent";
        public const string CreditCardDenied = "CreditCardDeniedEvent";
        public const string OrderCancelledByCustomer = "OrderCancelledByCustomerEvent";
        public const string OrderLost = "OrderLostEvent";
        public const string OrderPlaced = "OrderPlacedEvent";
        public const string OrderReceived = "OrderReceievedEvent";
        public const string OrderShipped = "OrderShippedEvent";
        public const string OrderStocked = "InventoryAvailableEvent";
        public const string OutOfStock = "InventoryNotAvailableEvent";
    }
}