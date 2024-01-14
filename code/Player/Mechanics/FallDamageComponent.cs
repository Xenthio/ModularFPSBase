namespace FPSKit;

public class FallDamageComponent : Component
{
	[Property] public PlayerComponent Player { get; set; }

	[Property] public float LethalFallSpeed { get; set; } = 1024;
	[Property] public float SafeFallSpeed { get; set; } = 580;

	[Property] public SoundEvent LandingSound { get; set; }

	float DamageForSpeed => (float)100 / (LethalFallSpeed - SafeFallSpeed); // damage per unit per second.
	float PreviousZVelocity = 0;
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		var FallSpeed = -PreviousZVelocity;
		if ( FallSpeed > (SafeFallSpeed * GameObject.Transform.Scale.z) && Player.Movement.IsOnGround )
		{
			var FallDamage = (FallSpeed - (SafeFallSpeed * GameObject.Transform.Scale.z)) * (DamageForSpeed * GameObject.Transform.Scale.z);
			var info = DamageInfo.Generic( FallDamage ).WithTag( "fall" );
			Player.Life.TakeDamage( info );
			Sound.Play( LandingSound );
		}
		PreviousZVelocity = Player.Movement.Velocity.z;
	}
}
