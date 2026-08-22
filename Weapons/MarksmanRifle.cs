using Raylib_cs;
public class MarksmanRifle : Gun
{
    public MarksmanRifle()
    {
        Name = "MK14";
        Icon = Raylib.LoadTexture(Path.Combine("sprites", "Guns", "Weapons", "Assault-Rifle", "MK14.png"));
        Grip = TypeGrip.LongGun;
        Damage = 60;
        MagazineSize = 30;
        CurrentAmmo = 30;
        TotalAmmo = 90;
        MaxAmmo = 90;
        TimeBetweenFires = 0.10f;
        Sound_Fire = Raylib.LoadSound(Path.Combine("audio","sfx","Weapons", "MK14", "MK14_SemiAuto_Fire.wav"));
    }
}