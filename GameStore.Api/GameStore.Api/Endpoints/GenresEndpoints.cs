using System;
using GameStore.Api.Data;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres");
        group.MapGet("/",async (GameStoreDbContext dbContext)=>
            await dbContext.Genres
                           .Select(genre=>genre.ToDto())
                           .AsNoTracking()
                           .ToListAsync());
        return group;
    }
}
