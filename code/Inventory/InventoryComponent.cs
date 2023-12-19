using Sandbox.Citizen;

namespace FPSKit;

public class InventoryComponent : Component
{
	[Property] public GameObject Eye { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }

	public GameObject ActiveItem;
	public List<GameObject> Items = new List<GameObject>();
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if ( ActiveItem != null && ActiveItem.Components.TryGet<CarriableComponent>( out var activeequippable ) )
		{
			activeequippable.CarriableUpdate();
		}
	}
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		var tr = Scene.Trace.Ray( GameObject.Transform.Position, GameObject.Transform.Position ).Radius( 64 ).WithoutTags( "player" ).Run();
		if ( tr.Hit && tr.GameObject != null && tr.GameObject.Components.TryGet<CarriableComponent>( out var equippable ) )
		{
			if ( equippable.OwnerInventory == null )
			{
				Add( tr.GameObject );
			}
		}
		if ( ActiveItem != null && ActiveItem.Components.TryGet<CarriableComponent>( out var activeequippable ) )
		{
			ActiveItem.Transform.LocalPosition = Vector3.Zero;
			ActiveItem.Transform.LocalRotation = Rotation.Identity;
			activeequippable.FixedCarriableUpdate();
		}

		if ( AnimationHelper is not null && ActiveItem is not null && ActiveItem.Components.TryGet<CarriableComponent>( out var equippableComponent ) )
		{
			AnimationHelper.HoldType = equippableComponent.HoldType;
		}
		else if ( AnimationHelper is not null )
		{
			AnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
		}
	}

	public void TriggerAttack()
	{
		if ( AnimationHelper is not null )
		{
			AnimationHelper.Target.Set( "b_attack", true );
		}
	}
	public void Add( GameObject item )
	{
		item.Parent = Body;
		if ( item.Components.TryGet<CarriableComponent>( out var equippable ) )
		{
			equippable.OwnerInventory = this;
		}
		if ( item.Components.TryGet<Collider>( out var collider ) ) collider.Enabled = false;
		if ( item.Components.TryGet<Rigidbody>( out var rigidbody ) ) rigidbody.PhysicsBody.MotionEnabled = false;

		if ( item.Components.TryGet<SkinnedModelRenderer>( out var skinnedModelRenderer ) )
		{
			skinnedModelRenderer.BoneMergeTarget = Body.Components.Get<SkinnedModelRenderer>();
		}
		item.Network.TakeOwnership();
		Items.Add( item );

		ActiveItem = item;
	}
}
