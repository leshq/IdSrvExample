dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

dotnet ef migrations add InitialAppIdentityMigration -c AppIdentityDbContext -o Data/Migrations/AppMigrations

dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext

dotnet ef database update -c AppIdentityDbContext