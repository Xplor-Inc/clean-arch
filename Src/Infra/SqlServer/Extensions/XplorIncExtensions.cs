using ShareMarket.Core.Entities.Settings;
using ShareMarket.Core.Enumerations;
using ShareMarket.Core.Interfaces.Utility.Security;

namespace ShareMarket.SqlServer.Extensions;
public static class ShareMarketExtensions
{
    public static void AddInitialData(this ShareMarketContext context, IEncryption encryption)
    {
        context.SeedUsers(encryption);
        context.SeedAppSettings();
    }

    private static void SeedUsers(this ShareMarketContext context, IEncryption encryption)
    {
        if (!context.Users.Any())
        {
            var salt = encryption.GenerateSalt();
            var user = new User
            {
                CreatedById = SystemConstant.SystemUserId,
                CreatedOn = DateTimeOffset.Now.ToIst(),
                EmailAddress = "test@app.com",
                FirstName = "Admin",
                LastName = "User",
                IsActivated = true,
                IsActive = true,
                PasswordHash = encryption.GenerateHash("1qazxsw2", salt),
                PasswordSalt = salt,
                Role = UserRole.Admin,
                SecurityStamp = $"{Guid.NewGuid():N}",
            };
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
    private static void SeedAppSettings(this ShareMarketContext context)
    {
        if (!context.AppConfigs.Any())
        {
            var app = new AppConfig
            {
                CreatedById     = SystemConstant.SystemUserId,
                CreatedOn       = DateTimeOffset.Now.ToIst(),
                ApiKey          = "Z7c7CjISvM5xWSqFylWS0jeYmq9G8Fnl",
                ApiSecretKey    = "BRCCAsbT573VcOtgO6a67yyckUFCvjf5",
                AppName         = "Dashboard",
                Provider        = "Sharekhan",
                RedirectUrl     = "https://localhost:8080/",
            };
            context.Add(app);
            context.SaveChanges();
        }
    }
}