namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface IAiChatService
    {
        Task<string> AskAssistantAsync(string userMessage);
    }
}
