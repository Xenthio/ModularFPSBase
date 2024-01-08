using Sandbox.Citizen;

namespace FPSKit;

public class InventoryComponent : Component
{
	[Property] public GameObject Eye { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Viewmodel { get; set; }
	[Property] public CameraComponent ViewmodelCamera { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }


	public SkinnedModelRenderer ViewmodelModel => Viewmodel.Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelf );
	public SkinnedModelRenderer ActiveItemModel => ActiveItem.Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelf );
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
			// set the viewmodel to the current items viewmodel
			if ( activeequippable.Viewmodel != null )
			{
				ViewmodelModel.Model = activeequippable.Viewmodel;
			}
			else
			{
				ViewmodelModel.Model = null;
			}
		}

		if ( AnimationHelper is not null && ActiveItem is not null && ActiveItem.Components.TryGet<ItemComponent>( out var equippableComponent ) )
		{
			AnimationHelper.HoldType = equippableComponent.HoldType;
		}
		else if ( AnimationHelper is not null )
		{
			AnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
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
		if ( AnimationHelper is not null )
		{
			AnimationHelper.Target.Set( "b_attack", true );
			ViewmodelModel.Set( "fire", true );
		}
	}
	public void CreateParticle( ParticleSystem prt )
	{
		Transform transform = new Transform();
		var mflash = LegacyParticle.Create( prt?.Name, transform.Position, transform.Rotation );
		if ( !IsProxy && ViewmodelCamera.Enabled )
		{
			transform = ViewmodelModel.GetAttachment( "muzzle" ).Value;
			mflash.GameObject.Tags.Add( "viewmodel" );
			mflash.LegacyParticleSystem.SceneObject.Tags.Add( "viewmodel" );
		}
		else
		{
			transform = ActiveItemModel.GetAttachment( "muzzle" ).Value;
		}
		mflash.Position = transform.Position;
		mflash.Rotation = transform.Rotation;
		mflash.SetVector( 1, Vector3.Zero );
	}
	public void Add( GameObject item )
	{
		// game crashes on pickup disable this for now
		//return;
		item.Parent = Body;
		item.Network.TakeOwnership();
		Items.Add( item );
		//if ( item.Components.TryGet<Rigidbody>( out var rigidbody ) ) rigidbody.PhysicsBody.MotionEnabled = false;
		if ( item.Components.TryGet<Rigidbody>( out var rigidbody ) ) rigidbody.Enabled = false;
		if ( item.Components.TryGet<Collider>( out var collider ) ) collider.Enabled = false;
		if ( item.Components.TryGet<SkinnedModelRenderer>( out var skinnedModelRenderer ) )
		{
			skinnedModelRenderer.BoneMergeTarget = Body.Components.Get<SkinnedModelRenderer>();
		}
		if ( item.Components.TryGet<ItemComponent>( out var equippable ) )
		{
			equippable.OwnerInventory = this;
			equippable.OnPickup();
		}

		ActiveItem = item;
	}
}
