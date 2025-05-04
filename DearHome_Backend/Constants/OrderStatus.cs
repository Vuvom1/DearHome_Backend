namespace DearHome_Backend.Constants;

public enum OrderStatus
{
    Placed, 
    WaitForPayment,
    Processing, 
    Shipping,
    Delivered,
    Completed,
    Cancelled,
}
