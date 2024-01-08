namespace FPSKit;

public class LifeComponent : Component
{
	/// <summary>
	/// How much Health Points this object has
	/// </summary>
	[Property] public float Health { get; set; } = 100.0f;

	/// <summary>
	/// How much Armour Points this object has, just for demo purposes remove if you don't need this
	/// </summary>
	[Property] public float Armour { get; set; } = 35.0f;

	/// <summary>
	/// State of life this object has
	/// </summary>
	[Property] public LifeState LifeState { get; set; } = LifeState.Alive;


	[Property] public bool PhysicsImpact { get; set; } = true;
	public DamageInfo LastDamage;
	public Action<DamageInfo> OnTakeDamage;
	public Action OnKilled;
	public Action OnRespawn;

	public void TakeDamage( DamageInfo info )
	{
		Health = MathF.Max( Health - info.Damage, 0 );
		if ( GameObject.Components.TryGet<Rigidbody>( out var rigidbody ) )
		{
			rigidbody.ApplyImpulseAt( info.Position, info.Force * 4096 );
		}
		if ( OnTakeDamage != null ) OnTakeDamage( info );
		LastDamage = info;
		if ( LifeState == LifeState.Alive && Health <= 0 ) Kill();
	}

	public void Kill()
	{
		LifeState = LifeState.Dead;
		if ( OnKilled != null ) OnKilled();
	}

	public void Respawn()
	{

		LifeState = LifeState.Respawning;
		Health = 100.0f;
		if ( OnRespawn != null ) OnRespawn();
		LifeState = LifeState.Alive;
	}
}

public enum LifeState
{
	Alive,
	Dead,
	Respawning,
	Limbo
}

// Todo use the builtin one when it has more functionality
public struct DamageInfo
{
	public DamageInfo()
	{
	}

	public float Damage { get; set; }
	public ITagSet Tags { get; set; } = new TagSet();
	public int Bone { get; set; }
	public Vector3 Position { get; set; }
	public Vector3 Force { get; set; }
	public GameObject Weapon { get; set; }
	public GameObject Attacker { get; set; }

	public static DamageInfo Generic( float damage )
	{
		var info = new DamageInfo();
		info.Damage = damage;
		return info;
	}
	public static DamageInfo FromBullet( Vector3 position, Vector3 force, float damage )
	{
		var info = new DamageInfo();
		info.Damage = damage;
		info.Tags.Add( "bullet" );
		info.Position = position;
		info.Force = force;
		return info;
	}
	public DamageInfo WithWeapon( GameObject weapon )
	{
		this.Weapon = weapon;
		return this;
	}
	public DamageInfo WithAttacker( GameObject attacker )
	{
		this.Attacker = attacker;
		return this;
	}
	public DamageInfo WithBone( int bone )
	{
		this.Bone = bone;
		return this;
	}
	public DamageInfo WithTag( string tag )
	{
		this.Tags.Set( tag, true );
		return this;
	}
}
