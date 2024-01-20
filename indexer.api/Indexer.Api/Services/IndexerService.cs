using System.Net;

using Ardalis.GuardClauses;

using Indexer.Api.Helpers;
using Indexer.Api.Services.Abstractions;
using Indexer.Api.Options;
using System.Diagnostics;
using System.Net.Mail;


namespace Indexer.Api.Services;

public class IndexerService : IIndexerService
{
    private readonly ILogger<IndexerService> _logger;
    private readonly HttpRequestHelper _httpRequestHelper;
    private readonly InvalidCidrRangesConfig _invalidCidrRangesConfig;
    private readonly List<InvalidCidrRange> _invalidCidrRanges;
    private readonly List<Task> _setTasks;
    private int _totalRunningThreads;


    public IndexerService(
        ILogger<IndexerService> logger,
        HttpRequestHelper httpRequestHelper,
        InvalidCidrRangesConfig invalidCidrRangesConfig)
    {
        _httpRequestHelper = Guard.Against.Null(httpRequestHelper);
        _logger = Guard.Against.Null(logger);
        _invalidCidrRangesConfig = Guard.Against.Null(invalidCidrRangesConfig);
        _invalidCidrRanges = _invalidCidrRangesConfig.InvalidCidrRanges;
        _setTasks = new List<Task>();
        Console.Clear();
    }


    public async Task StartAsync()
    {
        int numberOfSets = 5;
        int threadsPerSet = 51;
        List<Task> setTasks = new List<Task>();

        for (int set = 0; set < numberOfSets; set++)
        {
            int startIpAddress = set * threadsPerSet + 1;
            int endIpAddress = (set + 1) * threadsPerSet;
            setTasks.Add(Task.Run(() => StartIpSearchSetAsync(startIpAddress, endIpAddress)));
        }

        await Task.WhenAll(setTasks);
    }

    private async Task StartIpSearchSetAsync(int startIpAddress, int endIpAddress)
    {
        List<Task> ipSearchTasks = new List<Task>();

        for (int firstOctet = startIpAddress; firstOctet <= endIpAddress; firstOctet++)
            for (int secondOctet = 1; secondOctet <= 255; secondOctet++)
                ipSearchTasks.Add(ProcessIpAddressAsync(firstOctet, secondOctet));

        await Task.WhenAll(ipSearchTasks);
    }

    private async Task ProcessIpAddressAsync(int firstOctet, int secondOctet)
    {
        Random random = new Random();
        string ipAddress = $"{random.Next(1, 256)}.{random.Next(1, 256)}.{random.Next(1, 256)}.{random.Next(1, 256)}";

        while (true)
        {
            if (!IsLocalOrInvalidIpAddress(ipAddress))
            {
                HttpResponseMessage? httpResponse = await _httpRequestHelper.SendHttpRequestAsync(ipAddress);
                string domain = await Indexer.Api.Helpers.GetDomainHelper.GetDomainAsync(ipAddress);

                if (httpResponse is not null && domain != "Unknown")
                    // TODO: aggregate data. Its just example for logging output
                    LogData($"{(int)httpResponse?.StatusCode} FROM: {ipAddress} - {domain}");
            }
            ipAddress = IncrementIpAddress(ipAddress);
        }
    }

    private bool IsLocalOrInvalidIpAddress(string ipAddress)
    {
        if (IPAddress.TryParse(ipAddress, out IPAddress ip))
        {
            if (IPAddress.IsLoopback(ip))
                return true;

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = ip.GetAddressBytes();

                // 192.168.0.0/16
                if (ipBytes[0] == 192 && ipBytes[1] == 168)
                    return true;

                // 10.0.0.0/8
                if (ipBytes[0] == 10)
                    return true;

                // 172.16.0.0 to 172.31.255.255
                if (ipBytes[0] == 172 && ipBytes[1] >= 16 && ipBytes[1] <= 31)
                    return true;
            }

            var invalidCidrArray = _invalidCidrRanges.ToArray();
            foreach (var invalidCidrRange in invalidCidrArray)
                if (IsWithinCidrRange(ipAddress, invalidCidrRange.Range, invalidCidrRange.StartIp, invalidCidrRange.EndIp))
                    return true;
        }

        return false;
    }

    public async Task Stop()
    {
        foreach (var task in _setTasks)
            if (!task.IsCompleted)
                task.Dispose();

        _setTasks.Clear();
    }

    public async Task<bool> Status() => _setTasks.Any(task => !task.IsCompleted);

    private bool IsWithinCidrRange(string ipAddress, string cidr, string firstIp, string lastIp)
    {
        IPAddress targetAddress = IPAddress.Parse(ipAddress);

        var cidrRange = IPAddressRangeHelper.Parse(cidr);
        return cidrRange.Contains(targetAddress);
    }

    private string IncrementIpAddress(string ipAddress)
    {
        string[] ipParts = ipAddress.Split('.');
        int lastIndex = ipParts.Length - 1;

        for (int i = lastIndex; i >= 0; i--)
            if (int.TryParse(ipParts[i], out int value))
            {
                if (value < 255)
                {
                    ipParts[i] = (value + 1).ToString();
                    break;
                }
                else if (value == 255 && i > 0)
                    ipParts[i] = "0";
            }

        return string.Join(".", ipParts);
    }

    private bool IsInRange(IPAddress ipAddress, IPAddress startRange, IPAddress endRange)
    {
        byte[] ipBytes = ipAddress.GetAddressBytes();
        byte[] startRangeBytes = startRange.GetAddressBytes();
        byte[] endRangeBytes = endRange.GetAddressBytes();

        for (int i = 0; i < ipBytes.Length; i++)
        {
            if (ipBytes[i] < startRangeBytes[i] || ipBytes[i] > endRangeBytes[i])
            {
                return false;
            }
        }

        return true;
    }

    private void LogData(string a)
    {
        // Customize this method to log or store relevant data
        _logger.LogInformation($"IP: {a}, Status Code: {a}, Elapsed Time: {a} ms");
    }
}