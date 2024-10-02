using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
const string GetGameEndpointName = "GetGame";
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
app.MapGet("games/{id}", (int id) => games.Find(game => game.Id == id))
    .WithName(GetGameEndpointName);

// games post 
app.MapPost("games", (CreateGameDto newGame) =>
{
    GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
    );
    games.Add(game);
    return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
});

// PUT games
app.MapPut("games/{id}", (int id, UpdateGameDto UpdatedGame) =>
{
    var Index = games.FindIndex(game=>game.Id==id);
    games[Index] = new GameDto(
        id,
        UpdatedGame.Name,
        UpdatedGame.Genre,
        UpdatedGame.Price,
        UpdatedGame.ReleaseDate
    );
    return Results.NoContent();
});

app.Run();
