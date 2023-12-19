public sealed class bridgecreatorlol : Component
{
	GameObject PreviouslyCreatedGameObject;
	GameObject FinalPlank;
	[Property] public int PlankCount { get; set; } = 32;
	[Property] public int Spacing { get; set; } = 8;
	[Property] public int Slack { get; set; } = 8;
	bool updated = false;
	protected override void OnStart()
	{
		if ( updated ) return;
		updated = true;
		base.OnStart();
		PreviouslyCreatedGameObject = null;


		for ( int i = 1; i <= PlankCount; i++ )
		{
			PreviouslyCreatedGameObject = CreateCopy( i );
		}

		GameObject.Components.Get<Rigidbody>().PhysicsBody.MotionEnabled = false;
		GameObject.Components.Get<Rigidbody>().PhysicsBody.GravityEnabled = false;
		GameObject.Components.Get<Rigidbody>().PhysicsBody.Sleeping = true;
		GameObject.Components.Get<ModelCollider>().Static = false;
	}
	public GameObject Duplicate()
	{

		var copy = GameManager.ActiveScene.CreateObject();
		foreach ( Component component in this.Components.GetAll() )
		{
			if ( component is bridgecreatorlol ) continue;
			var c = copy.Components.Create( TypeLibrary.GetType( component.GetType() ) );
			c.DeserializeImmediately( component.Serialize().AsObject() );
		}
		foreach ( var tag in this.Tags.TryGetAll() )
		{
			copy.Tags.Add( tag );
		}
		copy.Transform.Rotation = Transform.Rotation;
		copy.Transform.Position = Transform.Position;
		copy.Transform.Scale = new Vector3( 1.5f, 1, 1 );
		return copy;
	}
	GameObject CreateCopy( int i )
	{

		var copy = Duplicate();

		copy.Parent = GameObject;
		copy.Transform.Position = Transform.Position + Transform.Rotation.Right * (Slack * i);


		var joint2 = copy.Components.Create<HingeJoint>();
		joint2.EnableCollision = false;
		joint2.MaxAngle = 0;
		joint2.MinAngle = 0;
		joint2.Friction = 1000000;
		joint2.BreakForce = 1000;
		joint2.BreakTorque = 1000;
		joint2.EnableCollision = false;
		joint2.Body = PreviouslyCreatedGameObject;

		if ( i == 1 || i == PlankCount )
		{
			var joint3 = copy.Components.Create<HingeJoint>();
			joint3.MaxAngle = 0;
			joint3.MinAngle = 0;
			joint3.Friction = 1000000;
			joint3.BreakForce = 1000;
			joint3.BreakTorque = 1000;
			joint3.EnableCollision = false;
		}

		copy.Transform.Position = Transform.Position + Transform.Rotation.Right * (Spacing * i);
		return copy;
	}
	protected override void OnUpdate()
	{

	}
}
