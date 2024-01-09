namespace FPSKit;

public class ViewmodelComponent : Component
{
	[Property] public SkinnedModelRenderer Model { get; set; }
	[Property] public InventoryComponent Inventory { get; set; }
	[Property] public CameraComponent Camera { get; set; }
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		if ( Inventory.ActiveItem == null ) return;
		if ( Inventory.ActiveItem.Components.TryGet<ItemComponent>( out var item ) && item.Viewmodel != null )
		{
			Model.Model = Inventory.ActiveItem.Components.Get<ItemComponent>().Viewmodel;
		}
		else
		{
			Model.Model = null;
		}
	}
}
