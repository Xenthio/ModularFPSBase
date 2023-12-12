using Sandbox;
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
	[Property] public CitizenAnimation AnimationHelper { get; set; }
	[Property] public bool FirstPerson { get; set; }
	[Property] public bool AlwaysRun { get; set; }

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
			EyeAngles = cam.Transform.Rotation.Angles();
			EyeAngles.roll = 0;
		}
	}

	protected override void OnUpdate()
	{


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
			cam.Transform.LocalPosition = new Vector3(0,0,EyeHeight - _duckAmountPerFrame );

			var lookDir = EyeAngles.ToRotation();

			if ( FirstPerson )
			{
				cam.Transform.Position = Eye.Transform.Position;
				cam.Transform.Rotation = lookDir;
			}
			else
			{
				cam.Transform.Position = Transform.Position + lookDir.Backward * 300 + Vector3.Up * 75.0f;
				cam.Transform.Rotation = lookDir;
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
			mdl.Tint = FirstPerson ? Color.Transparent : Color.White;
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

		CheckDuck();
		BuildWishVelocity();

		var cc = GameObject.Components.Get<CharacterController>();

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

		if ( cc.IsOnGround )
		{
			var wishspeed = WishVelocity;

			cc.Velocity = cc.Velocity.WithZ( 0 );
			cc.Accelerate( wishspeed );
			cc.ApplyFriction( 4.0f );
		}
		else
		{
			var wishspeed = WishVelocity;

			cc.Velocity -= Gravity * Time.Delta * 0.5f;

			if ( wishspeed.Length > AirControl )
				wishspeed = wishspeed.ClampLength(AirControl);

			cc.Accelerate( wishspeed );
			//cc.ApplyFriction( 0.1f );
		}

		cc.Move();

		if ( !cc.IsOnGround )
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
		}
	}
	public void CheckDuck()
	{
		var duckDelta = _duckAmount;

		var cc = GameObject.Components.Get<CharacterController>();

		IsDucking = Input.Down( "Duck" );

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
	public void BuildWishVelocity()
	{
		var rot = EyeAngles.ToRotation();

		WishVelocity = 0;

		if ( Input.Down( "Forward" ) ) WishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) WishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) WishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( Input.Down( "Walk" ) ) WishVelocity *= WalkSpeed;
		else if ( Input.Down( "Duck" ) ) WishVelocity *= CrouchSpeed;
		else if ( Input.Down( "Run" ) || AlwaysRun ) WishVelocity *= RunSpeed;
		else WishVelocity *= NormalSpeed;
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
