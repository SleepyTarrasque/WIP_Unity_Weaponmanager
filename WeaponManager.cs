using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
  
    Weapon CurrentWeapon;
    
    [SerializeField] private string weaponName;
    [SerializeField] private Weapon.AmmoType ammo;
    [SerializeField] private Weapon.AmmoPropertyFlags ammoProperty;
    [SerializeField] private float fireRate;
    [SerializeField] private int magazineSize;
    [SerializeField] private int maxMags;
    [SerializeField] private float reloadTime;
    float timeElapsed;
    

//////////////////////////////////
///  P  R  O  P  E  R  T  I  E  S
////////////////////////////////// properties


////////////////////////////////////////
///  M  A  I  N     M  E  T  H  O  D  S
//////////////////////////////////////// main methods
    void Start() 
    {
        CurrentWeapon = new Weapon(weaponName, ammo, ammoProperty, fireRate, magazineSize, maxMags, reloadTime);
        if(Debug.isDebugBuild)
        {
            Debug.Log($"Current weapon is {CurrentWeapon.WeaponName}");
            timeElapsed = 0;
        }
    }
     void Update()
    {
        if(Input.GetMouseButtonDown(0) || CurrentWeapon.isFiring) // Fire
        {
            CurrentWeapon.Fire();
        } 
        CurrentWeapon.FireTimeElapsed += Time.deltaTime;
        if(Input.GetMouseButtonUp(0))
        {
            CurrentWeapon.isFiring = false;
        }
        
        if(Input.GetKeyDown(KeyCode.R) || CurrentWeapon.isReloading) // Reload
        {            
            CurrentWeapon.ReloadWeapon();
            CurrentWeapon.ReloadTimeElapsed += Time.deltaTime;
        }
        if(Debug.isDebugBuild)
        {
            timeElapsed+=Time.deltaTime;
            if(timeElapsed > 10)
            {
                Debug.Log(Time.time);
                timeElapsed-=10;
            }
        } //Output time at 10 second intervals.


        //DEBUG CALLS
        if(Debug.isDebugBuild)
        {
        
        if(Input.GetKeyDown(KeyCode.I)) // CurrentWeapon.ToString
        {
            PrintString();
        }
        }
    }



  


//////////////////////////////////
///  M  E  T  H  O  D  S
////////////////////////////////// methods
    
    

////////////////////////////////////////
///  C  O  N  S  T  R  U  C  T  O  R  S
//////////////////////////////////////// constructors


////////////////////////////////////////
///  D  E  B  U  G     M  E  T  H  O  D  S
//////////////////////////////////////// debug methods

 public void PrintString()
 {      
    Debug.Log(CurrentWeapon.ToString());        
 }

}

public class Weapon
{

    ////////////////////////////////////////
    ///  V  A  R  I  A  B  L  E  S
    //////////////////////////////////////// variables
    
    public float fireTimeElapsed;   
    private float reloadTimeElapsed;
   
    private string weaponName;
    public enum AmmoType 
    {Ammo7_62mm, Ammo50cal, Ammo20mm, Ammo40mm, Ammo105mm, Ammo155mm, AmmoSmoke, AmmoFire, AmmoCold};
    private AmmoType ammoType;
    [Flags] public enum AmmoPropertyFlags
    {Automatic = 1, Explosive = 2, Armorpiercing = 4, Incendiary = 8, Freezing = 16, Smoke = 32}
    private AmmoPropertyFlags ammoProperties;
    private float explosiveRadius;
    public float armorPiercingValue;

    private int damage;
    private float fireRate;
    public static float weaponReadyTimeBase = .5f;
    private float weaponReadyTimeModifier;
    public bool isFiring;
    
    private float recoilDeviation;
    private float recoilAcceleration;

