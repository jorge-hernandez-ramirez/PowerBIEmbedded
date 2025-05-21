namespace PowerBIEmbedded.Models;

public class ReportEmbeddedConfiguration
{
    public string ReportId   { get; set; } = string.Empty;
    public string EmbeddedUrl   { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string TokenExpiration { get; set; } = string.Empty;
}
