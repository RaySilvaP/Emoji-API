using System.Globalization;
using ToEmoji.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("{language}/emojis", (string language) => {
    if(XmlService.RegionExists(language))
        return Results.Ok(XmlService.GetEmojis(language));
    return Results.NotFound();
});

app.MapGet("{language}/emojis/{name}", (string language, string name) => {
    if(XmlService.RegionExists(language))
    {
        var emoji = XmlService.GetEmoji(name, language);
        if(emoji is not null)
            return Results.Ok(emoji);
    }
    return Results.NotFound();
});

app.MapGet("{language}/emojis/search/{name}", (string language, string name) => {
    if(XmlService.RegionExists(language))
    {
        var emoji = XmlService.GetEmojis(name, language);
        if(emoji is not null)
            return Results.Ok(emoji);
    }
    return Results.NotFound();
});

app.MapGet("/regions", () => {
    return Results.Ok(XmlService.GetRegions());
});

app.Run();