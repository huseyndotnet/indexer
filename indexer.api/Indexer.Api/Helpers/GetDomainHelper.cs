using System.Net;


namespace Indexer.Api.Helpers;

public static class GetDomainHelper
{
    public static async Task<string> GetDomainAsync(string ipAddress)
    {
        try
        {
            IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddress);
            return hostEntry.HostName;
        }
        catch (Exception)
        {
            return "Unknown";
        }
    }
}