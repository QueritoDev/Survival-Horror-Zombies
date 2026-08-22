using Raylib_cs;

public class Weapon
{
    public Item itemData; // Weapon "tem um" Item, não "é um" Item

    public int damage;
    public int magazineSize;
    public int currentAmmo;

    public Weapon(string name, int _damage, int _magazineSize, Texture2D icon)
    {
        itemData = new Item(name, ItemType.Weapon, 1, icon);
        damage = _damage;
        magazineSize = _magazineSize;
        currentAmmo = _magazineSize;
    }
}