namespace FPSKit;

public class WeaponComponent : CarriableComponent
{
	public override void FixedCarriableUpdate()
	{
		base.FixedCarriableUpdate();
	}
	public override void CarriableUpdate()
	{
		base.CarriableUpdate();

		if ( Input.Pressed( "attack1" ) )
		{
			PrimaryAttack();
		}
		if ( Input.Pressed( "attack2" ) )
		{
			SecondaryAttack();
		}
	}

	public virtual void PrimaryAttack()
	{

	}

	public virtual void SecondaryAttack()
	{

	}
}
