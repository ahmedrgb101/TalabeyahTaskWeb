namespace TalabeyahTaskWeb.Client.Infrastructure.Common;

public static class ConfigNames
{
#if DEBUG
    public const string ApiBaseUrl = "ApiDevBaseUrl";
#else
    public const string ApiBaseUrl = "ApiBaseUrl";
#endif
    public const string ApiScope = "ApiScope";
}