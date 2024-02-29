using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ToEmoji.Controllers;

public class EmojiController
{
    public static async Task<IQueryable<Emoji>> GetEmojisAsync(string language)
    {
        var culture = CultureInfo.CreateSpecificCulture(language);
        if (culture.Name == string.Empty)
            throw new CultureNotFoundException();
        var fileName = Path.Combine("./", "Data", "annotations", culture.Parent + ".xml");
        var reader = XmlReader.Create(fileName, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            Async = true
        });

        List<Emoji> emojis = [];
        Emoji emoji = new();
        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
            {
                var cp = reader.GetAttribute("cp");
                var type = reader.GetAttribute("type");
                if (cp is not null && type is null)
                {
                    emoji.Code = cp;
                    await reader.ReadAsync();
                    var codenames = await reader.GetValueAsync();
                    emoji.Codenames = codenames.Split(" | ");
                }
                else if (cp is not null && type is not null)
                {
                    await reader.ReadAsync();
                    emoji.Name = await reader.GetValueAsync();
                    emojis.Add(emoji);
                    emoji = new();
                }
            }
        }
        return emojis.AsQueryable();
    }

    public static async Task<IQueryable<Emoji>> GetEmojisAsync(string name, string language)
    {
        var emojis = await GetEmojisAsync(language);
        emojis = emojis.Where(e => Regex.IsMatch(e.Name, name, RegexOptions.IgnoreCase) || e.Codenames.Contains(name));
        return emojis;
    }

    public static async Task<Emoji?> GetEmojiAsync(string name, string language)
    {
        var emojis = await GetEmojisAsync(language);
        var emoji = emojis
            .Where(e => Regex.IsMatch(e.Name, name, RegexOptions.IgnoreCase))
            .FirstOrDefault()
            ??
            emojis.Where(e => e.Codenames.Contains(name)).FirstOrDefault();
        return emoji;
    }
}