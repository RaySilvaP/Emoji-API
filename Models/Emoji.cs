using System.Xml.Serialization;

namespace ToEmoji.Models;

public class Emoji
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string[] Codenames { get; set; } = [];
}