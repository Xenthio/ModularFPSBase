using Sandbox;

public sealed class SineUpandDown : Component
{ 
	Transform trn;
	Vector3 InitialPosition;
	[Property] public int Strength { get; set; } = 128;
	protected override void OnAwake()
	{
		base.OnAwake();
		trn = GameObject.Transform.World;
		InitialPosition = trn.Position;
	}
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		var rigidbody = GameObject.Components.Get<Rigidbody>();
		trn.Position = InitialPosition + Vector3.Up * (MathF.Sin( Time.Now ) * Strength);
		rigidbody.PhysicsBody.Move( trn, 2f * Time.Delta );
		rigidbody.PhysicsBody.Mass = 5000;
		rigidbody.PhysicsBody.UseController = true;
		//rigidbody.AngularVelocity = new Vector3 (0, 0, 2);
	}
}
