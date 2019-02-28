public class ItemQuantityAlteredEvent
{
    public string ItemId { get; }
    public ItemQuantityAlteredEvent(string itemId) => ItemId = itemId;
}

public class ItemQuantityIncreasedEvent : ItemQuantityAlteredEvent
{
    public decimal IncreaseBy { get; }
    public ItemQuantityIncreasedEvent(string itemId, decimal increaseBy)
        : base(itemId) => IncreaseBy = increaseBy;
}

public class ItemQuantityDecreasedEvent : ItemQuantityAlteredEvent
{
    public decimal DecreaseBy { get; }
    public ItemQuantityDecreasedEvent(string itemId, decimal decreaseBy)
        : base(itemId) => DecreaseBy = decreaseBy;
}

public class ItemAddedEvent : ItemQuantityAlteredEvent
{
    public decimal Quantity { get; }
    public ItemAddedEvent(string itemId, decimal quantity)
        : base(itemId) => Quantity = quantity;
}

public class ItemRemovedEvent : ItemQuantityAlteredEvent
{
    public ItemRemovedEvent(string itemId) : base(itemId)
    {
    }
}
