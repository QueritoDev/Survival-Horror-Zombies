using Raylib_cs;

public class Pistol : Gun
{
    public Pistol()
    {
        Name = "Glock17";
        Icon = Raylib.LoadTexture(Path.Combine("sprites", "Guns", "Weapons", "Pistols", "Glock.png"));
        Grip = TypeGrip.Pistol;
        Damage = 20;
        MagazineSize = 7;
        CurrentAmmo = 7;
        TotalAmmo = 28;
        MaxAmmo = 28;
        TimeBetweenFires = 0.3f;
        Sound_Fire = Raylib.LoadSound(Path.Combine("audio","sfx","Weapons", "Glock-17-HL1", "pistol_fire.wav"));
    }
}