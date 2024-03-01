using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ToEmoji.Controllers;

public abstract class EmojiController
{
    public static async Task<IQueryable<Emoji>> GetEmojisAsync(string name, string language)
    {
        var emojis = await GetEmojisAsync(language);
        emojis = emojis
            .Where(e => Regex.IsMatch(e.Name, name, RegexOptions.IgnoreCase)
                || e.Codenames.Contains(name));
        return emojis;
    }

    public static async Task<Emoji?> GetEmojiAsync(string code, string language)
    {
        var emojis = await GetEmojisAsync(language);
        return emojis.FirstOrDefault(e => e.Code == code);
    }

    public static async Task<IQueryable<Emoji>> GetEmojisAsync(string language)
    {
        var culture = CultureInfo.CreateSpecificCulture(language);
        if (culture.Name == string.Empty)
            throw new CultureNotFoundException();

        var emojis = await LoadEmojisAsync(culture.Parent.Name);
        try
        {
            var regionEmojis = await LoadEmojisAsync(culture.Name.Replace('-', '_'));
            return regionEmojis.Union(emojis).AsQueryable();
        }
        catch (FileNotFoundException)
        {
            return emojis.AsQueryable();
        }
    }

    private static async Task<HashSet<Emoji>> LoadEmojisAsync(string fileName)
    {
        var filePath = Path.Combine("./", "Data", "annotations", fileName + ".xml");
        var reader = XmlReader
            .Create(filePath, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                Async = true
            });
        HashSet<Emoji> emojis = [];

        var root = await XElement.LoadAsync(reader, LoadOptions.None, CancellationToken.None);
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