using Sandbox.Citizen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class InventoryComponent : Component
{
	[Property] public GameObject Eye { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }

	public GameObject ActiveItem;
	public List<GameObject> Items = new List<GameObject>();
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if ( ActiveItem != null && ActiveItem.Components.TryGet<CarriableComponent>( out var activeequippable ) )
		{
			activeequippable.EquipUpdate();
		}
	}
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		var tr = Scene.Trace.Ray( GameObject.Transform.Position, GameObject.Transform.Position ).Radius( 64 ).WithoutTags( "player" ).Run();
		if (tr.Hit && tr.GameObject != null  && tr.GameObject.Components.TryGet<CarriableComponent>(out var equippable))
		{
			if (equippable.OwnerInventory == null) 
			{
				Add(tr.GameObject);
			}
		}
		if (ActiveItem != null && ActiveItem.Components.TryGet<CarriableComponent>( out var activeequippable ) )
		{
			activeequippable.FixedEquipUpdate();
		}

		if ( AnimationHelper is not null && ActiveItem is not null && ActiveItem.Components.TryGet<CarriableComponent>(out var equippableComponent))
		{
			AnimationHelper.HoldType = equippableComponent.HoldType;
		}
		else if ( AnimationHelper is not null )
		{
			AnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
		}
	}
	public void Add(GameObject item)
	{
		item.Parent = GameObject;
		if ( item.Components.TryGet<CarriableComponent>(out var equippable ) )
		{
			equippable.OwnerInventory = this;
		}
		if ( item.Components.TryGet<Collider>( out var collider ) ) collider.Enabled = false;
		if ( item.Components.TryGet<Rigidbody>( out var rigidbody ) ) rigidbody.PhysicsBody.MotionEnabled = false;
		item.Transform.LocalPosition = Vector3.Zero;
		Items.Add(item);
		
		ActiveItem = item;
	}
}
