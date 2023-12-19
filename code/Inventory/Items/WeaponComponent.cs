using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class WeaponComponent : BaseEquippableComponent
{
	public override void FixedEquipUpdate()
	{
		base.FixedEquipUpdate();

	}
	public override void EquipUpdate()
	{
		base.EquipUpdate();

		if ( Input.Pressed( "attack1" ) )
		{
			Log.Info( "attack!" );
			PrimaryAttack();
		}
	}

	public virtual void PrimaryAttack()
	{

	}

	public virtual void SecondaryAttack() 
	{

	}
}
