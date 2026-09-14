using System.Net.Http;
using System.Text;
using System.Text.Json;
using InventoryManagementSystemBLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class GeminiChatService : IAiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IDashboardService _dashboardService;

        public GeminiChatService(HttpClient httpClient, IConfiguration configuration, IDashboardService dashboardService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _dashboardService = dashboardService;
        }

        public async Task<string> AskAssistantAsync(string userMessage)
        {
            var apiKey = _configuration["AiSettings:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "يرجى تعيين مفتاح الـ API في ملف appsettings.json أولاً.";
            }

            var model = _configuration["AiSettings:Model"] ?? "gemini-3.5-flash";

            // جلب ملخص فوري عن المخزون لتزويد الذكاء الاصطناعي بسياق البيانات
            string inventoryContext = "بيانات المخزون الحالية غير متوفرة.";
            try
            {
                var summary = await _dashboardService.GetSummaryAsync(5, 5);
                inventoryContext = $@"
- إجمالي عدد المنتجات: {summary.TotalProducts}
- إجمالي كمية المخزون: {summary.TotalStockQuantity}
- المنتجات منخفضة المخزون: {summary.LowStockProductsCount}
- إجمالي المبيعات: {summary.TotalSalesAmount:C2} ({summary.TotalSalesCount} عملية)
- إجمالي المشتريات: {summary.TotalPurchasesAmount:C2} ({summary.TotalPurchasesCount} عملية)
";
            }
            catch
            {
                // إذا لم تتوفر قاعدة البيانات، يستمر المساعد كخبير مخازن عام
            }

            var systemInstruction = $@"أنت مساعد ذكاء اصطناعي VIP خبير في إدارة المخازن والمبيعات (Inventory & Sales Expert) لنظام ERP.
تتحدث باللغة العربية بأسلوب راقٍ، مهني، مباشر، ومفيد جداً.
البيانات الحالية للنظام:
{inventoryContext}

أجب على أسئلة المستخدم باحترافية، وقدم نصائح لإدارة المخزون، تقليل الهالك، تحسين السيولة، وزيادة الأرباح.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { text = systemInstruction + "\n\nسؤال المستخدم: " + userMessage }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                // Fallback to flash-latest or secondary model if needed
                var fallbackModel = _configuration["AiSettings:FallbackModel"] ?? "gemini-2.5-flash";
                var fallbackUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{fallbackModel}:generateContent?key={apiKey}";
                var fallbackResponse = await _httpClient.PostAsync(fallbackUrl, content);

                if (!fallbackResponse.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    return $"فشل الاتصال: {(int)response.StatusCode} - {err}";
                }

                response = fallbackResponse;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var candidate = candidates[0];
                if (candidate.TryGetProperty("content", out var cContent) &&
                    cContent.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0)
                {
                    return parts[0].GetProperty("text").GetString() ?? "لم يتم استلام نص.";
                }
            }

            return "لم يتم استلام رد من نموذج الذكاء الاصطناعي.";
        }
    }
}