namespace FPSKit;

public class GunComponent : WeaponComponent
{
	[Property, Group( "Primary Attack" )] public float PrimaryDamage { get; set; } = 10.0f;
	[Property, Group( "Primary Attack" )] public float PrimarySpread { get; set; } = 0.024f;
	[Property, Group( "Primary Attack" )] public float PrimaryRecoil { get; set; } = 1.0f;
	[Property, Group( "Primary Attack" )] public float PrimaryForce { get; set; } = 10.0f;
	[Property, Group( "Primary Attack" )] public float PrimaryHeadshotMultiplier { get; set; } = 2.0f;
	[Property, Group( "Primary Attack" )] public int PrimaryBulletCount { get; set; } = 1;
	[Property, Group( "Primary Attack" )] public AmmoType PrimaryAmmoType { get; set; }
	[Property, Group( "Primary Attack" )] public int PrimaryClipMax { get; set; } = 17;
	[Property, Group( "Primary Attack" )] public SoundEvent PrimaryShootSound { get; set; }
	[Property, Group( "Primary Attack" )] public ParticleSystem PrimaryMuzzleflash { get; set; }


	[Property, Group( "Secondary Attack" )] public float SecondaryDamage { get; set; } = 10.0f;
	[Property, Group( "Secondary Attack" )] public float SecondarySpread { get; set; } = 0.024f;
	[Property, Group( "Secondary Attack" )] public float SecondaryRecoil { get; set; } = 1.0f;
	[Property, Group( "Secondary Attack" )] public float SecondaryForce { get; set; } = 10.0f;
	[Property, Group( "Secondary Attack" )] public float SecondaryHeadshotMultiplier { get; set; } = 2.0f;
	[Property, Group( "Secondary Attack" )] public int SecondaryBulletCount { get; set; } = 1;
	[Property, Group( "Secondary Attack" )] public AmmoType SecondaryAmmoType { get; set; }
	[Property, Group( "Secondary Attack" )] public int SecondaryClipMax { get; set; } = 17;
	[Property, Group( "Secondary Attack" )] public SoundEvent SecondaryShootSound { get; set; }
	[Property, Group( "Secondary Attack" )] public ParticleSystem SecondaryMuzzleflash { get; set; }

	public int PrimaryClip = 0;

	public int SecondaryClip = 0;

	protected override void OnStart()
	{
		base.OnStart();
		PrimaryClip = PrimaryClipMax;
		SecondaryClip = SecondaryClipMax;
	}
	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && PrimaryClip > 0;
	}
	public override void PrimaryAttack()
	{
		var bulletinfo = new BulletInfo()
		{
			Owner = Owner.Player.GameObject,
			Damage = PrimaryDamage,
			Spread = PrimarySpread,
			Position = Owner.Player.Aim.Transform.Position,
			Direction = Owner.Player.Aim.Transform.Rotation.Forward,
			Force = PrimaryForce,
			HeadshotMultiplier = PrimaryHeadshotMultiplier,
			Count = PrimaryBulletCount,
		};
		PrimaryClip -= 1;
		Sound.Play( PrimaryShootSound, Owner.Player.Aim.Transform.Position );
		Bullet.ShootBullet( bulletinfo );
		CreateParticle( PrimaryMuzzleflash );
		TriggerAttack();
		base.PrimaryAttack();
	}
	//Owner.Player.Ammo.AmmoCount( PrimaryAmmoType )
	public override bool CanSecondaryAttack()
	{
		return base.CanSecondaryAttack() && SecondaryClip > 0;
	}
	public override void SecondaryAttack()
	{
		var bulletinfo = new BulletInfo()
		{
			Owner = Owner.Player.GameObject,
			Damage = SecondaryDamage,
			Spread = SecondarySpread,
			Position = Owner.Player.Aim.Transform.Position,
			Direction = Owner.Player.Aim.Transform.Rotation.Forward,
			Force = SecondaryForce,
			HeadshotMultiplier = SecondaryHeadshotMultiplier,
			Count = SecondaryBulletCount,
		};
		SecondaryClip -= 1;
		Sound.Play( SecondaryShootSound, Owner.Player.Aim.Transform.Position );
		Bullet.ShootBullet( bulletinfo );
		CreateParticle( SecondaryMuzzleflash );
		TriggerAttack();
		base.SecondaryAttack();
	}

	public void ReloadPrimary()
	{
		PrimaryClip += Owner.Player.Ammo.TakeAmmo( PrimaryAmmoType, PrimaryClipMax - PrimaryClip );
	}
	public void ReloadSecondary()
	{
		SecondaryClip += Owner.Player.Ammo.TakeAmmo( SecondaryAmmoType, SecondaryClipMax - SecondaryClip );
	}
	// how do we want this
	public virtual void Recoil()
	{

	}
}