    private int magazineSize;
    private int currentBulletsInMagazine;
    private int currentMagazineCount; //How many magazines available to reload
    private int maxMagazineCount;
    private float reloadTimer;
    private int reloadStage;
    public static int finalReloadStage = 3; //for progressive reloading
    public bool isReloading; 


    ////////////////////////////////////////
    ///  P  R  O  P  E  R  T  I  E  S
    //////////////////////////////////////// properties
    
    public float FireTimeElapsed
    {
        get=>fireTimeElapsed;
        set
        {
            fireTimeElapsed = value;   
        }
    }
    public float ReloadTimeElapsed
    {
        get=>reloadTimeElapsed;
        set
        {
            reloadTimeElapsed = value;   
        }
    }
    public string WeaponName
    {
        get => weaponName;
        set 
        {
            weaponName = value;
        }
    } 
    public AmmoType CurrentAmmoType
    {
        get => ammoType;
        set 
        {
            ammoType = value;
        }
    }
    public AmmoPropertyFlags AmmoProperties
    {
        get => ammoProperties;
        set 
        {
            ammoProperties = value;
        }
    }
    public float ExplosiveRadius
    {
        get => explosiveRadius;
        set
        {
            if(value >= 0) {explosiveRadius = value;}
            else {explosiveRadius = 0;}
        }
    }
    public float ArmorPiercingValue
    {
        get => armorPiercingValue;
        set
        {
            if(value >= 0) {armorPiercingValue = value;}
            else {armorPiercingValue = 0;}
        }
    }
    public int Damage
    {
        get => damage;
        set 
        {
            if(value>=0){damage=value;}
            else {damage = 1;}
        }
    }
    public float FireRate
    {
        get => fireRate;
        set 
        {
            if(value>=0){fireRate=value;}
            else {fireRate = 1;}
        }
    }

    public float WeaponReadyTimeModifier
    {
        get => weaponReadyTimeModifier;
        set
        {
            if(value > 0) {weaponReadyTimeModifier=value;}
            else {weaponReadyTimeModifier=1;}
        }
    }
    public float RecoilDeviation
    {
        get => recoilDeviation;
        set 
        {
            if(value>=0){recoilDeviation=value;}
            else {recoilDeviation = 1;}
        }
    }
    public float RecoilAcceleration
    {
        get => recoilAcceleration;
        set 
        {
            if(value>=0){recoilAcceleration=value;}
            else {recoilAcceleration = 1;}
        }
    }
    public int MagazineSize
    {
        get => magazineSize;
        set 
        {
            if(value>=0){magazineSize=value;}
            else {magazineSize = 1;}
        }
    }
    public int CurrentBulletsInMagazine
    {
        get => currentBulletsInMagazine;
        set
        {
            if(value>=0) {currentBulletsInMagazine = value;}
            else {currentBulletsInMagazine = 0;}
        }
    }
    public int CurrentMagazineCount

    {
        get => currentMagazineCount;
        set
        {
            if(value>=0) {currentMagazineCount = value;}
            else {currentMagazineCount = 0;}
        }
    }

    public int MaxMagazineCount
    {
        get => maxMagazineCount;
        set
        {
            if (value>=0){maxMagazineCount = value;}
            else {maxMagazineCount = 0;}
        }
    }
    public float ReloadTimer
    {
        get => reloadTimer;
        set 
        {
            if(value>=0){reloadTimer=value;}
            else {reloadTimer = 1;}
        }
    }
    public int ReloadStage
    {
        get => reloadStage;
        set 
        {
            if(value>=0){reloadStage=value;}
            else {reloadStage = 0;}
        }
    }


    ////////////////////////////////////////
    ///  M  A  I  N     M  E  T  H  O  D  S
    //////////////////////////////////////// main methods
    

