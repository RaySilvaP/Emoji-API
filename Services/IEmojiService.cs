namespace ToEmoji.Services;

public interface IEmojiService
{
    public Task<IEnumerable<Emoji>> GetEmojisAsync(string language);

    public Task<IEnumerable<Emoji>> GetEmojisAsync(string language, string query);
    public Task<Emoji?> GetEmojiAsync(string language, string code);
}