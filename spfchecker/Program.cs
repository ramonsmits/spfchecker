using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ARSoft.Tools.Net.Dns;

class Program
{
    private static readonly bool OnlySpf = true;

    static async Task Main(string[] domains)
    {
        foreach (var domain in domains) await ParseDomain(domain).ConfigureAwait(false);

        foreach (var domain in domains)
        {
            var spf = spfsPerDomain[domain];
            foreach (var d in spfsPerDomain.Keys)
            {
                spf = spf.Replace("include:" + d, spfsPerDomain[d]
                    .Substring(7, spfsPerDomain[d].Length - 7 - 5)
                    );
            }
            await WL(spf, ConsoleColor.Blue).ConfigureAwait(false);
        }
    }

    private static string _indent = string.Empty;

    private const string IndentStep = "  ";

    private static Dictionary<string, string> spfsPerDomain = new Dictionary<string, string>();
    private static async Task<int> ParseDomain(string domainname)
    {
        var count = 0;
        await WL($"{domainname}", ConsoleColor.White).ConfigureAwait(false);
        _indent += IndentStep;

        var hit = false;
        var line = string.Empty;
        var records = await GetTxtRecords(domainname).ConfigureAwait(false);
        foreach (var record in records)
        {
            var l = record.TextData;

            if (l.StartsWith("v=spf1")) hit = true;
            if (hit) line += l;
            if (!OnlySpf || hit)
            {
                await WL(l, hit ? ConsoleColor.DarkCyan : ConsoleColor.DarkGray).ConfigureAwait(false);
                spfsPerDomain[domainname] = l;
            }
            if (!l.EndsWith("all")) continue;

            Console.ResetColor();
            hit = false;
            foreach (var domain in ParseInclude(line))
            {
                var childCount = await ParseDomain(domain).ConfigureAwait(false);
                count++;
                count += childCount;
            }
            await WL($"count: {count}", ConsoleColor.Green).ConfigureAwait(false);
            line = string.Empty;
        }
        _indent = _indent.Substring(IndentStep.Length);
        return count;
    }

    private static async Task WL(object instance, ConsoleColor color)
    {
        var current = Console.ForegroundColor;
        Console.ForegroundColor = color;
        await Console.Out.WriteLineAsync(_indent + instance).ConfigureAwait(false);
        Console.ForegroundColor = current;
    }

    private static Task<List<TxtRecord>> GetTxtRecords(string hostname)
    {
        var resolver = new DnsStubResolver();
        return resolver.ResolveAsync<TxtRecord>(hostname, RecordType.Txt);
    }

    private static IEnumerable<string> ParseInclude(string line)
    {
        var includeLength = "include:".Length;

        var start = 0;
        while (-1 != (start = line.IndexOf("include:", start, StringComparison.InvariantCulture)))
        {
            start += includeLength;
            var end = line.IndexOf(" ", start, StringComparison.InvariantCulture);
            var domain = line.Substring(start, end - start);
            yield return domain;
            start = end + 1;
        }
    }

}
