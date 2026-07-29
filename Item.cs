using Raylib_cs;

public enum ItemType
{
    None,
    Weapon,
    Ammo,
    Herb,
    Powder,
    KeyItem
}

public struct Item
{
    public string name;
    public ItemType type;
    private int quantity;
    public Texture2D icon;

    public int Quantity
    {
        get { return quantity; }
        set { quantity = Math.Max(value, 0);}
    }

    public Item(string _name, ItemType _type, int _quantity, Texture2D _icon)
    {
        name = _name;
        type = _type;
        icon = _icon;
        quantity = 0;
        Quantity = _quantity;
    }
}