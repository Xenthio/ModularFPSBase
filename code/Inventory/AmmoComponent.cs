﻿namespace FPSKit;

public class AmmoComponent : Component
{

	public IList<int> Ammo = new List<int>();
	public void ClearAmmo()
	{
		Ammo.Clear();
	}

	public int AmmoCount( AmmoType type )
	{
		var iType = (int)type;
		if ( Ammo == null ) return 0;
		if ( Ammo.Count <= iType ) return 0;

		return Ammo[(int)type];
	}

	public bool SetAmmo( AmmoType type, int amount )
	{
		var iType = (int)type;
		if ( Ammo == null ) return false;

		while ( Ammo.Count <= iType )
		{
			Ammo.Add( 0 );
		}

		Ammo[(int)type] = amount;
		return true;
	}

	public int GiveAmmo( AmmoType type, int amount )
	{
		if ( Ammo == null ) return 0;
		if ( type == AmmoType.None ) return 0;

		var total = AmmoCount( type ) + amount;
		var max = MaxAmmo( type );

		if ( total > max ) total = max;
		var taken = total - AmmoCount( type );

		SetAmmo( type, total );
		return taken;
	}

	public int TakeAmmo( AmmoType type, int amount )
	{
		if ( Ammo == null ) return 0;

		var available = AmmoCount( type );
		amount = Math.Min( available, amount );

		SetAmmo( type, available - amount );
		return amount;
	}

	public int MaxAmmo( AmmoType ammo )
	{
		switch ( ammo )
		{
			case AmmoType.Pistol: return 250;
			case AmmoType.SMG: return 36;
			case AmmoType.Buckshot: return 100;
			case AmmoType.Rifle: return 40;
		}

		return 0;
	}
}

public enum AmmoType
{
	None,
	Pistol,
	SMG,
	Buckshot,
	Rifle,

}
