namespace StateMachineDemo.Domain
{
    public enum OrderStatus
    {
        AwaitingOrder,
        AwaitingPayment,
        AwaitingShipment,
        OnBackorder,
        InTransit,
        OrderComplete,
        OrderCancelled
    }
}