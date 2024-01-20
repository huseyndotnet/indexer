namespace Indexer.Api.Options;

public class InvalidCidrRangesConfig
{
    public List<InvalidCidrRange> InvalidCidrRanges { get; set; }
}

public class InvalidCidrRange
{
    public string Range { get; set; }

    public string StartIp { get; set; }

    public string EndIp { get; set; }
}