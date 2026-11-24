namespace MarketStockTracker.Models;

public class StockBatchModel
{
    public int ArrivalDay { get; set; }
    public int TotalAmount { get; set; }
    public int ExpirationDay { get; set; }
    public string? ProductName { get; set; }
    
    public override string ToString() => $"Product: {ProductName}, Total Amount: {TotalAmount}, SKT: {ExpirationDay}, Arrival Day: {ArrivalDay}";
}