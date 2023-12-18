using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class GunComponent : WeaponComponent
{
	[Property] public float Damage { get; set; } = 10.0f;
	[Property] public float Spread { get; set; } = 0.5f;
	[Property] public float Recoil { get; set; } = 1.0f;
	[Property] public float Force { get; set; } = 10.0f;
	[Property] public SoundEvent ShootSound { get; set; }
	public override void PrimaryAttack()
	{
		Log.Info( "blam!" );
		var bulletinfo = new BulletInfo()
		{
			Damage = Damage,
			Spread = Spread,
			Position = OwnerInventory.Eye.Transform.Position,
			Direction = OwnerInventory.Eye.Transform.Rotation.Forward,
			Force = Force,
		};
		Sound.Play( ShootSound, OwnerInventory.Eye.Transform.Position );
		Bullet.ShootBullet( bulletinfo );
		base.PrimaryAttack();
	}
}
