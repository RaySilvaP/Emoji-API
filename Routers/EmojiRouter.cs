using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ToEmoji.Controllers;

namespace ToEmoji.Routers;

public static class EmojiRouter
{
    public static void MapEmojiEndpoints(this WebApplication app)
    {
        app.MapGet("{language}/emojis", async (string language, int page = 1, int pageSize = 10) =>
        {
            try
            {
                var skipAmount = pageSize * (page - 1);
                var emojis = await EmojiController.GetEmojisAsync(language);
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
        });

        app.MapGet("{language}/emojis/{code}", async (string language, string code) =>
        {
            try
            {
                var emoji = await EmojiController.GetEmojiAsync(code, language);
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
        });

        app.MapGet("{language}/emojis/search", async (string language, string name) =>
        {
            try
            {
                var emojis = await EmojiController.GetEmojisAsync(name, language);
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
        });

        app.MapGet("{language}/translate", async (string language, string phrase) =>
        {
            try
            {
                var words = phrase.Split(' ');
                string translation = "";
                foreach (var word in words)
                {
                    var emojis = await EmojiController.GetEmojisAsync(word, language);
                    var emoji = emojis.FirstOrDefault();
                    if (emoji is null || emojis.Count() > 10)
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
        });
    }
}