using Sandbox.Citizen;

namespace FPSKit;

public class ItemComponent : Component
{
	[Property] public CitizenAnimationHelper.HoldTypes HoldType { get; set; }
	[Property] public Model Viewmodel { get; set; }
	public InventoryComponent OwnerInventory;
	public virtual void OnPickup()
	{

	}
	public virtual void OnDrop()
	{

	}
	protected override void OnStart()
	{
		base.OnStart();
		GameObject.Tags.Set( "item", true );
	}
	public void TriggerAttack()
	{
		if ( OwnerInventory.Player.Body.Animation is not null )
		{
			OwnerInventory.Player.Body.Animation.Target.Set( "b_attack", true );
			OwnerInventory.Player.Viewmodel.Model.Set( "fire", true );
		}
	}
	public void CreateParticle( ParticleSystem particle )
	{
		Transform transform = new Transform();
		var mflash = LegacyParticle.Create( particle?.Name, transform.Position, transform.Rotation );
		if ( !IsProxy && OwnerInventory.Player.Viewmodel.Camera.Enabled )
		{
			transform = OwnerInventory.Player.Viewmodel.Model.GetAttachment( "muzzle" ).Value;
			mflash.GameObject.Tags.Add( "viewmodel" );
			mflash.LegacyParticleSystem.SceneObject.Tags.Add( "viewmodel" );
		}
		else
		{
			transform = Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelf ).GetAttachment( "muzzle" ).Value;
		}
		mflash.Position = transform.Position;
		mflash.Rotation = transform.Rotation;
		mflash.SetVector( 1, Vector3.Zero );
	}
	public void PlaySound( SoundEvent sound )
	{
		Sound.Play( sound, OwnerInventory.Player.Eye.Transform.Position );
	}
	public virtual void FixedCarriableUpdate()
	{

	}
	public virtual void CarriableUpdate()
	{

	}
}