    ////////////////////////////////////////
    ///  M  E  T  H  O  D  S
    //////////////////////////////////////// methods
    
    
    public void Fire()
    {
        if(isReloading && ReloadStage == 0) // Firing can interrupt reloading only before stage 1
        {
            if(Debug.isDebugBuild){Debug.Log($"Reloading interrupted before stage 1.");}
            isReloading = false;
            FireTimeElapsed = -(Weapon.weaponReadyTimeBase + WeaponReadyTimeModifier*FireRate);
            isFiring = true;
        }
        
        

        if(CurrentBulletsInMagazine>0 && fireTimeElapsed>FireRate)
        {           
            if(AmmoProperties.HasFlag(Weapon.AmmoPropertyFlags.Automatic) == true){isFiring = true;}
            CurrentBulletsInMagazine--;
            fireTimeElapsed = 0;
            if(Debug.isDebugBuild){Debug.Log("Bullets in magazine: " + CurrentBulletsInMagazine);}
        }
        else if (CurrentBulletsInMagazine == 0 && Debug.isDebugBuild){Debug.Log("Out of ammo in magazine."); }
        
        
    }
    public void ReloadWeapon()
    { 
        if(Debug.isDebugBuild){if (CurrentMagazineCount == 0) {Debug.Log("No magazines reamaining."); return;}}
        if (CurrentMagazineCount>0 && CurrentBulletsInMagazine!=MagazineSize)
        {  
            if(!isReloading) 
            {
            reloadTimeElapsed = 0;
            GetLastReloadStage();
            isReloading = true;
            if(Debug.isDebugBuild){Debug.Log("Beginning Reload...");}
            }
 
            GetCurrentReloadStage();    
            if(ReloadStage == Weapon.finalReloadStage)
            {
            CurrentBulletsInMagazine = MagazineSize;
            CurrentMagazineCount--;           
            ReloadStage = 0;
            isReloading = false;
            if(Debug.isDebugBuild){Debug.Log($"Reload Complete...\nMagzines remaining: {CurrentMagazineCount}");}
            }
        }

    }

    private void GetCurrentReloadStage()
    {        
        int tmpLastReloadStage = ReloadStage;   
            if (reloadTimeElapsed > ReloadTimer*(.25f) && reloadTimeElapsed < ReloadTimer*(.75f))
            { ReloadStage = 1;}
            else if (reloadTimeElapsed >= ReloadTimer*(.75f) && reloadTimeElapsed < ReloadTimer)
            { ReloadStage = 2;}
            else if (reloadTimeElapsed >= ReloadTimer)
            { ReloadStage = Weapon.finalReloadStage;}

            if(tmpLastReloadStage!=ReloadStage && Debug.isDebugBuild)
            {
                Debug.Log("Current Reload Stage is " + ReloadStage);
            }
    }

    private void GetLastReloadStage()
    {
        switch (ReloadStage)
        {
            case 0:
                reloadTimeElapsed = 0;
                break;

            case 1:
                reloadTimeElapsed = ReloadTimer*(.25f);
                break;

            case 2:
                reloadTimeElapsed = ReloadTimer*(.75f);
                break;
        }
        if(Debug.isDebugBuild){Debug.Log("Last reload stage was " + ReloadStage);}
    }


