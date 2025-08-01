namespace LoveForTennis.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool IsDevelopment { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
