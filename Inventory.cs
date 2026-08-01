using Raylib_cs;
public class Inventory
{
    const int GRID_ROWS = 4;
    const int GRID_COLS = 4;
    const int COMBINE_ROWS = 2;
    const int COMBINE_COLS = 4;
    Item[,] itemGrid = new Item[GRID_ROWS, GRID_COLS];
    Item[,] combineGrid = new Item[COMBINE_ROWS, COMBINE_COLS];
    
    bool IsCombinable(ItemType type)
    {
    return type == ItemType.Herb || type == ItemType.Powder;
    // adicione outros tipos combináveis conforme for criando
    }

    public Inventory()
    {
        for(int line = 0; line < GRID_ROWS; line++)
        {
            for(int colum = 0; colum < GRID_COLS; colum++)
            {
                itemGrid[line, colum] = new Item("", ItemType.None, 0, default);
                Raylib.DrawText($"{itemGrid}", 400, 100, 14, Color.Blue);
            }
        }
        AddStartingItems();
    }

    void AddStartingItems() //Initial items of Player
    {
        Texture2D pistolTexture = Raylib.LoadTexture(Path.Combine("sprites", "Guns", "weapon_pistolTexture.png"));
        Item pistol = new Item("Pistola", ItemType.Weapon, 1, pistolTexture);
        AddItem(pistol);
    }

    public Item this[int line, int colum, bool fromCombine = false]
    {
        get => fromCombine ? combineGrid[line, colum] : itemGrid[line,colum];
    }
    public bool AddItem(Item newItem)
    {
        if(IsCombinable(newItem.type))
            return AddToGrid(newItem, combineGrid, COMBINE_ROWS, COMBINE_COLS);
        else
            return AddToGrid(newItem, itemGrid, GRID_ROWS, GRID_COLS);
    }

    public bool AddToGrid(Item newItem, Item[,] grid, int _ROWS, int _COLS)
    {
        
        for(int line = 0; line < _ROWS; line++)
        {
            for(int colum = 0; colum < _COLS; colum++)
            {
                if(grid[line, colum].type == ItemType.None)
                {
                    grid[line, colum] = newItem; // encontrou slot vazio, coloca aqui
                    return true; // avisa que deu certo
                }
            }
        }
        return false; // percorreu tudo e não achou vazio - inventário cheio
    }

    public void RemoveItem(int line, int colum, bool fromCombine = false)
    {
        Item empty = new Item("", ItemType.None, 0, default);
        if(fromCombine)
            combineGrid[line, colum] = empty;
        else
            itemGrid[line, colum] = empty;
    }
}