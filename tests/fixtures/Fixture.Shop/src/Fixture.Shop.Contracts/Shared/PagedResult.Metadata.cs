namespace Fixture.Shop.Contracts.Shared;

public sealed partial record PagedResult<TItem>
{
    public bool HasItems
    {
        get
        {
            return Items.Count > 0;
        }
    }
}
