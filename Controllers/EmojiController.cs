using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using ToEmoji.Services;

namespace ToEmoji.Controllers;

public static class EmojiController
{
    public static async Task<IResult> GetEmojis([FromServices] IEmojiService service, string language, int page = 1, int pageSize = 10)
    {
        try
        {
            var skipAmount = pageSize * (page - 1);
            var emojis = await service.GetEmojisAsync(language);
            return Results.Ok(emojis.Skip(skipAmount).Take(pageSize));
        }
        catch (Exception e) when (e is CultureNotFoundException || e is FileNotFoundException)
        {
            return Results.NotFound("Language not found.");
        }
        catch (Exception)
        {
            return Results.StatusCode(500);
        }
    }

    public static async Task<IResult> GetEmoji([FromServices] IEmojiService service, string language, string code)
    {
        try
        {
            var emoji = await service.GetEmojiAsync(language, code);
            if (emoji is null)
                return Results.NotFound("Emoji not found.");
            return Results.Ok(emoji);
        }
        catch (Exception e) when (e is CultureNotFoundException || e is FileNotFoundException)
        {
            return Results.NotFound("Language not found.");
        }
        catch (Exception)
        {
            return Results.StatusCode(500);
        }
    }

    public static async Task<IResult> GetEmojisByQuery([FromServices] IEmojiService service, string language, string query)
    {
        try
        {
            var emojis = await service.GetEmojisAsync(language, query);
            if (emojis is null)
                return Results.NotFound("Emoji not found.");
            return Results.Ok(emojis.Take(10));
        }
        catch (Exception e) when (e is CultureNotFoundException || e is FileNotFoundException)
        {
            return Results.NotFound("Language not found.");
        }
        catch (Exception)
        {
            return Results.StatusCode(500);
        }
    }

    public static async Task<IResult> Translate([FromServices] IEmojiService service, string language, string phrase)
    {
        try
        {
            var words = phrase.Split(' ');
            string translation = "";
            foreach (var word in words)
            {
                var emojis = await service.GetEmojisAsync(language, word);
                var ordered = emojis.Order(new MatchComparer(word));
                var emoji = emojis.FirstOrDefault();
                if (emoji is null || word.Length <= 2)
                {
                    translation += word + " ";
                }
                else
                {
                    translation += emoji.Code + " ";
                }
            }
            return Results.Ok(translation);
        }
        catch (Exception e) when (e is CultureNotFoundException || e is FileNotFoundException)
        {
            return Results.NotFound("Language not found.");
        }
        catch (Exception)
        {
            return Results.StatusCode(500);
        }
    }
}