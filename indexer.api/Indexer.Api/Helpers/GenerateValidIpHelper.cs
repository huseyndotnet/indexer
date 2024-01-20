namespace Indexer.Api.Helpers;

public static class GenerateValidIpHelper
{
    public static string GenerateRandomValidIpAddress()
    {
        var random = new Random();
        int[] ipAddressParts = new int[4];
        ipAddressParts[0] = random.Next(1, 256);

        for (int i = 1; i < 4; i++)
            ipAddressParts[i] = random.Next(256);

        while (ipAddressParts[0] == 127)
            ipAddressParts[0] = random.Next(1, 256);

        return string.Join(".", ipAddressParts);
    }

}