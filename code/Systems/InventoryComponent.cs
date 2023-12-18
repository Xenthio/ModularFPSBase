using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class InventoryComponent : Component
{
	[Property] public GameObject Eye { get; set; }

	public GameObject ActiveItem;
	public List<GameObject> Items = new List<GameObject>();
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if ( ActiveItem != null && ActiveItem.Components.TryGet<BaseEquippableComponent>( out var activeequippable ) )
		{
			activeequippable.EquipUpdate();
		}
	}
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		var tr = Scene.Trace.Ray( GameObject.Transform.Position, GameObject.Transform.Position ).Radius( 64 ).WithoutTags( "player" ).Run();
		if (tr.Hit && tr.GameObject != null  && tr.GameObject.Components.TryGet<BaseEquippableComponent>(out var equippable))
		{
			if (equippable.OwnerInventory == null) 
			{
				Add(tr.GameObject);
			}
		}
		if (ActiveItem != null && ActiveItem.Components.TryGet<BaseEquippableComponent>( out var activeequippable ) )
		{
			activeequippable.FixedEquipUpdate();
		}
	}
	public void Add(GameObject item)
	{
		item.Parent = GameObject;
		if ( item.Components.TryGet<BaseEquippableComponent>(out var equippable ) )
		{
			equippable.OwnerInventory = this;
		}
		if ( item.Components.TryGet<ModelRenderer>( out var model ) )
		{
			model.Enabled = false;
		}
		Items.Add(item);
		
		ActiveItem = item;
	}
}
