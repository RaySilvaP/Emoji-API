using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ToEmoji.Services;

public static class XmlService
{
    private static readonly string _directory = Path.Combine("./", "Data", "common", "annotations");
    public static async Task<IQueryable<Region>> GetRegionsAsync()
    {
        var files = Directory.GetFiles(_directory, "*.xml");
        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Parse,
            Async = true
        };
        List<Region> regions = [];
        foreach (var file in files)
        {
            var reader = XmlReader.Create(file, settings);
            var xelement = await XElement.LoadAsync(reader, LoadOptions.None, CancellationToken.None);
            xelement = xelement.Element("identity");
            var region = new Region
            {
                Language = xelement?.Element("language")?.Attribute("type")?.Value,
                Territory = xelement?.Element("territory")?.Attribute("type")?.Value,
            };
            regions.Add(region);
        }
        return regions.AsQueryable();
    }
}