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
        private readonly IInventoryQueryService _queryService;

        public GeminiChatService(
            HttpClient httpClient,
            IConfiguration configuration,
            IInventoryQueryService queryService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _queryService = queryService;
        }

        public async Task<string> AskAssistantAsync(string userMessage)
        {
            var apiKey = _configuration["AiSettings:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return "يرجى تعيين مفتاح الـ API في ملف appsettings.json أولاً.";

            var model = _configuration["AiSettings:Model"] ?? "gemini-3.6-flash";

            var intent = DetectIntent(userMessage);
            string realData = await FetchRealDataAsync(intent, userMessage);

            var systemInstruction = $@"أنت مساعد ذكاء اصطناعي دقيق ومباشر لنظام ERP لإدارة المخازن.

🚨 قواعد صارمة جداً:
1. اعرض فقط البيانات المطلوبة.
2. ممنوع تكتب: ""للعلم والاطلاع"" أو ""معلومات إضافية"" أو ""ملاحظة"" أو ""هل ترغب"" أو ""يسعدني مساعدتك"" أو ""أهلاً بك"" أو ""في خدمتك"".
3. ابدأ الإجابة مباشرة. متضيفش مقدمات.
4. متضيفش منتجات مش موجودة في البيانات المرفقة.
5. الرد قصير ومباشر (3-5 أسطر max).
6. لو مفيش بيانات → قول ""لا توجد بيانات كافية"".

=== البيانات الحقيقية من قاعدة البيانات ===
{realData}
=== نهاية البيانات ===";

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
                },
                generationConfig = new
                {
                    temperature = 0.1,
                    maxOutputTokens = 1000
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var response = await SendWithRetryAsync(url, json);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;

                if (statusCode == 503 || statusCode == 429)
                {
                    var fallbackModel = _configuration["AiSettings:FallbackModel"] ?? "gemini-3.5-flash";
                    var fallbackUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{fallbackModel}:generateContent?key={apiKey}";
                    var fallbackResponse = await SendWithRetryAsync(fallbackUrl, json);

                    if (fallbackResponse.IsSuccessStatusCode)
                    {
                        response = fallbackResponse;
                    }
                    else
                    {
                        var err = await response.Content.ReadAsStringAsync();
                        return $"فشل الاتصال: {(int)response.StatusCode} - {err}";
                    }
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    return $"فشل الاتصال: {statusCode} - {err}";
                }
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
                    var reply = parts[0].GetProperty("text").GetString() ?? "لم يتم استلام نص.";
                    return TrimExtraContent(reply);
                }
            }

            return "لم يتم استلام رد.";
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(string url, string json)
        {
            var maxRetries = 5;
            var delay = TimeSpan.FromSeconds(1);

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode) return response;

                var statusCode = (int)response.StatusCode;
                if (statusCode == 429 || statusCode >= 500)
                {
                    if (attempt == maxRetries) return response;
                    var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500));
                    await Task.Delay(delay + jitter);
                    delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 60));
                    continue;
                }
                return response;
            }

            var finalContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, finalContent);
        }

        private string TrimExtraContent(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply)) return reply;

            var stopMarkers = new[]
            {
                "للعلم والاطلاع", "للعلم:", "معلومات إضافية", "إجمالي حالة المخزون",
                "ملاحظة:", "*ملاحظة", "هل ترغب", "يسعدني مساعدتك",
                "إذا كان لديك", "للاطلاع فقط", "أهلاً بك", "في خدمتك"
            };

            foreach (var marker in stopMarkers)
            {
                var idx = reply.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx > 0) reply = reply.Substring(0, idx).Trim();
            }

            return reply.TrimEnd('-', '*', '\n', ' ', '\r').Trim();
        }

        private string DetectIntent(string message)
        {
            var msg = message.ToLower().Trim();

            if (msg.Contains("نفد") || msg.Contains("خلص تمام") || msg.Contains("مفيش")
                || msg.Contains("صفر") || msg.Contains("out of stock")
                || msg.Contains("غير متوفر") || msg.Contains("مش موجود"))
                return "out_of_stock";

            if (msg.Contains("منخفض") || msg.Contains("قرب") || msg.Contains("قربت")
                || msg.Contains("أوشك") || msg.Contains("اوشك") || msg.Contains("على وشك")
                || msg.Contains("low stock") || msg.Contains("تنبيه")
                || msg.Contains("تحت الحد") || msg.Contains("الحد الأدنى")
                || msg.Contains("ناقص") || msg.Contains("إعادة طلب"))
                return "low_stock";

            if (msg.Contains("أكثر مبيع") || msg.Contains("اكثر مبيع")
                || msg.Contains("most sold") || msg.Contains("best seller"))
                return "most_sold";

            if (msg.Contains("مبيعات") || msg.Contains("sales")) return "sales";
            if (msg.Contains("مشتريات") || msg.Contains("purchases")) return "purchases";
            if (msg.Contains("مورد") || msg.Contains("supplier")) return "suppliers";
            if (msg.Contains("قسم") || msg.Contains("أقسام") || msg.Contains("اقسام") || msg.Contains("category")) return "categories";
            if (msg.Contains("sku") || msg.Contains("كود")) return "product_by_sku";
            if (msg.Contains("إجمالي") || msg.Contains("اجمالي") || msg.Contains("ملخص") || msg.Contains("summary") || msg.Contains("تقرير")) return "summary";
            if (msg.Contains("منتج") || msg.Contains("products")) return "all_products";

            return "summary";
        }

        private async Task<string> FetchRealDataAsync(string intent, string userMessage)
        {
            try
            {
                var sb = new StringBuilder();

                switch (intent)
                {
                    case "out_of_stock":
                        var outOfStock = await _queryService.GetOutOfStockProductsAsync();
                        sb.AppendLine($"### المنتجات اللي نفدت تماماً ({outOfStock.Count}):");
                        if (outOfStock.Count == 0) sb.AppendLine("لا يوجد منتجات نفدت.");
                        else foreach (var p in outOfStock)
                                sb.AppendLine($"- {p.ProductName} | SKU: {p.SKU} | القسم: {p.CategoryName} | السعر: {p.UnitPrice:C2}");
                        break;

                    case "low_stock":
                        var lowStock = await _queryService.GetLowStockProductsAsync();
                        sb.AppendLine($"### المنتجات منخفضة المخزون ({lowStock.Count}):");
                        if (lowStock.Count == 0) sb.AppendLine("لا يوجد منتجات منخفضة المخزون.");
                        else foreach (var p in lowStock)
                                sb.AppendLine($"- {p.ProductName} | SKU: {p.SKU} | الكمية: {p.StockQuantity} | حد التنبيه: {p.LowStockThreshold} | السعر: {p.UnitPrice:C2}");
                        sb.AppendLine("⚠️ اعرض فقط المنتجات المذكورة أعلاه.");
                        break;

                    case "most_sold":
                        var top = await _queryService.GetMostSoldProductsAsync(10);
                        sb.AppendLine($"### الأكثر مبيعاً ({top.Count}):");
                        foreach (var p in top)
                            sb.AppendLine($"- {p.ProductName} | الكمية: {p.TotalQuantitySold} | الإيراد: {p.TotalRevenue:C2}");
                        break;

                    case "sales":
                        var sales = await _queryService.GetRecentSalesAsync(10);
                        sb.AppendLine($"### آخر {sales.Count} عملية بيع:");
                        foreach (var s in sales)
                            sb.AppendLine($"- فاتورة #{s.Id} | {s.SaleDate:yyyy-MM-dd} | {s.CustomerInfo ?? "نقدي"} | {s.TotalAmount:C2}");
                        break;

                    case "purchases":
                        var purchases = await _queryService.GetRecentPurchasesAsync(10);
                        sb.AppendLine($"### آخر {purchases.Count} عملية شراء:");
                        foreach (var p in purchases)
                            sb.AppendLine($"- فاتورة #{p.Id} | {p.PurchaseDate:yyyy-MM-dd} | {p.SupplierName} | {p.TotalAmount:C2}");
                        break;

                    case "suppliers":
                        var suppliers = await _queryService.GetAllSuppliersAsync();
                        sb.AppendLine($"### الموردين ({suppliers.Count}):");
                        foreach (var s in suppliers)
                            sb.AppendLine($"- {s.SupplierName} | {s.Phone} | {s.Email}");
                        break;

                    case "categories":
                        var categories = await _queryService.GetAllCategoriesAsync();
                        sb.AppendLine($"### الأقسام ({categories.Count}):");
                        foreach (var c in categories)
                            sb.AppendLine($"- {c.CategoryName} | عدد المنتجات: {c.ProductCount}");
                        break;

                    case "product_by_sku":
                        var sku = ExtractSku(userMessage);
                        if (!string.IsNullOrEmpty(sku))
                        {
                            var product = await _queryService.GetProductBySkuAsync(sku);
                            if (product != null)
                                sb.AppendLine($"### تفاصيل المنتج SKU: {sku}\n- الاسم: {product.ProductName}\n- القسم: {product.CategoryName}\n- السعر: {product.UnitPrice:C2}\n- الكمية: {product.StockQuantity}\n- حد التنبيه: {product.LowStockThreshold}");
                            else sb.AppendLine($"لا يوجد منتج بالـ SKU: {sku}");
                        }
                        break;

                    case "all_products":
                        var products = await _queryService.GetAllProductsListAsync();
                        sb.AppendLine($"### كل المنتجات ({products.Count}):");
                        foreach (var p in products)
                            sb.AppendLine($"- {p.ProductName} | SKU: {p.SKU} | كمية: {p.StockQuantity} | سعر: {p.UnitPrice:C2}");
                        break;

                    case "summary":
                    default:
                        var summary = await _queryService.GetFullSummaryAsync();
                        sb.AppendLine("### ملخص النظام:");
                        sb.AppendLine($"- إجمالي المنتجات: {summary.TotalProducts}");
                        sb.AppendLine($"- إجمالي الأقسام: {summary.TotalCategories}");
                        sb.AppendLine($"- إجمالي الموردين: {summary.TotalSuppliers}");
                        sb.AppendLine($"- إجمالي كمية المخزون: {summary.TotalStockQuantity}");
                        sb.AppendLine($"- منتجات منخفضة المخزون: {summary.LowStockProductsCount}");
                        sb.AppendLine($"- إجمالي المبيعات: {summary.TotalSalesAmount:C2}");
                        sb.AppendLine($"- إجمالي المشتريات: {summary.TotalPurchasesAmount:C2}");
                        break;
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"حدث خطأ في جلب البيانات: {ex.Message}";
            }
        }

        private string? ExtractSku(string message)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                message, @"SKU[-_]?\d+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return match.Success ? match.Value : null;
        }
    }
}