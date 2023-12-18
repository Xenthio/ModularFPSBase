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
		if (GameObject.Components.TryGet<ModelRenderer>(out var mdl ) )
		{

			mdl.Destroy();
		}
	}
	protected override void OnUpdate()
	{

	}
}
