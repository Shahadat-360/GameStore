using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
List<GameDto> games = [
    new GameDto(
        1,
        "Asphalt 8",
        "Racing",
        200,
        new DateOnly(2006,10,2)
    ),
    new GameDto(
        2,
        "Asphalt 9",
        "Racing",
        300,
        new DateOnly(2008,10,2)
    ),
    new GameDto(
        3,
        "COD",
        "Fighting",
        700,
        new DateOnly(2010,10,2)
    ),
];

// all games 
app.MapGet("games", () => games);

// games by Id 
app.MapGet("games/{id}", (int id) => games.Find(game => game.Id == id));

app.Run();
