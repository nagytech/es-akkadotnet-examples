public class AlterItemQuantityCommand
{
    public string ItemId { get; }
    public decimal Delta { get; }
    public AlterItemQuantityCommand(string itemId, decimal delta) =>
        (ItemId, Delta) =
        (itemId, delta);
}

public class GetCartCommand
{

}
