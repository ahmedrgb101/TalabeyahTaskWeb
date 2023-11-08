namespace TalabeyahTaskWeb.Client.Infrastructure.Common;

public static class ApplicationConstants
{
    public static readonly List<string> SupportedImageFormats = new()
    {
        ".jpeg",
        ".jpg",
        ".png"
    };
    public static readonly List<string> SupportedDocumentsFormats = new()
    {
        ".pdf",
        ".doc",
        ".xlx",
        ".xlsx",
        ".docx"
    };
    public static readonly string StandardImageFormat = "image/jpeg";
    public static readonly string StandardDocFormat = "application/pdf";
    public static readonly string StandardExcelFormat = "application/octet-stream";
    public static readonly int MaxImageWidth = 1500;
    public static readonly int MaxImageHeight = 1500;
    public static readonly long MaxAllowedSize = 1000000; // Allows Max File Size of 1 Mb.
}