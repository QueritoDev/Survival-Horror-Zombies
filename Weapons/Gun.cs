using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;


public enum TypeGrip
{
    Pistol,
    LongGun,
    Knife
}



public abstract class Gun
{
    // Base attributes that every weapon will have
    
    public string Name {get; protected set;}
    public Texture2D Icon { get; protected set; }
    public Sound Sound_Fire { get; protected set; }
    private Sound reloadGun = Raylib.LoadSound(Path.Combine("audio","sfx","Weapons", "w_reload.wav"));
    private Sound emptyAmmo = Raylib.LoadSound(Path.Combine("audio","sfx","Weapons", "w_empty.wav"));
    
    public TypeGrip Grip {get; protected set;}
    public int Damage {get; protected set;}
    public int CurrentAmmo {get; protected set;}
    public int MagazineSize {get; protected set;}
    public int TotalAmmo {get; protected set;}
    public int MaxAmmo { get; protected set; }
    
    //Fire rate control
    public float TimeBetweenFires {get; protected set;}
    protected float timerFire = 0f;

    public List<Projetil> Projeteis { get; private set; } = new List<Projetil>();

    /* SWITCH FIRE MODE FEATURE SOON
    public FireMode[] Modes { get; protected set; } = { FireMode.Single };
    public int ModeIndex { get; protected set; } = 0;
    protected int pendingShots = 0;
    public void SwitchFireMode() => ModeIndex = (ModeIndex + 1) % ModeIndex.Length;
    */

    public void Fire(Vector2 origin, Vector2 direction)
    {   
        // Só atira se o tempo de recarga (cooldown) tiver zerado
        if (CurrentAmmo==0 && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Raylib.SetSoundVolume(emptyAmmo, 0.5f);
            Raylib.PlaySound(emptyAmmo);
        }
        
        if (CurrentAmmo > 0 && timerFire <= 0f)
        {
            CurrentAmmo--;
            // Calcula a direção do tiro (do jogador para o mouse)
            PlaySoundFire();
            
            Projeteis.Add(new Projetil(origin, direction));
            timerFire = TimeBetweenFires;
        }
    }
    
    public void PlaySoundFire()
    {
        float randomPitchModifier = Raylib.GetRandomValue(-10, 10) / 100f;
        float newPitch = 1.0f + randomPitchModifier;
        float newVolume = Raylib.GetRandomValue(60, 70) / 100f;

        Raylib.SetSoundPitch(Sound_Fire, newPitch);
        Raylib.SetSoundVolume(Sound_Fire, newVolume);
        Raylib.PlaySound(Sound_Fire);
    }

    public void RestoreMaxAmmo()
    {
        TotalAmmo = MaxAmmo;
    }

    public int GetTotalAmmo()
    {
        return TotalAmmo;
    }

    public void Reload()
    {
        
        int ammoNeeded = MagazineSize - CurrentAmmo; //Calculates how much is needed to fill the magazine
        int ammoToLoad = Math.Min(ammoNeeded, TotalAmmo); // Checks if the inventory has what we need (or uses what remains)
        Raylib.SetSoundVolume(reloadGun, 0.6f);
        
        if(ammoToLoad>0) 
        {
            Raylib.PlaySound(reloadGun);
        }

        CurrentAmmo+= ammoToLoad;
        TotalAmmo -= ammoToLoad;
    }

    public void Update(float deltaTime)
    {
        // Reduz o tempo de espera para o próximo tiro
        if (timerFire > 0) timerFire -= deltaTime;
        
        /*
        FireMode mode = Modes[ModeIndex];

        bool Fired = (mode == FireMode.Full_Auto) 
            ? Raylib.IsMouseButtonDown(MouseButton.Left) 
            : Raylib.IsMouseButtonPressed(MouseButton.Left);
        */
        
        // Atualiza os tiros e remove os que já saíram da tela
        for (int i = Projeteis.Count - 1; i >= 0; i--)
        {
            Projeteis[i].Update(deltaTime);
            if (!Projeteis[i].Active)
            {
                Projeteis.RemoveAt(i);
            }
        }
        Projeteis.RemoveAll(p => !p.Active);
    }

    public virtual void Draw()
    {
        
        foreach (var projetil in Projeteis)
        {
            projetil.Draw();
        }
    }
}