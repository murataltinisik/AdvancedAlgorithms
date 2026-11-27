using CashRegisterChangeCalculater.Models;

List<Banknote> banknotes = new List<Banknote>();
banknotes.Add(new Banknote() { Type = 20000, Quantity = 5 });
banknotes.Add(new Banknote() { Type = 10000, Quantity = 5 });
banknotes.Add(new Banknote() { Type = 5000, Quantity = 5 });
banknotes.Add(new Banknote() { Type = 2000, Quantity = 5 });
banknotes.Add(new Banknote() { Type = 1000, Quantity = 5 });
banknotes.Add(new Banknote() { Type = 500, Quantity = 5 });
banknotes.Add(new Banknote() { Type = 100, Quantity = 10 });
banknotes.Add(new Banknote() { Type = 50, Quantity = 10 });
banknotes.Add(new Banknote() { Type = 25, Quantity = 10 });
banknotes.Add(new Banknote() { Type = 10, Quantity = 10 });
banknotes.Add(new Banknote() { Type = 5, Quantity = 10 });

Console.WriteLine("============ Cash Register ============");
Console.WriteLine("1) Calculating Change");
Console.WriteLine("2) Show Case Status");
Console.WriteLine("0) Logout");
Console.WriteLine("=======================================");

string[] optionsNums = { "1", "2", "0" }; 

do
{
    Console.Write("Please, Select: ");
    string? optionStr = Console.ReadLine();

    if (string.IsNullOrEmpty(optionStr))
    {
        Console.WriteLine("Cannot option empty or null!");
        continue;
    }

    if (!optionsNums.Any(x => x == optionStr))
    {
        Console.WriteLine("Please, enter valid option");
        continue;
    }

    switch (optionStr)
    {
        case "1":
            Console.Write("Enter, Purchase Amount: ");
            Console.ForegroundColor = ConsoleColor.Green;
            double.TryParse(Console.ReadLine(), out double purchaseAmount);
            Console.ResetColor();
            
            Console.Write("Enter, Amount Paid: ");
            Console.ForegroundColor = ConsoleColor.Green;
            double.TryParse(Console.ReadLine(), out double amountPaid);
            Console.ResetColor();

            if (amountPaid < purchaseAmount)
            {
                Console.WriteLine("The amount paid must be greater than purchase amount!");
                continue;
            }
            
            double amountOfChange = 0.0;
            double difference = (int)Math.Round((amountPaid - purchaseAmount) * 100);

            foreach (Banknote banknote in banknotes.Where(x => x.Quantity > 0).OrderByDescending(x => x.Type).ToList())
            {
                // Calculate Amount Of Change
                double remaining = difference - amountOfChange;

                while (remaining >= banknote.Type && banknote.Quantity > 0)
                {
                    amountOfChange += banknote.Type;
                    banknote.Quantity--;
                    remaining = difference - amountOfChange;
                }
            }

            int paidKurus = (int)Math.Round(amountPaid * 100);
            int remainingToAdd = paidKurus;
            
            foreach (Banknote banknote in banknotes.OrderByDescending(x => x.Type))
            {
                int count = remainingToAdd / banknote.Type;
                
                if (count > 0)
                {
                    banknote.Quantity += count;
                    remainingToAdd -= count * banknote.Type;
                    Console.WriteLine($"{banknote.Type} krs -> added new +{count} (New Stock: {banknote.Quantity})");
                }
            }
            
            break;
        case "2":
            Console.WriteLine("============= Case Status =============");
            double totalCaseAmount = 0;

            foreach (var banknote in banknotes)
            {
                double unitValueTL = banknote.Type / 100.0;
                double subtotalTL = unitValueTL * banknote.Quantity;
                totalCaseAmount += subtotalTL;

                Console.WriteLine($"{banknote.Type,5} krş ({unitValueTL,5:0.##} TL) -> Adet: {banknote.Quantity,2} | Toplam: {subtotalTL,7:0.##} TL");
            }

            Console.WriteLine("---------------------------------------");
            Console.WriteLine($"Total Cash in Register: {totalCaseAmount:0.##} TL");
            Console.WriteLine("=======================================");
            break;
        case "0":
            Console.WriteLine("=========================================");
            Console.WriteLine("========== Logout Successfully ==========");
            Console.WriteLine("=========================================");
            break;
    }
    
} while (true);


