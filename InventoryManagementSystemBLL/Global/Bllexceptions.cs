namespace InventoryManagementSystemBLL.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with id '{key}' was not found.") { }
    }

    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string productName, int requested, int available)
            : base($"Cannot complete operation: '{productName}' has only {available} unit(s) in stock, but {requested} were requested.") { }
    }

    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}