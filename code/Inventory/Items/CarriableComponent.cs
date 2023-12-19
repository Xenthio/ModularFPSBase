using Sandbox.Citizen;

namespace FPSKit;

public class CarriableComponent : Component
{
	[Property] public CitizenAnimationHelper.HoldTypes HoldType { get; set; }
	public InventoryComponent OwnerInventory;
	public virtual void FixedCarriableUpdate()
	{

	}
	public virtual void CarriableUpdate()
	{

	}
}
