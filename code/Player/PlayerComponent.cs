using Sandbox.Citizen;

namespace FPSKit;
// split this up into othrr parts maybe??
public class PlayerComponent : Component
{
	[Property] public LifeComponent Life { get; set; }
	[Property] public AimComponent Aim { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }
	[Property] public ViewmodelComponent Viewmodel { get; set; }
	protected override void OnAwake()
	{
		base.OnAwake();
		Life.OnTakeDamage += OnTakeDamage;
		Life.OnKilled += Kill;
	}
	public void OnTakeDamage( DamageInfo info )
	{

	}

	[ConCmd( "kill" )]
	public static void KillConCmd()
	{
		foreach ( var i in GameManager.ActiveScene.Components.GetAll<PlayerComponent>() )
		{
			i.Life.TakeDamage( DamageInfo.Generic( 100000 ) );
		}
	}
	[ConCmd( "respawn" )]
	public static void RespawnConCmd()
	{
		foreach ( var i in GameManager.ActiveScene.Components.GetAll<PlayerComponent>() )
		{
			i.Life.Respawn();
		}
	}
	public void Kill()
	{
		Log.Info( "U R DEAD!" );
		//GameObject.Components.Get<CameraComponent>( FindMode.EnabledInSelfAndChildren ).GameObject.SetParent( null );
		Body.Components.Get<ModelPhysics>( true ).Enabled = true;
		Body.Components.Get<ModelPhysics>( true ).Enabled = true;
		//Body.Components.Get<ModelPhysics>( true ).GameObject.Tags
		Body.Components.Get<SkinnedModelRenderer>( true ).SceneModel.UseAnimGraph = false;
		//Body.SetParent( null );
		//Body.Components.Get<CitizenAnimationHelper>( true ).Enabled = false;
		//Body.Components.Get<SkinnedModelRenderer>( true ).Reset();
		//GameObject.Destroy();
	}

	public void OnDamage( in global::DamageInfo damage )
	{
		throw new NotImplementedException();
	}
}
