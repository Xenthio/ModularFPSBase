using Sandbox;

public sealed class RotatePhysics : Component
{
	[Property] public float RotateAmount { get; set; } = 0.2f;
	Transform trn;
	protected override void OnAwake()
	{
		base.OnAwake();
		trn = GameObject.Transform.World;
	}
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate(); 
		var rigidbody = GameObject.Components.Get<Rigidbody>();
		
		trn = trn.RotateAround( trn.Position, Rotation.FromYaw( RotateAmount * 4 ) );
		rigidbody.PhysicsBody.Move( trn, 1f * Time.Delta );
		rigidbody.PhysicsBody.Mass = 5000;
		rigidbody.PhysicsBody.UseController = true;
		//rigidbody.AngularVelocity = new Vector3 (0, 0, 2);
	}
}
