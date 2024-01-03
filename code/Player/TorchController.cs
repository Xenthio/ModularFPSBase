namespace FPSKit;

public class TorchController : Component
{
	[Property] public SpotLight TorchLight { get; set; }
	float distance = 0;
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		if ( Input.Pressed( "Flashlight" ) )
		{
			TorchLight.Enabled = !TorchLight.Enabled;
		}
		if ( TorchLight.Enabled )
		{
			var tr = Scene.Trace.Ray( TorchLight.GameObject.Parent.Transform.Position, TorchLight.GameObject.Parent.Transform.Position + (TorchLight.GameObject.Transform.Rotation.Forward * 24) ).WithoutTags( "player" ).Run();

			distance = distance.LerpTo( tr.Distance, Time.Delta * 16f );
			TorchLight.Transform.LocalPosition = -Vector3.Forward * (4 - distance);
		}
	}
}
