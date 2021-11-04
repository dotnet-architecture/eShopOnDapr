namespace Microsoft.eShopOnDapr.BlazorClient.Host;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string RequestId { get; set; } = null!;

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public ErrorModel()
    {
    }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
