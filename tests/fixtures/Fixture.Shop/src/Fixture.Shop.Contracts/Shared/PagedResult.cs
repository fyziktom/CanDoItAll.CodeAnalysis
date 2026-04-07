namespace Fixture.Shop.Contracts.Shared;

public sealed partial record PagedResult<TItem>(
    IReadOnlyList<TItem> Items,
    int TotalCount);
