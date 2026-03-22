namespace Application.Contracts.OrderProcessing;

public enum OrderProcessingState
{
    Approved,
    Packing,
    Packed,
    InDelivery,
    Delivered,
}