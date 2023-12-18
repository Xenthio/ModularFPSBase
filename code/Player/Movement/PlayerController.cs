using Sandbox;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime;
namespace FPSKit;

public class PlayerController : Component, INetworkSerializable
{
	[Property] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );
	[Property] public float WalkSpeed { get; set; } = 120.0f;
	[Property] public float NormalSpeed { get; set; } = 190.0f;
	[Property] public float RunSpeed { get; set; } = 320.0f;
	[Property] public float CrouchSpeed { get; set; } = 80.0f;


	[Property] public float BodyHeight { get; set; } = 72.0f;
	[Property] public float EyeHeight { get; set; } = 64.0f;

	[Property] public float DuckOffset { get; set; } = 40.0f;

	[Property] public float AirControl { get; set; } = 30.0f;

	public Vector3 WishVelocity { get; private set; }

	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public GameObject PhysicsShadow { get; set; }
	[Property] public GameObject PlayerShadow { get; set; }
	[Property] public CitizenAnimation AnimationHelper { get; set; }
	[Property] public bool FirstPerson { get; set; }
	[Property] public bool AlwaysRun { get; set; }

	Vector3 BaseVelocity;

	public Angles EyeAngles;
	public bool IsRunning;
	public bool IsDucking;
	public float _duckAmount = 0;
	public float _duckAmountPerFrame = 0;
	protected override void OnEnabled()
	{
		base.OnEnabled();

		if ( IsProxy )
			return;

		var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();
		if ( cam is not null )
		{
			EyeAngles = Eye.Transform.LocalRotation.Angles();
			EyeAngles.roll = 0;
		}
	}
	protected override void OnUpdate()
	{

		PhysicsShadowReset();


		// Eye input
		if ( !IsProxy )
		{
			EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
			EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
			EyeAngles.roll = 0;
			EyeAngles.pitch = EyeAngles.pitch.Clamp( -89f, 89f );
			var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();

			//doing all ducking stuff per frame is broken for some reason, so I had to add our own lerp for a every frame updated value
			_duckAmountPerFrame = _duckAmountPerFrame.LerpTo( IsDucking ? DuckOffset : 0, 8 * Time.Delta );
			Eye.Transform.LocalPosition = GameObject.Transform.Rotation.Up * (EyeHeight - _duckAmountPerFrame);

			Eye.Transform.LocalRotation = EyeAngles.ToRotation();

			if ( FirstPerson )
			{
				cam.Transform.Position = Eye.Transform.Position;
				cam.Transform.Rotation = Eye.Transform.Rotation;
			}
			else
			{
				cam.Transform.Position = Transform.Position + Eye.Transform.Rotation.Backward * 300 + Vector3.Up * 75.0f;
				cam.Transform.Rotation = Eye.Transform.Rotation;
			}



			IsRunning = Input.Down( "Run" ) || AlwaysRun;
		}

		var cc = GameObject.Components.Get<CharacterController>();
		if ( cc is null ) return;

		float rotateDifference = 0;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

			var v = cc.Velocity.WithZ( 0 );

			if ( v.Length > 10.0f )
			{
				targetAngle = Rotation.LookAt( v, Vector3.Up );
			}

			rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			if ( rotateDifference > 50.0f || cc.Velocity.Length > 10.0f )
			{
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * 2.0f );
			}
			var mdl = Body.Components.Get<SkinnedModelRenderer>();
			
			mdl.Tint = FirstPerson && !mdl.IsProxy ? Color.Transparent : Color.White;
		}


		if ( AnimationHelper is not null )
		{
			AnimationHelper.WithVelocity( cc.Velocity );
			AnimationHelper.WithWishVelocity( WishVelocity );
			AnimationHelper.IsGrounded = cc.IsOnGround;
			AnimationHelper.FootShuffle = rotateDifference;
			AnimationHelper.WithLook( EyeAngles.Forward, 1, 1, 1.0f );
			AnimationHelper.MoveStyle = IsRunning ? CitizenAnimation.MoveStyles.Run : CitizenAnimation.MoveStyles.Walk;
			AnimationHelper.DuckLevel = _duckAmount / DuckOffset;
		}
	}

	[Broadcast]
	public void OnJump( float floatValue, string dataString, object[] objects, Vector3 position )
	{
		AnimationHelper?.TriggerJump();
	}

	float fJumps;

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		var cc = GameObject.Components.Get<CharacterController>();

		//Log.Info( BaseVelocity.z );
		if ( BaseVelocity.z > 100 ) 
		{ 
			cc.Punch( Vector3.Up * BaseVelocity.z );
			cc.IsOnGround = false; 
		}
		CheckDuck();

		if ( true ) // Not swimming or on ladder
		{
			cc.Velocity -= (Gravity * 0.5f) * Time.Delta;
			cc.Velocity += new Vector3( 0, 0, BaseVelocity.z ) ;

			BaseVelocity = BaseVelocity.WithZ( 0 );
		}
		//else
		//{
		//	cc.Velocity = cc.Velocity.WithZ( 0 );
		//}

		WishVelocity = BuildWishVelocity();



		if ( cc.IsOnGround && Input.Down( "Jump" ) )
		{
			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;
			//if ( Duck.IsActive )
			//	flMul *= 0.8f;

			cc.Punch( Vector3.Up * flMul * flGroundFactor );
			//	cc.IsOnGround = false;

			OnJump( fJumps, "Hello", new object[] { Time.Now.ToString(), 43.0f }, Vector3.Random );

			fJumps += 1.0f;

		}
		PhysicsShadowUpdate();

		PhysicsShadowReset();
		PlayerShadowUpdate();
		if ( cc.IsOnGround )
		{
			var wishspeed = WishVelocity;

			//cc.Velocity = cc.Velocity.WithZ( 0 );

			//ps.AngularVelocity = Vector3.Zero;
			//ps.Velocity = Vector3.Zero;
			cc.Accelerate( wishspeed );
			cc.ApplyFriction( 4.0f );
		}
		else
		{
			var wishspeed = WishVelocity;


			if ( wishspeed.Length > AirControl )
				wishspeed = wishspeed.ClampLength(AirControl); 
			cc.Accelerate( wishspeed );
			//cc.ApplyFriction( 0.1f );
		}

		var doBaseVelocity = cc.IsOnGround;
		if ( doBaseVelocity ) cc.Velocity += BaseVelocity;
		cc.Move();
		if ( doBaseVelocity ) cc.Velocity -= BaseVelocity;

		if ( true )
		{
			// finish gravity
			cc.Velocity -= (Gravity * 0.5f) * Time.Delta;
		}


	} 

	void PlayerShadowUpdate()
	{

		var ps = PlayerShadow.Components.Get<Rigidbody>();
		var cc = GameObject.Components.Get<CharacterController>();
		ps.PhysicsBody.Mass = 70;
		ps.PhysicsBody.UseController = false;
		ps.PhysicsBody.AngularVelocity = Vector3.Zero;

		var shvel = cc.Velocity;
		shvel.x = MathF.MaxMagnitude( shvel.x, WishVelocity.x );
		shvel.y = MathF.MaxMagnitude( shvel.y, WishVelocity.y );
		shvel.z = MathF.MaxMagnitude( shvel.z, WishVelocity.z );

		ps.PhysicsBody.Velocity = shvel;
		ps.Transform.LocalPosition = Vector3.Zero.WithZ( (((BodyHeight - _duckAmountPerFrame)) / 2) );
		ps.Transform.LocalRotation = GameObject.Transform.World.RotationToLocal( Rotation.Identity );
	}
	void PhysicsShadowReset()
	{
		var ps = PhysicsShadow.Components.Get<Rigidbody>();
		var cc = GameObject.Components.Get<CharacterController>();
		//ps.PhysicsBody.AngularVelocity = Vector3.Zero;
		//ps.PhysicsBody.Velocity = Vector3.Zero;
		ps.Transform.LocalPosition = Vector3.Zero.WithZ( (((BodyHeight - _duckAmountPerFrame)) / 2) );
		ps.Transform.LocalRotation = GameObject.Transform.World.RotationToLocal( Rotation.Identity );
	}

	bool _physetup = false;
	void PhysicsBodyFirstTimeSetup()
	{
		if ( _physetup ) return;
		_physetup = true;
		var ps = PhysicsShadow.Components.Get<Rigidbody>();
		ps.PhysicsBody.SpeculativeContactEnabled = false; 
		ps.PhysicsBody.Mass = 70;
		ps.PhysicsBody.LinearDamping = 0;
		if ( ps.PhysicsBody.Surface == null )
		{
			var newsurf = new Surface()
			{
				Friction = 2000000,
				Elasticity = 0,
			};
			ps.PhysicsBody.Surface = newsurf;
		}
	}

	bool PreviouslyOnGround = false;
	bool PreviouslyPushed = false;

	public void PhysicsShadowUpdate()
	{
		PhysicsBodyFirstTimeSetup();
		var ps = PhysicsShadow.Components.Get<Rigidbody>();
		var cc = GameObject.Components.Get<CharacterController>();

		var trdown = Scene.PhysicsWorld.Trace.Box( cc.BoundingBox, GameObject.Transform.Position, GameObject.Transform.Position + Vector3.Down ).WithoutTags( cc.IgnoreLayers ).Run();
		
		var tr = Scene.PhysicsWorld.Trace.Box( cc.BoundingBox, GameObject.Transform.Position, GameObject.Transform.Position ).WithoutTags( cc.IgnoreLayers ).Run();

		var body = trdown.Body;


		var vel = ps.PhysicsBody.Velocity;
		var angv = ps.PhysicsBody.AngularVelocity;

		//Log.Info( $"{PreviouslyOnGround} {cc.IsOnGround}" );
		// Do transfer from jumping or moving off something moving
		if ((PreviouslyOnGround && !trdown.Hit) || (PreviouslyPushed && !tr.Hit))
		{
			BaseVelocity = Vector3.Zero;
			ps.PhysicsBody.Velocity = Vector3.Zero;
			Log.Info( "transfer" );
			cc.Velocity += vel;
			PreviouslyPushed = tr.Hit;
			PreviouslyOnGround = trdown.Hit;
			return;
		}
		PreviouslyPushed = tr.Hit;
		PreviouslyOnGround = trdown.Hit;

		if ( trdown.Body == null || trdown.Body.MotionEnabled == false )
		{
			ps.PhysicsBody.Velocity = Vector3.Zero;
			if ( cc.IsOnGround ) BaseVelocity = Vector3.Zero;
			return;
		} 


		if (tr.Hit)
		{
			vel.z = MathF.Max( 0, vel.z );
			GameObject.Transform.Position += vel * Time.Delta; 
		}
		else
		{ 
			if ( cc.IsOnGround ) BaseVelocity = vel;
		} 

		// do rotation
		var a = new Angles( angv.x, angv.y, angv.z ); 

		//only rotate yaw
		var axis = angv.WithX( 0 ).WithY( 0 ); 
		GameObject.Transform.Rotation = GameObject.Transform.Rotation.RotateAroundAxis( axis.Normal, axis.Length);
		ps.AngularDamping = 10000;
		//Log.Info( vel );
		//Log.Info( angv );
	}
	public void CheckDuck()
	{
		var duckDelta = _duckAmount;

		var cc = GameObject.Components.Get<CharacterController>();

		var uncrouchedBbox = new BBox( new Vector3( 0f - cc.Radius, 0f - cc.Radius, 0f ), new Vector3( cc.Radius, cc.Radius, BodyHeight ) );
		IsDucking = Input.Down( "Duck" ) || IsDucking && Scene.PhysicsWorld.Trace.Box( uncrouchedBbox, GameObject.Transform.Position, GameObject.Transform.Position).WithoutTags(cc.IgnoreLayers).Run().Hit;

		if ( IsDucking )
		{
			_duckAmount = _duckAmount.LerpTo( DuckOffset, 8 * Time.Delta );
		}
		else
		{
			_duckAmount = _duckAmount.LerpTo( 0, 8 * Time.Delta );
		}
		duckDelta -= _duckAmount;
		
		cc.Height = BodyHeight - _duckAmount;
		if ( !cc.IsOnGround )
		{
			cc.GameObject.Transform.Position += new Vector3( 0, 0, duckDelta * -1);
		}
	}
	public Vector3 BuildWishVelocity()
	{
		Vector3 wishVelocity;
		var rot = Eye.Transform.Rotation;

		wishVelocity = 0;

		if ( Input.Down( "Forward" ) ) wishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) wishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) wishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) wishVelocity += rot.Right;

		wishVelocity = wishVelocity.WithZ( 0 );

		if ( !wishVelocity.IsNearZeroLength ) wishVelocity = wishVelocity.Normal;

		if ( Input.Down( "Walk" ) ) wishVelocity *= WalkSpeed;
		else if ( Input.Down( "Duck" ) || IsDucking ) wishVelocity *= CrouchSpeed;
		else if ( Input.Down( "Run" ) || AlwaysRun ) wishVelocity *= RunSpeed;
		else wishVelocity *= NormalSpeed;
		return wishVelocity;
	}

	public void Write( ref ByteStream stream )
	{
		stream.Write( IsRunning );
		stream.Write( EyeAngles );
	}

	public void Read( ByteStream stream )
	{
		IsRunning = stream.Read<bool>();
		EyeAngles = stream.Read<Angles>();
	}
}
