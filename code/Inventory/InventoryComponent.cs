using Sandbox.Citizen;

namespace FPSKit;

public class InventoryComponent : Component
{
	[Property] public PlayerComponent Player { get; set; }

	[Sync] public GameObject ActiveItem { get; set; }
	public List<GameObject> Items = new List<GameObject>();
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if ( Player.Life.LifeState != LifeState.Alive )
			return;
		if ( ActiveItem != null && ActiveItem.Components.TryGet<ItemComponent>( out var activeequippable ) )
		{
			activeequippable.CarriableUpdate();
		}
	}
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		if ( Player.Life.LifeState != LifeState.Alive )
			return;
		TryPickupThings();
		if ( ActiveItem != null && ActiveItem.Components.TryGet<ItemComponent>( out var activeequippable ) )
		{
			ActiveItem.Transform.LocalPosition = Vector3.Zero;
			ActiveItem.Transform.LocalRotation = Rotation.Identity;
			activeequippable.FixedCarriableUpdate();
		}

		if ( Player.Body.Animation is not null && ActiveItem is not null && ActiveItem.Components.TryGet<ItemComponent>( out var equippableComponent ) )
		{
			Player.Body.Animation.HoldType = equippableComponent.HoldType;
		}
		else if ( Player.Body.Animation is not null )
		{
			Player.Body.Animation.HoldType = CitizenAnimationHelper.HoldTypes.None;
		}
	}

	public void TryPickupThings()
	{

		var tr = Scene.Trace.Ray( GameObject.Transform.Position, GameObject.Transform.Position ).Radius( 64 ).WithTag( "item" ).WithoutTags( "player", "physicsshadow", "playershadow" ).Run();
		if ( tr.Hit && tr.GameObject != null && tr.GameObject.Components.TryGet<ItemComponent>( out var equippable ) )
		{
			if ( equippable.Owner == null )
			{
				Add( tr.GameObject );
			}
		}
		if ( tr.Hit && tr.GameObject != null && tr.GameObject.Components.TryGet<CollectableComponent>( out var collectable ) )
		{
			collectable.OnPickup( Player );
			collectable.GameObject.Destroy();
		}
	}
	public void Add( GameObject item )
	{
		// game crashes on pickup disable this for now
		//return;
		item.Parent = Player.Body.GameObject;
		item.Network.TakeOwnership();
		Items.Add( item );
		//if ( item.Components.TryGet<Rigidbody>( out var rigidbody ) ) rigidbody.PhysicsBody.MotionEnabled = false;
		if ( item.Components.TryGet<Rigidbody>( out var rigidbody ) ) rigidbody.Enabled = false;
		if ( item.Components.TryGet<Collider>( out var collider ) ) collider.Enabled = false;
		if ( item.Components.TryGet<SkinnedModelRenderer>( out var skinnedModelRenderer ) )
		{
			skinnedModelRenderer.BoneMergeTarget = Player.Body.Components.Get<SkinnedModelRenderer>();
		}
		if ( item.Components.TryGet<ItemComponent>( out var equippable ) )
		{
			equippable.Owner = this;
			equippable.OnPickup( Player );
		}

		ActiveItem = item;
	}
}
