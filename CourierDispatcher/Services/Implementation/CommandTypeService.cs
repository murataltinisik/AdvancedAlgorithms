using CourierDispatcher.Enums;
using CourierDispatcher.Services.Abstraction;

namespace CourierDispatcher.Services.Implementation;

public class CommandTypeService : ICommandTypeService
{
    public bool IsValidType(string commandType)
    {
        if (
            commandType != CommandType.City &&
            commandType != CommandType.Path &&
            commandType != CommandType.Order &&
            commandType != CommandType.Courier
        ) return false;

        return true;
    }

    public bool IsValidProperties(string raw, string commandType)
    {
        string rawStr = raw.ToLower().Trim();
        
        if (commandType == CommandType.City)
        {
            if (!rawStr.Contains("numberofdistrict")) return false;
        }
        else if (commandType == CommandType.Order)
        {
            if (!rawStr.Contains("time") && 
                !rawStr.Contains("restaurant") && 
                !rawStr.Contains("customer")
            ) 
                return false;
        }
        else if (commandType == CommandType.Courier)
        {
            if (!rawStr.Contains("no") && 
                !rawStr.Contains("district")
            ) 
                return false;
        }
        else if (commandType == CommandType.Path)
        {
            if (!rawStr.Contains("a") && 
                !rawStr.Contains("b") && 
                !rawStr.Contains("minute")
            ) 
                return false;
        }

        return true;
    }
}