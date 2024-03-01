using System.Xml.Serialization;

namespace ToEmoji.Models;

public class Emoji
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string[] Codenames { get; set; } = [];

    public override bool Equals(object? obj)
    {
        if (obj is not Emoji other)
            return false;
        return Code.Equals(other.Code);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}