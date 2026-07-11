namespace IdentityAccess.Infrastructure.Tests.TestHelpers;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class IdentityAccessDatabaseCollection
    : ICollectionFixture<IdentityAccessDatabaseFixture>
{
    public const string Name = "IdentityAccess database";
}
