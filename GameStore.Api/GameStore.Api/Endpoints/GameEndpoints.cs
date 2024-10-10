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
    
    public static RouteGroupBuilder MapGetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                       .WithParameterValidation();
        // all games 
        group.MapGet("/", (GameStoreDbContext dbContext) => 
            dbContext.Games
                        .Include(game=>game.Genre)
                        .Select(game=>game.ToGameSummaryDto())
                        .AsNoTracking());

        // games by Id 
        group.MapGet("/{id}", (int id,GameStoreDbContext dbContext) =>
        {
            Game? game = dbContext.Games.Find(id);
            return game is null? 
                Results.NotFound():Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);

        // games post 
        group.MapPost("/", (CreateGameDto newGame,GameStoreDbContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(
                GetGameEndpointName, 
                new { id = game.Id }, 
                game.ToGameDetailsDto());
        });

        // PUT games
        group.MapPut("/{id}", (int id, UpdateGameDto UpdatedGame,GameStoreDbContext dbContext) =>
        {
            var ExistingGame = dbContext.Games.Find(id);
            if (ExistingGame is null) return Results.NotFound();
            dbContext.Entry(ExistingGame)
                        .CurrentValues
                        .SetValues(UpdatedGame.ToEntity(id));
            dbContext.SaveChanges();
            return Results.NoContent();
        });

        // DELETE game 
        group.MapDelete("/{id}", (int id,GameStoreDbContext dbContext) =>
        {
            dbContext.Games
                     .Where(game => game.Id == id)
                     .ExecuteDelete();
            return Results.NoContent();
        });

        return group;
    }
}
