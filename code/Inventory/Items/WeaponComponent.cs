namespace FPSKit;

public class WeaponComponent : ItemComponent
{

	[Property, Group( "Primary Attack" )] public float TimeBetweenPrimaryAttack { get; set; } = 0.1f;
	[Property, Group( "Primary Attack" )] public bool AutomaticPrimary { get; set; } = false;
	[Property, Group( "Secondary Attack" )] public float TimeBetweenSecondaryAttack { get; set; } = 0.1f;
	[Property, Group( "Secondary Attack" )] public bool AutomaticSecondary { get; set; } = false;

	TimeSince TimeSincePrimaryAttack;
	TimeSince TimeSinceSecondaryAttack;
	public override void FixedCarriableUpdate()
	{
		base.FixedCarriableUpdate();
	}
	public override void CarriableUpdate()
	{
		base.CarriableUpdate();

		if ( WantsPrimaryAttack() && CanPrimaryAttack() )
		{
			TimeSincePrimaryAttack = 0;
			PrimaryAttack();
		}
		if ( WantsSecondaryAttack() && CanSecondaryAttack() )
		{
			TimeSinceSecondaryAttack = 0;
			SecondaryAttack();
		}
	}

	public virtual bool WantsPrimaryAttack()
	{
		return AutomaticPrimary ? Input.Down( "attack1" ) : Input.Pressed( "attack1" );
	}
	public virtual bool WantsSecondaryAttack()
	{
		return AutomaticSecondary ? Input.Down( "attack2" ) : Input.Pressed( "attack2" );
	}
	public virtual bool CanPrimaryAttack()
	{
		return TimeSincePrimaryAttack > TimeBetweenPrimaryAttack;
	}
	public virtual bool CanSecondaryAttack()
	{
		return TimeSinceSecondaryAttack > TimeBetweenSecondaryAttack;
	}

	[Broadcast]
	public virtual void PrimaryAttack()
	{

	}

	[Broadcast]
	public virtual void SecondaryAttack()
	{

	}
}
