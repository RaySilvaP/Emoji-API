using ToEmoji.Controllers;
using Microsoft.OpenApi.Models;

namespace ToEmoji.Routers;

public static class EmojiRouter
{
    public static void MapEmojiEndpoints(this WebApplication app)
    {
        app.MapGet("{language}/emojis", EmojiController.GetEmojis)
            .WithName("GetEmojis")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get emojis",
                Description = "Return information about all the emojis"
            });

        app.MapGet("{language}/emojis/{code}", EmojiController.GetEmoji)
            .WithName("GetEmoji")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get emoji information",
                Description = "Return information about a given emoji"
            });

        app.MapGet("{language}/emojis/search", EmojiController.GetEmojisByQuery)
            .WithName("GetEmojisByQuery")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get emojis by query",
                Description = "Return a collection of emojis that matches a query"
            });

        app.MapGet("{language}/translate", EmojiController.Translate)
            .WithName("Translate")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Translate a phrase",
                Description = "Return a translation of a given phrase to emojis"
            });
    }
}