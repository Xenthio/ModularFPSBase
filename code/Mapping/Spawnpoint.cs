using Sandbox;


namespace FPSKit;

[Title( "Spawnpoint" )]
[Category( "Game" )]
public sealed class Spawnpoint : Component
{
	// Editor model doesnt work yet
	protected override void OnStart()
	{
		base.OnStart();
		GameObject.Components.Get<ModelRenderer>().Destroy();
	}
	protected override void OnUpdate()
	{

	}
}
