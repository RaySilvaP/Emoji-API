using System.Xml;
using System.Xml.Linq;

namespace ToEmoji.Controllers;

public static class RegionController
{
    public static async Task<IQueryable<Region>> GetRegionsAsync()
    {
        var directory = Path.Combine("./", "Data", "common", "annotations");
        var files = Directory.GetFiles(directory, "*.xml");
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