using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class BaseEquippableComponent : Component
{
	public InventoryComponent OwnerInventory;
	public virtual void FixedEquipUpdate()
	{

	}
	public virtual void EquipUpdate() 
	{

	}
}
