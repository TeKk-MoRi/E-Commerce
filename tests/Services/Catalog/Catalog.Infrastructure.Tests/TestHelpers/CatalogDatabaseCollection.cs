namespace Catalog.Infrastructure.Tests.TestHelpers;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class CatalogDatabaseCollection
    : ICollectionFixture<CatalogDatabaseFixture>
{
    public const string Name = "Catalog database";
}