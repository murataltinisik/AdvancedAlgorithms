
namespace CashRegisterChangeCalculater.Models;

public class Banknote
{
   public int Type { get; set; }
   public int Quantity { get; set; }

   public override string ToString()
   {
      return $"Type: {Type / 100.0}, Quantity: {Quantity}";
   }
}