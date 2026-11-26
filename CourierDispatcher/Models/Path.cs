namespace CourierDispatcher.Models;

public class Path
{
    public int A { get; set; }
    public int B { get; set; }
    public int Minute { get; set; }
    
    public override string ToString()
    {
        return $"Path: {A} -> {B} | Duration: {Minute} minutes";
    }
}