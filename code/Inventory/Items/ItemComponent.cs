using Sandbox.Citizen;

namespace FPSKit;

public class ItemComponent : Component
{
	[Property] public CitizenAnimationHelper.HoldTypes HoldType { get; set; }
	[Property] public Model Viewmodel { get; set; }
	public InventoryComponent OwnerInventory;
	public virtual void OnPickup()
	{

	}
	public virtual void OnDrop()
	{

	}
	protected override void OnStart()
	{
		base.OnStart();
		GameObject.Tags.Set( "item", true );
	}
	public void TriggerAttack()
	{
		OwnerInventory.TriggerAttack();
	}
	public void CreateParticle( ParticleSystem particle )
	{
		OwnerInventory.CreateParticle( particle );
	}
	public void PlaySound( SoundEvent sound )
	{
		Sound.Play( sound, OwnerInventory.Eye.Transform.Position );
	}
	public virtual void FixedCarriableUpdate()
	{

	}
	public virtual void CarriableUpdate()
	{

	}
}
