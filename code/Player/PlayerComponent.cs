namespace FPSKit;
// split this up into othrr parts maybe??
public class PlayerComponent : Component
{
	[Property] public LifeComponent Life { get; set; }
	[Property] public AimComponent Aim { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public PlayerBodyComponent Body { get; set; }
	[Property] public AmmoComponent Ammo { get; set; }
	[Property] public InventoryComponent Inventory { get; set; }
	[Property] public ViewmodelComponent Viewmodel { get; set; }
	[Property] public CameraComponent Camera { get; set; }
	[Property] public PlayerController Controller { get; set; }
	[Property] public CharacterController Movement { get; set; }

	protected override void OnStart()
	{
		base.OnStart();
		MoveToSpawnpoint();
	}
	public void MoveToSpawnpoint()
	{
		var Spawnpoints = Scene.GetAllComponents<SpawnPoint>().ToList();
		if ( Spawnpoints is not null && Spawnpoints.Count > 0 )
		{
			GameObject.Transform.World = Random.Shared.FromList( Spawnpoints, default ).GameObject.Transform.World;
		}
	}
	protected override void OnAwake()
	{
		base.OnAwake();
		Life.OnTakeDamage += TakeDamage;
		Life.OnKilled += Kill;
		Life.OnRespawn += Respawn;
	}
	public void TakeDamage( DamageInfo info )
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

		Body.Physics.Enabled = true;
		Body.Physics.PhysicsGroup.AddVelocity( Movement.Velocity );
		Body.Tags.RemoveAll();
		Body.Tags.Add( "nopush" );
		Body.Tags.Remove( "player" );
		Body.Model.SceneModel.UseAnimGraph = false;
		Controller.PhysicsShadow.Enabled = false;
		Controller.PlayerShadow.Enabled = false;

		//Body.Components.Get<ModelPhysics>( true ).GameObject.Tags
		//Body.SetParent( null );
		//Body.Components.Get<CitizenAnimationHelper>( true ).Enabled = false;
		//Body.Components.Get<SkinnedModelRenderer>( true ).Reset();
		//GameObject.Destroy();
	}


	public void Respawn()
	{
		Movement.Velocity = Vector3.Zero;
		Body.Transform.LocalPosition = Vector3.Zero;
		Body.Transform.LocalRotation = Rotation.Identity;
		Body.Physics.Enabled = false;
		Body.Model.SceneModel.UseAnimGraph = true;
		Controller.PhysicsShadow.Enabled = true;
		Controller.PlayerShadow.Enabled = true;
		MoveToSpawnpoint();
	}
}
