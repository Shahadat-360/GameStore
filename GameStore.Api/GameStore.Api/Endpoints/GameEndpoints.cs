using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                       .WithParameterValidation();
        // all games 
        group.MapGet("/", async (GameStoreDbContext dbContext) => 
            await dbContext.Games
                        .Include(game=>game.Genre)
                        .Select(game=>game.ToGameSummaryDto())
                        .AsNoTracking()
                        .ToListAsync());

        // games by Id 
        group.MapGet("/{id}", async (int id,GameStoreDbContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);
            return game is null? 
                Results.NotFound():Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);

        // games post 
        group.MapPost("/", async (CreateGameDto newGame,GameStoreDbContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetGameEndpointName, 
                new { id = game.Id }, 
                game.ToGameDetailsDto());
        });

        // PUT games
        group.MapPut("/{id}", async (int id, UpdateGameDto UpdatedGame,GameStoreDbContext dbContext) =>
        {
            var ExistingGame = await dbContext.Games.FindAsync(id);
            if (ExistingGame is null) return Results.NotFound();
            dbContext.Entry(ExistingGame)
                        .CurrentValues
                        .SetValues(UpdatedGame.ToEntity(id));
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE game 
        group.MapDelete("/{id}", async (int id,GameStoreDbContext dbContext) =>
        {
            await dbContext.Games
                     .Where(game => game.Id == id)
                     .ExecuteDeleteAsync();
            return Results.NoContent();
        });

        return group;
    }
}
