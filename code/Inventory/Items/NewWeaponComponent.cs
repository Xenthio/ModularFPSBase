namespace FPSKit;


internal class NewWeaponComponent : WeaponComponent
{
	[Property, Group( "Primary Attack" )] public Action<NewWeaponComponent> PrimaryAttackAction { get; set; }

	/// <param name="Position"></param>
	[Property, Group( "Secondary Attack" )] public Action<NewWeaponComponent> SecondaryAttackAction { get; set; }



	[Title( "Shoot Bullet" )]
	public void ShootBulletAction( Vector3 Position, Vector3 Direction, float Damage, float Force, float Count )
	{
	}

	public void TakeAmmo( int ammo )
	{

	}
}
