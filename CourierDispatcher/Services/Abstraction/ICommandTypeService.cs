namespace CourierDispatcher.Services.Abstraction;

public interface ICommandTypeService
{
    public bool IsValidType(string commandType);
    public bool IsValidProperties(string raw, string commandType);
}