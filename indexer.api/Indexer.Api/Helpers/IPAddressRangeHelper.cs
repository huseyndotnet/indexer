using System.Net;
using System.Net.Sockets;


namespace Indexer.Api.Helpers;

public class IPAddressRangeHelper
{
    private readonly IPAddress _start;
    private readonly IPAddress _end;

    public IPAddressRangeHelper(IPAddress start, IPAddress end)
    {
        _start = start ?? throw new ArgumentNullException(nameof(start));
        _end = end ?? throw new ArgumentNullException(nameof(end));
    }

    public static IPAddressRangeHelper Parse(string cidr)
    {
        string[] parts = cidr.Split('/');
        if (parts.Length != 2)
        {
            throw new FormatException("Invalid CIDR notation");
        }

        IPAddress start = IPAddress.Parse(parts[0]);
        int prefixLength = int.Parse(parts[1]);

        if (start.AddressFamily == AddressFamily.InterNetwork)
        {
            uint startValue = BitConverter.ToUInt32(start.GetAddressBytes(), 0);
            uint endValue = startValue + (uint)(Math.Pow(2, 32 - prefixLength) - 1);

            byte[] endBytes = BitConverter.GetBytes(endValue);
            Array.Reverse(endBytes);

            IPAddress end = new IPAddress(endBytes);
            return new IPAddressRangeHelper(start, end);
        }
        else
        {
            throw new NotSupportedException("IPv6 is not supported in this example");
        }
    }

    public bool Contains(IPAddress address)
    {
        if (address.AddressFamily != _start.AddressFamily)
            return false;

        byte[] addressBytes = address.GetAddressBytes();
        byte[] startBytes = _start.GetAddressBytes();
        byte[] endBytes = _end.GetAddressBytes();

        for (int i = 0; i < startBytes.Length; i++)
            if (addressBytes[i] < startBytes[i] || addressBytes[i] > endBytes[i])
                return false;

        return true;
    }
}