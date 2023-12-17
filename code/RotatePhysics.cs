using Sandbox;

public sealed class RotatePhysics : Component
{
	protected override void OnUpdate()
	{
		GameObject.Components.Get<Rigidbody>().AngularVelocity = new Vector3 (0, 0, 2);
	}
}
