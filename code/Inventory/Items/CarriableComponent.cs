using Sandbox.Citizen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class CarriableComponent : Component
{
	[Property] public CitizenAnimationHelper.HoldTypes HoldType { get; set; }
	public InventoryComponent OwnerInventory;
	public virtual void FixedEquipUpdate()
	{

	}
	public virtual void EquipUpdate() 
	{

	}
}
