
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ToEmoji.Services;

public class EmojiService : IEmojiService
{
    public async Task<IEnumerable<Emoji>> GetEmojisAsync(string language, string query)
    {
        var emojis = await GetEmojisAsync(language);
        emojis = emojis
            .Where(e => e.Codenames.Any(n => Regex.IsMatch(n, query)));
        return emojis.Order(new MatchComparer(query));
    }

    public async Task<Emoji?> GetEmojiAsync(string language, string code)
    {
        var emojis = await GetEmojisAsync(language);
        return emojis.FirstOrDefault(e => e.Code == code);
    }
    public Task<IEnumerable<Emoji>> GetEmojisAsync(string language)
    {
        var culture = CultureInfo.CreateSpecificCulture(language);
        if (culture.Name == string.Empty)
            throw new CultureNotFoundException("Language not found. Verify the format and acronym.");

        var emojis = LoadEmojis(culture.Parent.Name);
        try
        {
            var regionEmojis = LoadEmojis(culture.Name.Replace('-', '_'));
            regionEmojis.UnionWith(emojis);
            return Task.FromResult(regionEmojis.AsEnumerable());
        }
        catch (FileNotFoundException)
        {
            return Task.FromResult(emojis.AsEnumerable());
        }
    }

    private static HashSet<Emoji> LoadEmojis(string language)
    {
        var filePath = Path.Combine("./", "Data", "annotations", language + ".xml");
        var reader = XmlReader
            .Create(filePath, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                Async = true
            });
        HashSet<Emoji> emojis = [];

        var root = XElement.Load(reader, LoadOptions.None);
        var annotations = root.Element("annotations")?.Elements();
        if (annotations is null)
            return emojis;

        foreach (var annotation in annotations)
        {
            var emoji = new Emoji
            {
                Code = annotation.Attribute("cp")?.Value ?? "",
                Codenames = annotation.Value.Split(" | "),
            };
            emoji.Name = emoji.Codenames[0];
            if (emojis.TryGetValue(emoji, out var e) && annotation.Attribute("type") is not null)
            {
                e.Name = annotation.Value;
            }
            else
            {
                emojis.Add(emoji);
            }
        }
        return emojis;
    }
}

public class MatchComparer(string pattern) : IComparer<Emoji>
{
    private readonly string _pattern = pattern;

    public int Compare(Emoji? x, Emoji? y)
    {
        if (x is null)
            return -1;
        else if (y is null)
            return 1;

        int xDifference = GetBestMatch(x.Codenames);
        int yDifference = GetBestMatch(y.Codenames);
        return xDifference < yDifference ? -1 : xDifference > yDifference ? 1 : 0;
    }

    private int GetBestMatch(string[] strings)
    {
        int difference = -1;
        foreach (var s in strings)
        {
            if (Regex.IsMatch(s, _pattern))
            {
                int currentDiff = s.Length - _pattern.Length;
                if (currentDiff < difference || difference == -1)
                    difference = currentDiff;
            }
        }
        return difference;
    }
}