using System.Globalization;
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
            catch (CultureNotFoundException)
            {
                return Results.NotFound("Language not found.");
            }
            catch (FileNotFoundException)
            {
                return Results.NotFound("Language not found.");
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        app.MapGet("{language}/emojis/{name}", async (string language, string name) =>
        {
            try
            {
                var emoji = await EmojiController.GetEmojiAsync(name, language);
                if (emoji is null)
                    return Results.NotFound("Emoji not found.");
                return Results.Ok(emoji);
            }
            catch (CultureNotFoundException)
            {
                return Results.NotFound("Language not found.");
            }
            catch (FileNotFoundException)
            {
                return Results.NotFound("Language not found.");
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        app.MapGet("{language}/emojis/search/{name}", async (string language, string name) =>
        {
            try
            {
                var emojis = await EmojiController.GetEmojisAsync(name, language);
                if (emojis is null)
                    return Results.NotFound("Emoji not found.");
                return Results.Ok(emojis);
            }
            catch (CultureNotFoundException)
            {
                return Results.NotFound("Language not found.");
            }
            catch (FileNotFoundException)
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