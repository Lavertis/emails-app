namespace EmailsApp.Database.Seeders;

public static class DataSeederExtensions
{
    public static async Task SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        var seeder = new DataSeeder(context);
        await seeder.SeedDataAsync();
    }
}