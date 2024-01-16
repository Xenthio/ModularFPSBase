using Sandbox.Citizen;

namespace FPSKit;

public class ItemComponent : Component
{
	[Property] public CitizenAnimationHelper.HoldTypes HoldType { get; set; }
	[Property] public Model Viewmodel { get; set; }
	public InventoryComponent Owner;
	public virtual void OnPickup( PlayerComponent Player )
	{

	}
	public virtual void OnDrop( PlayerComponent Player )
	{

	}
	protected override void OnStart()
	{
		base.OnStart();
		GameObject.Tags.Set( "item", true );
	}
	public void TriggerAttack()
	{
		if ( Owner.Player.Body.Animation is not null )
		{
			Owner.Player.Body.Animation.Target.Set( "b_attack", true );
			Owner.Player.Viewmodel.Model.Set( "fire", true );
		}
	}
	public void CreateParticle( ParticleSystem particle )
	{
		Transform transform = new Transform();
		var mflash = LegacyParticle.Create( particle?.Name, transform.Position, transform.Rotation );
		if ( !IsProxy && Owner.Player.Viewmodel.Camera.Enabled )
		{
			transform = Owner.Player.Viewmodel.Model.GetAttachment( "muzzle" ).Value;
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
		Sound.Play( sound, Owner.Player.Eye.Transform.Position );
	}
	public virtual void FixedCarriableUpdate()
	{

	}
	public virtual void CarriableUpdate()
	{

	}
}
