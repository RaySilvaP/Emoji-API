using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Internal;
using ToEmoji.Models;

namespace ToEmoji.Services;

public static class XmlService
{
    private static readonly string _directory = Path.Combine("./", "Data", "common", "annotations");
    public static IEnumerable<Region> GetRegions()
    {
        var files = Directory.GetFiles(_directory, "*.xml");

        foreach(var file in files)
        {
            var xelement = XElement.Load(file).Element("identity");
            var region = new Region{
                Language = xelement?.Element("language")?.Attribute("type")?.Value,
                Territory = xelement?.Element("territory")?.Attribute("type")?.Value,
            };
            yield return region;
        }
    }

    public static bool RegionExists(string language)
    {
        return GetRegions()
            .Where(r => r.Language == language)
            .FirstOrDefault() is not null;
    }

    public static IEnumerable<Emoji> GetEmojis(string language)
    {
        var culture = CultureInfo.CreateSpecificCulture(language);
        var fileName = Path.Combine("./", "Data", "common", "annotations", culture.Parent + ".xml");
        var reader = XmlReader.Create(fileName, new XmlReaderSettings{DtdProcessing = DtdProcessing.Parse});

        Emoji emoji = new();
        while(reader.Read())
        {
            if(reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
            {
                var cp = reader.GetAttribute("cp");
                var type = reader.GetAttribute("type");
                if(cp is not null && type is null)
                {
                    emoji.Code = cp;
                    reader.Read();
                    emoji.Codenames = reader.Value.Split(" | ");
                }
                else if(cp is not null && type is not null)
                {
                    reader.Read();
                    emoji.Name = reader.Value;
                    yield return emoji;
                }
            }
        }
    }

    public static IEnumerable<Emoji> GetEmojis(string name, string language)
    {
        var emojis = GetEmojis(language)
            .Where(e => Regex.IsMatch(e.Name, name, RegexOptions.IgnoreCase) || e.Codenames.Contains(name));
        return emojis;
    }

    public static Emoji? GetEmoji(string name, string language)
    {
        var emoji = GetEmojis(language)
            .Where(e => Regex.IsMatch(e.Name, name, RegexOptions.IgnoreCase))
            .FirstOrDefault() 
            ?? 
            GetEmojis(language)
                .Where(e => e.Codenames.Contains(name))
                .FirstOrDefault();
        return emoji;
    }
}