    ////////////////////////////////////////
    ///  C  O  N  S  T  R  U  C  T  O  R  S
    //////////////////////////////////////// constructors
    public Weapon(string weapName, AmmoType amo, AmmoPropertyFlags amoProperty, float firRate, int magSize, int maxMags, float reloadTime)
    {
        WeaponName = weapName;
        CurrentAmmoType = amo;
        ammoProperties = amoProperty;
        FireRate = firRate;
        MagazineSize = magSize;
        CurrentBulletsInMagazine = magSize;
        MaxMagazineCount = maxMags;
        CurrentMagazineCount = maxMags;
        ReloadTimer = reloadTime;

        switch(amo)
        {
            case AmmoType.Ammo7_62mm:
            {
                Damage = 7;
                WeaponReadyTimeModifier = 0.8f;
                RecoilDeviation = 0.5f;
                RecoilAcceleration = 0.25f;
                ArmorPiercingValue = 1;
                break;
            }
            case AmmoType.Ammo50cal:
            {
                Damage = 17;
                WeaponReadyTimeModifier = 0.7f;
                RecoilDeviation = 0.35f;
                RecoilAcceleration = 0.1f;                
                ArmorPiercingValue = 2f;       
                break;
            }
            case AmmoType.Ammo20mm:
            {
                Damage = 30;
                WeaponReadyTimeModifier = 1f;
                RecoilDeviation = 0.2f;
                RecoilAcceleration = 0.13f;
                ExplosiveRadius = 0.5f;
                ArmorPiercingValue = 4.0f;
                break;
            }
            case AmmoType.Ammo40mm:
            {
                Damage = 30;
                WeaponReadyTimeModifier = 0.8f;
                RecoilDeviation = 0.6f;
                RecoilAcceleration = 0.3f;
                ExplosiveRadius = 4.0f;
                ArmorPiercingValue = 0.7f;
                break;
            }
            case AmmoType.Ammo105mm:
            {
                Damage = 65;
                WeaponReadyTimeModifier = 1.4f;
                RecoilDeviation = 1.0f;
                RecoilAcceleration = 0.6f;
                ExplosiveRadius = 7.0f;
                ArmorPiercingValue = 2.5f;
                break;
            }
            case AmmoType.Ammo155mm:
            {
                Damage = 100;
                WeaponReadyTimeModifier = 2.25f;
                RecoilDeviation = 2.5f;
                RecoilAcceleration = 1f;
                ExplosiveRadius = 15.0f;
                ArmorPiercingValue = 5f;
                break;
            }
            case AmmoType.AmmoFire:
            {
                Damage = 12;
                WeaponReadyTimeModifier = 1.15f;
                RecoilDeviation = 1.5f;
                RecoilAcceleration = 0.3f;
                ExplosiveRadius = 0f;
                ArmorPiercingValue = 0.5f;
                break;
            }
            case AmmoType.AmmoCold:
            {
                Damage = 12;
                WeaponReadyTimeModifier = 1.15f;
                RecoilDeviation = 1.5f;
                RecoilAcceleration = 0.3f;
                ExplosiveRadius = 0f;
                ArmorPiercingValue = 0.5f;
                break;
            }
            case AmmoType.AmmoSmoke:
            {
                Damage = 0;
                WeaponReadyTimeModifier = 0.2f;
                RecoilDeviation = 0.5f;
                RecoilAcceleration = 0.3f;
                ExplosiveRadius = 15.0f;
                ArmorPiercingValue = 0f;
                break;
            }
            
            
        }
    }

    public Weapon()
    {
        WeaponName = "Default Weapon";
        CurrentAmmoType = AmmoType.Ammo7_62mm;
        AmmoProperties |= AmmoPropertyFlags.Automatic;
        FireRate = 0.85f;
        MagazineSize = 200;
        MaxMagazineCount = 4;
        ReloadTimer = 2.5f;
    }

    ////////////////////////////////////////
    ///  D  E  B  U  G     M  E  T  H  O  D  S
    //////////////////////////////////////// debug methods

    public override string ToString()
    {
        return $"Weapon Name: {WeaponName}\nAmmo Type: {CurrentAmmoType}\nAmmo Flags: {ammoProperties}\nFireRate: {fireRate}\n Mag Size:{magazineSize}\nCurrent Bullets: {CurrentBulletsInMagazine}\nMax Mags: {MaxMagazineCount}\nCurrent Mags: {CurrentMagazineCount}\nReload Duration: {ReloadTimer}\nDamage: {Damage}\nReady Time Modifier: {weaponReadyTimeModifier}\nRecoil Deviation: {RecoilDeviation}\nRecoil Accelertaion: {RecoilAcceleration}\nAP Value: {ArmorPiercingValue}";
    }

}