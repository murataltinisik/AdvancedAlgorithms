using System.Text.Json;
using MarketStockTracker.Constants;
using MarketStockTracker.Models;

int numberOfProductDestroyed = 0;
List<StockBatchModel> stockBatches = new List<StockBatchModel>();

do
{
    Console.Write($"Please, Enter value: ");
    string? raw = Console.ReadLine();

    if (string.IsNullOrEmpty(raw))
    {
        Console.WriteLine($"Please, enter anything or correct value, for example: {StockCommandType.Inbound} productName=sut amount=10 stk=5 day=1");
        continue;
    }

    string[] rawArray = raw.Split(" ");
    string commandType = rawArray[0].Trim();
    
    if (!IsValidateRaw(raw, commandType))
    {
        Console.WriteLine($"Please, enter anything or correct value, for example: {StockCommandType.Inbound} productName=sut amount=10 stk=5 day=1");
        continue;
    }

    SaleModel saleModel = new SaleModel();
    ReportModel reportModel = new ReportModel();
    DestroyModel destroyModel = new DestroyModel();
    StockBatchModel stockBatch = new StockBatchModel();
    
    foreach (var value in rawArray)
    {
        string[] valueArray = value.Split("=");
        
        for (int i = 0; i < valueArray.Length - 1; i++)
        {
            string fieldName = valueArray[0].ToLower(); 
            string fieldValue = valueArray[1].ToLower(); 
            
            if(commandType == StockCommandType.Inbound){
                switch (fieldName)
                {
                    case "productname":
                        stockBatch.ProductName = fieldValue;
                        break;
                    case "amount":
                        int.TryParse(fieldValue, out int amount);
                        stockBatch.TotalAmount = amount;
                        break;
                    case "stk":
                        int.TryParse(fieldValue, out int expiryDay);
                        stockBatch.ExpirationDay = expiryDay;
                        break;
                    case "day":
                        int.TryParse(fieldValue, out int arrivalDay);
                        stockBatch.ArrivalDay = arrivalDay;
                        break;
                }
            }
            else if (commandType == StockCommandType.Sale)
            {
                switch (fieldName)
                {
                    case "productname":
                        saleModel.ProductName = fieldValue;
                        break;
                    case "amount":
                        int.TryParse(fieldValue, out int amount);
                        saleModel.Amount = amount;
                        break;
                    case "day":
                        int.TryParse(fieldValue, out int day);
                        saleModel.SaleDay = day;
                        break;
                }
            }
            else if (commandType == StockCommandType.Report)
            {
                switch (fieldName)
                {
                    case "day":
                        int.TryParse(fieldValue, out int arrivalDay);
                        reportModel.ArrivalDay = arrivalDay;
                        break;
                    case "stkstartday":
                        int.TryParse(fieldValue, out int stkStartDay);
                        reportModel.StkStartDay = stkStartDay;
                        break;
                    case "stkendday":
                        int.TryParse(fieldValue, out int stkEndDay);
                        reportModel.StkEndDay = stkEndDay;
                        break;
                }
            }
            else if (commandType == StockCommandType.Disposal)
            {
                switch (fieldName)
                {
                    case "day":
                        int.TryParse(fieldValue, out int day);
                        destroyModel.Day = day;
                        break;
                    case "productname":
                        destroyModel.ProductName = fieldValue;
                        break;
                    case "amount":
                        int.TryParse(fieldValue, out int amount);
                        destroyModel.Amount = amount;
                        break;
                }
            }
        }
    }
    
    switch (commandType)
    {
        case StockCommandType.Inbound:
            stockBatches.Add(stockBatch);
            break;
        case StockCommandType.Sale:
            int totalAmount = 0;
            int remainingAmount = 0;
            var matchingBatches = stockBatches
                .Where(x => x.ProductName.Equals(saleModel.ProductName, StringComparison.OrdinalIgnoreCase) && x.TotalAmount > 0)
                .OrderBy(x => x.ExpirationDay)
                .ThenBy(x => x.ArrivalDay);

            foreach (StockBatchModel batchModel in matchingBatches)
            {
                remainingAmount = saleModel.Amount - totalAmount;
                
                if (remainingAmount > batchModel.TotalAmount)
                {
                    totalAmount += batchModel.TotalAmount;
                    batchModel.TotalAmount = 0;
                }
                else if (remainingAmount < batchModel.TotalAmount)
                {
                    totalAmount += remainingAmount;
                    batchModel.TotalAmount -= remainingAmount;
                }
            }
            
            Console.WriteLine($"{StockCommandType.Sale} Product={saleModel.ProductName}, Sold Out={totalAmount}, Out of Stock={saleModel.Amount - totalAmount}");
            break;
        case StockCommandType.Disposal:
            StockBatchModel batch = stockBatches.SingleOrDefault(x =>
                x.ProductName.Contains(destroyModel.ProductName) && x.ArrivalDay == destroyModel.Day);

            if (batch is null)
            {
                Console.WriteLine("Not Found Product, try again!");
                continue;
            }
            if (batch.TotalAmount < destroyModel.Amount)
            {
                numberOfProductDestroyed += batch.TotalAmount;
                batch.TotalAmount = 0;
            }
            else
            {
                numberOfProductDestroyed += destroyModel.Amount;
                batch.TotalAmount -= destroyModel.Amount;    
            }
            
            
            Console.WriteLine($"{StockCommandType.Disposal} Total={numberOfProductDestroyed}");
            break;
        case StockCommandType.Report:
            int reportCount = stockBatches
                .Where(x => x.ExpirationDay <= reportModel.StkEndDay && x.ExpirationDay >= reportModel.StkStartDay)
                .Sum(x => x.TotalAmount);
            Console.WriteLine($"REPORT: Total Product Amount={reportCount}");
            break;
        case StockCommandType.Print:
            Console.WriteLine(JsonSerializer.Serialize(stockBatches, new JsonSerializerOptions { WriteIndented = true }));
            break;
        default:
            Console.WriteLine($"Invalid Command Type, Please, enter: {StockCommandType.Inbound}, {StockCommandType.Sale}, {StockCommandType.Disposal}, {StockCommandType.Report}");
            return;
    }
} while (true);

bool IsValidateRaw(string raw, string commandType)
{
    string rawString = raw.Trim();
    string commandTypeString = commandType.Trim();
    
    if(commandTypeString == StockCommandType.Inbound)
        if (!rawString.Contains("productName") || !rawString.Contains("stk") || !rawString.Contains("amount") || !rawString.Contains("day"))
            return false;

    if (commandTypeString == StockCommandType.Sale)
        if (!rawString.Contains("productName") || !rawString.Contains("amount") || !rawString.Contains("day"))
            return false;
    
    if (commandTypeString == StockCommandType.Report)
        if(!rawString.Contains("day") || !rawString.Contains("stkStartDay") || !rawString.Contains("stkEndDay"))
            return false;
    
    if (commandTypeString == StockCommandType.Disposal) 
        if (!rawString.Contains("day")) 
            return false;
    
    return true;
}
