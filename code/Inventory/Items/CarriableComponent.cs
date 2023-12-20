using Sandbox.Citizen;

namespace FPSKit;

public class CarriableComponent : Component
{
	[Property] public CitizenAnimationHelper.HoldTypes HoldType { get; set; }
	[Property] public Model Viewmodel { get; set; }
	public InventoryComponent OwnerInventory;
	public void TriggerAttack()
	{
		OwnerInventory.TriggerAttack();
	}
	public void CreateParticle( ParticleSystem particle )
	{
		OwnerInventory.CreateParticle( particle );
	}
	public virtual void FixedCarriableUpdate()
	{

	}
	public virtual void CarriableUpdate()
	{

	}
}
