using Sandbox.Citizen;

namespace FPSKit;

public class InventoryComponent : Component
{
	[Property] public PlayerComponent Player { get; set; }

	public GameObject ActiveItem;
	public List<GameObject> Items = new List<GameObject>();
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if ( ActiveItem != null && ActiveItem.Components.TryGet<ItemComponent>( out var activeequippable ) )
		{
			activeequippable.CarriableUpdate();
		}
	}
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		TryPickupThings();
		if ( ActiveItem != null && ActiveItem.Components.TryGet<ItemComponent>( out var activeequippable ) )
		{
			ActiveItem.Transform.LocalPosition = Vector3.Zero;
			ActiveItem.Transform.LocalRotation = Rotation.Identity;
			activeequippable.FixedCarriableUpdate();
		}

		if ( Player.Animation is not null && ActiveItem is not null && ActiveItem.Components.TryGet<ItemComponent>( out var equippableComponent ) )
		{
			Player.Animation.HoldType = equippableComponent.HoldType;
		}
		else if ( Player.Animation is not null )
		{
			Player.Animation.HoldType = CitizenAnimationHelper.HoldTypes.None;
		}
	}

	public void TryPickupThings()
	{

		var tr = Scene.Trace.Ray( GameObject.Transform.Position, GameObject.Transform.Position ).Radius( 64 ).WithTag( "item" ).WithoutTags( "player", "physicsshadow", "playershadow" ).Run();
		if ( tr.Hit && tr.GameObject != null && tr.GameObject.Components.TryGet<ItemComponent>( out var equippable ) )
		{
			if ( equippable.OwnerInventory == null )
			{
				Add( tr.GameObject );
			}
		}
	}
	public void TriggerAttack()
	{
		if ( Player.Animation is not null )
		{
			Player.Animation.Target.Set( "b_attack", true );
			Player.Viewmodel.Model.Set( "fire", true );
		}
	}
	public void CreateParticle( ParticleSystem prt )
	{
		Transform transform = new Transform();
		var mflash = LegacyParticle.Create( prt?.Name, transform.Position, transform.Rotation );
		if ( !IsProxy && Player.Viewmodel.Camera.Enabled )
		{
			transform = Player.Viewmodel.Model.GetAttachment( "muzzle" ).Value;
			mflash.GameObject.Tags.Add( "viewmodel" );
			mflash.LegacyParticleSystem.SceneObject.Tags.Add( "viewmodel" );
		}
		else
		{
			transform = ActiveItem.Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelf ).GetAttachment( "muzzle" ).Value;
		}
		mflash.Position = transform.Position;
		mflash.Rotation = transform.Rotation;
		mflash.SetVector( 1, Vector3.Zero );
	}
	public void Add( GameObject item )
	{
		// game crashes on pickup disable this for now
		//return;
		item.Parent = Player.Body;
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
			equippable.OwnerInventory = this;
			equippable.OnPickup();
		}

		ActiveItem = item;
	}
}
