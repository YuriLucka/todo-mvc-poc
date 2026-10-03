using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TodoApp.Api.Data;
using TodoApp.Shared;

namespace TodoApp.Api;

public static class UsageEndpoints
{
    private const string CacheKey = "usage";
    private const long MegaByte = 1024 * 1024;

    public static void MapUsageEndpoints(this IEndpointRouteBuilder app)
    {
        // Numeros agregados do Supabase. Cache curto para nao consultar o banco a cada visita.
        app.MapGet("/api/usage", async (AppDbContext db, IMemoryCache cache, IConfiguration config) =>
            await cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                return await MeasureAsync(db, config);
            }));
    }

    private static async Task<UsageDto> MeasureAsync(AppDbContext db, IConfiguration config)
    {
        var databaseBytes = await db.Database
            .SqlQueryRaw<long>("""select pg_database_size(current_database()) as "Value" """)
            .SingleAsync();

        // storage.objects so existe no Supabase; no Postgres local o Storage fica zerado.
        var hasStorage = await db.Database
            .SqlQueryRaw<bool>("""select to_regclass('storage.objects') is not null as "Value" """)
            .SingleAsync();

        long storageBytes = 0;
        var storageObjects = 0;
        if (hasStorage)
        {
            storageBytes = await db.Database
                .SqlQueryRaw<long>("""select coalesce(sum((metadata->>'size')::bigint), 0)::bigint as "Value" from storage.objects""")
                .SingleAsync();
            storageObjects = await db.Database
                .SqlQueryRaw<int>("""select count(*)::int as "Value" from storage.objects""")
                .SingleAsync();
        }

        var todos = await db.Todos.CountAsync();
        var todosDone = await db.Todos.CountAsync(t => t.IsDone);

        return new UsageDto(
            databaseBytes,
            config.GetValue("Usage:DatabaseLimitMb", 500L) * MegaByte,
            storageBytes,
            config.GetValue("Usage:StorageLimitMb", 1024L) * MegaByte,
            storageObjects,
            todos,
            todosDone,
            DateTime.UtcNow);
    }
}
