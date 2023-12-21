namespace FPSKit;
public struct BulletInfo
{
	public BulletInfo()
	{

	}
	public GameObject Owner;
	public GameObject Weapon;
	public Vector3 Position;
	public Vector3 Direction;
	public float Damage;
	public float Spread;
	public float Force;
	public float Count = 1;
	public float HeadshotMultiplier = 2;

	public Action<SceneTraceResult> OnBulletHit;
	public Action<DamageInfo> OnDealDamage;
	public GameObject IgnoreObject;
	public Vector3? TracerPosition;
	public string TracerOverride;
}
public partial class Bullet
{
	public static void ShootBullet( BulletInfo info, int level = 0 )
	{
		for ( int i = 0; i < info.Count; i++ )
		{
			Game.SetRandomSeed( Time.Tick + i );
			var position = info.Position;
			var forward = info.Direction;

			// TODO: Ignore player instead of this
			position += forward * 32;
			forward += Vector3.Random * info.Spread;

			var tr = GameManager.ActiveScene.Trace.Ray( position, position + (forward * 10000) )
				.UseHitboxes()
				//.Ignore( info.Owner )
				//.Ignore( info.IgnoreEntity ) 

				.WithoutTags( "trigger", "skybox", "playerclip", "viewmodel", "physicsshadow", "playershadow" )
				.Run();

			Gizmo.Draw.Line( tr.StartPosition, tr.EndPosition );
			Gizmo.Draw.SolidSphere( tr.EndPosition, 2 );
			if ( tr.Hit )
			{
				tr.Surface.DoBulletImpact( tr );
				var damage = info.Damage;

				if ( tr.Hitbox?.Tags.Has( "head" ) ?? false )
				{
					damage *= info.HeadshotMultiplier;
				}

				var dmgInfo = DamageInfo.FromBullet( tr.HitPosition, forward * info.Force, damage )
					.WithWeapon( info.Weapon )
					.WithBone( tr.Bone )
					.WithAttacker( info.Owner )
					.WithTag( "bullet" );

				if ( tr.GameObject != null )
				{
					if ( tr.GameObject.Components.TryGet<LifeComponent>( out var life ) )
					{
						life.TakeDamage( dmgInfo );
					}
					if ( tr.GameObject.Components.TryGet<Rigidbody>( out var rigidbody ) )
					{
						rigidbody.ApplyImpulseAt( tr.HitPosition, tr.Direction * (info.Force * 4096) );
					}
				}

				if ( info.OnDealDamage != null ) info.OnDealDamage( dmgInfo );
				if ( info.OnBulletHit != null ) info.OnBulletHit( tr );

			}
		}
	}
}
