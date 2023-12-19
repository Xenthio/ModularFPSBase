namespace FPSKit;

[Title( "Spawnpoint" )]
[Category( "Game" )]

public sealed class Spawnpoint : Component
{
	protected override void DrawGizmos()
	{
		base.DrawGizmos();
		Gizmo.Draw.Model( "models/editor/playerstart.vmdl" );
	}
	protected override void OnUpdate()
	{

	}
}
