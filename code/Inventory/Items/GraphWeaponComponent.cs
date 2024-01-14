namespace FPSKit;


internal class GraphWeaponComponent : WeaponComponent
{
	[Property, Group( "Primary Attack" )] public Action<GameObject, GraphWeaponComponent> PrimaryAttackAction { get; set; }


	[Property, Group( "Secondary Attack" )] public Action<GameObject, GraphWeaponComponent> SecondaryAttackAction { get; set; }

	public override void PrimaryAttack()
	{
		base.PrimaryAttack();
		if ( PrimaryAttackAction != null ) PrimaryAttackAction.Invoke( GameObject, this );
	}


	[Title( "Shoot Bullet" ), Category( "Weapons" )]
	public void ShootBulletAction( Vector3 Position, Vector3 Direction, float Damage, float Force, float Count )
	{
	}

	public void TakeAmmo( int ammo )
	{

	}
}
