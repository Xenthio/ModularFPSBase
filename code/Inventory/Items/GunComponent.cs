﻿namespace FPSKit;

public class GunComponent : WeaponComponent
{
	[Property, Group( "Primary Attack" )] public float PrimaryDamage { get; set; } = 10.0f;
	[Property, Group( "Primary Attack" )] public float PrimarySpread { get; set; } = 0.024f;
	[Property, Group( "Primary Attack" )] public float PrimaryRecoil { get; set; } = 1.0f;
	[Property, Group( "Primary Attack" )] public float PrimaryForce { get; set; } = 10.0f;
	[Property, Group( "Primary Attack" )] public float PrimaryHeadshotMultiplier { get; set; } = 2.0f;
	[Property, Group( "Primary Attack" )] public int PrimaryBulletCount { get; set; } = 1;
	[Property, Group( "Primary Attack" )] public AmmoType PrimaryAmmoType { get; set; }
	[Property, Group( "Primary Attack" )] public SoundEvent PrimaryShootSound { get; set; }
	[Property, Group( "Primary Attack" )] public ParticleSystem PrimaryMuzzleflash { get; set; }


	[Property, Group( "Secondary Attack" )] public float SecondaryDamage { get; set; } = 10.0f;
	[Property, Group( "Secondary Attack" )] public float SecondarySpread { get; set; } = 0.024f;
	[Property, Group( "Secondary Attack" )] public float SecondaryRecoil { get; set; } = 1.0f;
	[Property, Group( "Secondary Attack" )] public float SecondaryForce { get; set; } = 10.0f;
	[Property, Group( "Secondary Attack" )] public float SecondaryHeadshotMultiplier { get; set; } = 2.0f;
	[Property, Group( "Secondary Attack" )] public int SecondaryBulletCount { get; set; } = 1;
	[Property, Group( "Secondary Attack" )] public AmmoType SecondaryAmmoType { get; set; }
	[Property, Group( "Secondary Attack" )] public SoundEvent SecondaryShootSound { get; set; }
	[Property, Group( "Secondary Attack" )] public ParticleSystem SecondaryMuzzleflash { get; set; }

	public override void PrimaryAttack()
	{
		var bulletinfo = new BulletInfo()
		{
			Owner = OwnerInventory.Player.GameObject,
			Damage = PrimaryDamage,
			Spread = PrimarySpread,
			Position = OwnerInventory.Player.Aim.Transform.Position,
			Direction = OwnerInventory.Player.Aim.Transform.Rotation.Forward,
			Force = PrimaryForce,
			HeadshotMultiplier = PrimaryHeadshotMultiplier,
			Count = PrimaryBulletCount,
		};
		Sound.Play( PrimaryShootSound, OwnerInventory.Player.Aim.Transform.Position );
		Bullet.ShootBullet( bulletinfo );
		CreateParticle( PrimaryMuzzleflash );
		TriggerAttack();
		base.PrimaryAttack();
	}
	public override void SecondaryAttack()
	{
		var bulletinfo = new BulletInfo()
		{
			Owner = OwnerInventory.Player.GameObject,
			Damage = SecondaryDamage,
			Spread = SecondarySpread,
			Position = OwnerInventory.Player.Aim.Transform.Position,
			Direction = OwnerInventory.Player.Aim.Transform.Rotation.Forward,
			Force = SecondaryForce,
			HeadshotMultiplier = SecondaryHeadshotMultiplier,
			Count = SecondaryBulletCount,
		};
		Sound.Play( SecondaryShootSound, OwnerInventory.Player.Aim.Transform.Position );
		Bullet.ShootBullet( bulletinfo );
		CreateParticle( SecondaryMuzzleflash );
		TriggerAttack();
		base.SecondaryAttack();
	}
	// how do we want this
	public virtual void Recoil()
	{

	}
}
