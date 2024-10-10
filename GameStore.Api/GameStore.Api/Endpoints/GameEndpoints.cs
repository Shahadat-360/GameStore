using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";
    private static List<GameDto> games = [
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

    public static RouteGroupBuilder MapGetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                       .WithParameterValidation();
        // all games 
        group.MapGet("/", () => games);

        // games by Id 
        group.MapGet("/{id}", (int id) =>
        {
            var game = games.Find(game => game.Id == id);
            return game is null? Results.NotFound():Results.Ok(game);
        })
        .WithName(GetGameEndpointName);

        // games post 
        group.MapPost("/", (CreateGameDto newGame,GameStoreDbContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Genre = dbContext.Genres.Find(newGame.GenreId),
                Price = newGame.Price,
                ReleaseDate=newGame.ReleaseDate
            };
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            GameDto gameDto = new(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate
            );
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDto);
        });

        // PUT games
        group.MapPut("/{id}", (int id, UpdateGameDto UpdatedGame) =>
        {
            var Index = games.FindIndex(game => game.Id == id);

            if (Index == -1) return Results.NotFound();
            games[Index] = new GameDto(
                id,
                UpdatedGame.Name,
                UpdatedGame.Genre,
                UpdatedGame.Price,
                UpdatedGame.ReleaseDate
            );
            return Results.NoContent();
        });

        // DELETE game 
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });

        return group;
    }
}
