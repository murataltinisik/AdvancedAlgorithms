namespace CourierDispatcher.Models;

public class Order
{
    public int Time { get; set; }
    public string? Id { get; set; }
    public int Customer { get; set; }
    public int Restaurant { get; set; }
    public bool Status { get; set; } = false;
}