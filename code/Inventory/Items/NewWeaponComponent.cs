namespace FPSKit;


internal class NewWeaponComponent : WeaponComponent
{
	[Property, Group( "Primary Attack" )] public Action<GameObject> PrimaryAttackAction { get; set; }

	/// <param name="Position"></param>
	[Property, Group( "Secondary Attack" )] public Action<GameObject> SecondaryAttackAction { get; set; }




	[Title( "Shoot Bullet" ), Category( "Weapons" )]
	public void ShootBulletAction( Vector3 Position, Vector3 Direction, float Damage, float Force, float Count )
	{
	}

	public void TakeAmmo( int ammo )
	{

	}
}
