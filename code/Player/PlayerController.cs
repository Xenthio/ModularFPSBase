using Sandbox.Citizen;
namespace FPSKit;

public class PlayerController : Component, INetworkSerializable
{
	[Property, Group( "Movement" )] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );
	[Property, Group( "Movement" )] public float WalkSpeed { get; set; } = 120.0f;
	[Property, Group( "Movement" )] public float NormalSpeed { get; set; } = 190.0f;
	[Property, Group( "Movement" )] public float RunSpeed { get; set; } = 320.0f;
	[Property, Group( "Movement" )] public float CrouchSpeed { get; set; } = 80.0f;
	[Property, Group( "Movement" )] public float AirControl { get; set; } = 30.0f;
	[Property, Group( "Movement" )] public bool AlwaysRun { get; set; }

	[Property, Group( "Measurements" )] public float BodyHeight { get; set; } = 72.0f;
	[Property, Group( "Measurements" )] public float EyeHeight { get; set; } = 64.0f;
	[Property, Group( "Measurements" )] public float DuckOffset { get; set; } = 40.0f;


	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public GameObject PhysicsShadow { get; set; }
	[Property] public GameObject PlayerShadow { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }


	public Vector3 WishVelocity { get; private set; }
	public Vector3 BaseVelocity;
	public Vector3 PotentialVelocity;

	public bool IsRunning;
	public bool IsDucking;
	public float _duckAmount = 0;
	public float _duckAmountPerFrame = 0;

	protected override void OnUpdate()
	{

		PhysicsShadowReset();
		//PlayerShadowUpdate();

		// Eye input
		if ( !IsProxy )
		{

			// doing all ducking stuff per frame is broken for some reason, so I had to add our own lerp for a every frame updated value
			_duckAmountPerFrame = _duckAmountPerFrame.LerpTo( IsDucking ? DuckOffset : 0, 8 * Time.Delta );
			Eye.Transform.LocalPosition = GameObject.Transform.Rotation.Up * (EyeHeight - _duckAmountPerFrame);

			IsRunning = Input.Down( "Run" ) || AlwaysRun;
		}

		var cc = GameObject.Components.Get<CharacterController>();
		if ( cc is null ) return;

		float rotateDifference = 0;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = Rotation.FromYaw( Eye.Transform.Rotation.Yaw() );

			rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			if ( rotateDifference > 50.0f || cc.Velocity.Length > 10.0f )
			{
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * 4.0f );
			}
		}

		if ( AnimationHelper is not null )
		{
			AnimationHelper.WithVelocity( cc.Velocity );
			AnimationHelper.WithWishVelocity( WishVelocity );
			AnimationHelper.IsGrounded = cc.IsOnGround;
			AnimationHelper.FootShuffle = rotateDifference;
			AnimationHelper.WithLook( Eye.Transform.Rotation.Forward, 1, 1, 1.0f );
			AnimationHelper.MoveStyle = IsRunning ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Auto;
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
			cc.Velocity += new Vector3( 0, 0, BaseVelocity.z );

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
				wishspeed = wishspeed.ClampLength( AirControl );
			cc.Accelerate( wishspeed );
			//cc.ApplyFriction( 0.1f );
		}

		PlayerShadowUpdate();

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
		PlayerShadowFirstTimeSetup();

		var ps = PlayerShadow.Components.Get<Rigidbody>();
		var cc = GameObject.Components.Get<CharacterController>();
		var bc = PlayerShadow.Components.Get<BoxCollider>();



		var sc = bc.Scale;
		sc.z = BodyHeight - _duckAmountPerFrame;
		bc.Scale = sc;


		// This is the velocity we would have if we could move freely without bumping into anything
		PotentialVelocity = PotentialVelocity.WithAcceleration( WishVelocity, cc.Acceleration * Time.Delta );
		PotentialVelocity = AddFriction( PotentialVelocity, 4 );

		var shvel = cc.Velocity * 1f;
		var whvel = PotentialVelocity * 1f;

		shvel.x = MathF.MaxMagnitude( shvel.x, whvel.x );
		shvel.y = MathF.MaxMagnitude( shvel.y, whvel.y );
		shvel.z = MathF.MaxMagnitude( shvel.z, whvel.z );

		ps.PhysicsBody.Velocity = shvel;
		ps.PhysicsBody.AngularVelocity = Vector3.Zero;

		ps.Transform.LocalPosition = Vector3.Zero.WithZ( (((BodyHeight - _duckAmountPerFrame)) / 2) );
		ps.Transform.LocalRotation = GameObject.Transform.World.RotationToLocal( Rotation.Identity );
	}

	bool _plyshsetup = false;
	void PlayerShadowFirstTimeSetup()
	{

		if ( _plyshsetup ) return;
		_plyshsetup = true;

		var ps = PlayerShadow.Components.Get<Rigidbody>();
		ps.PhysicsBody.Mass = 70;
		ps.PhysicsBody.UseController = false;
		ps.PhysicsBody.SpeculativeContactEnabled = false;
	}

	Vector3 AddFriction( Vector3 vel, float frictionAmount, float stopSpeed = 140f )
	{
		float length = vel.Length;
		if ( !(length < 0.01f) )
		{
			float num = ((length < stopSpeed) ? stopSpeed : length);
			float num2 = num * Time.Delta * frictionAmount;
			float num3 = length - num2;
			if ( num3 < 0f )
			{
				num3 = 0f;
			}

			if ( num3 != length )
			{
				num3 /= length;
				vel *= num3;
			}
		}
		return vel;
	}

	void PhysicsShadowReset()
	{
		var ps = PhysicsShadow.Components.Get<Rigidbody>();
		var cc = GameObject.Components.Get<CharacterController>();
		var bc = PhysicsShadow.Components.Get<BoxCollider>();
		var sc = bc.Scale;
		sc.z = BodyHeight - _duckAmountPerFrame;
		bc.Scale = sc;
		//ps.PhysicsBody.AngularVelocity = Vector3.Zero;
		//ps.PhysicsBody.Velocity = Vector3.Zero;
		ps.PhysicsBody.LocalMassCenter = ps.Transform.World.ToLocal( GameObject.Transform.World ).Position;
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
		if ( ps.PhysicsBody.Surface == null )
		{
			var newsurf = new Surface()
			{
				Friction = 10000,
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

		//Gizmo.Draw.SolidSphere( ps.PhysicsBody.MassCenter, 2 );
		//Gizmo.Draw.LineBBox( ps.PhysicsBody.GetBounds() );
		var trdown = Scene.PhysicsWorld.Trace.Box( cc.BoundingBox, GameObject.Transform.Position, GameObject.Transform.Position + Vector3.Down ).WithoutTags( cc.IgnoreLayers ).Run();

		var tr = Scene.PhysicsWorld.Trace.Box( cc.BoundingBox, GameObject.Transform.Position, GameObject.Transform.Position ).WithoutTags( cc.IgnoreLayers ).Run();

		var body = trdown.Body;


		var vel = ps.PhysicsBody.Velocity;
		var angv = ps.PhysicsBody.AngularVelocity;

		//Log.Info( $"{PreviouslyOnGround} {cc.IsOnGround}" );
		// Do transfer from jumping or moving off something moving
		if ( (PreviouslyOnGround && !trdown.Hit) || (PreviouslyPushed && !tr.Hit) )
		{
			BaseVelocity = Vector3.Zero;
			ps.PhysicsBody.Velocity = Vector3.Zero;
			//Log.Info( "transfer" );
			cc.Velocity += vel;
			PreviouslyPushed = tr.Hit;
			PreviouslyOnGround = trdown.Hit;
			return;
		}
		PreviouslyPushed = tr.Hit;
		PreviouslyOnGround = trdown.Hit;


		// do rotation
		var a = new Angles( angv.x, angv.y, angv.z );
		//only rotate yaw
		var axis = angv.WithX( 0 ).WithY( 0 );
		GameObject.Transform.Rotation = GameObject.Transform.Rotation.RotateAroundAxis( axis.Normal, axis.Length );
		ps.AngularDamping = 10000;

		if ( trdown.Body == null || trdown.Body.MotionEnabled == false )
		{
			ps.PhysicsBody.Velocity = Vector3.Zero;
			if ( cc.IsOnGround ) BaseVelocity = Vector3.Zero;
			return;
		}


		if ( tr.Hit )
		{
			vel.z = MathF.Max( 0, vel.z );
			GameObject.Transform.Position += vel * Time.Delta;
		}
		else
		{
			if ( cc.IsOnGround ) BaseVelocity = vel;
		}


		//Log.Info( vel );
		//Log.Info( angv );
	}
	public void CheckDuck()
	{
		var duckDelta = _duckAmount;

		var cc = GameObject.Components.Get<CharacterController>();

		var uncrouchedBbox = new BBox( new Vector3( 0f - cc.Radius, 0f - cc.Radius, 0f ), new Vector3( cc.Radius, cc.Radius, BodyHeight ) );
		IsDucking = Input.Down( "Duck" ) || IsDucking && Scene.PhysicsWorld.Trace.Box( uncrouchedBbox, GameObject.Transform.Position, GameObject.Transform.Position ).WithoutTags( cc.IgnoreLayers ).Run().Hit;

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
			cc.GameObject.Transform.Position += new Vector3( 0, 0, duckDelta * -1 );
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
		stream.Write( IsDucking );
	}

	public void Read( ByteStream stream )
	{
		IsRunning = stream.Read<bool>();
		IsDucking = stream.Read<bool>();
	}
}
