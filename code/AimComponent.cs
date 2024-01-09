namespace FPSKit;

/// <summary>
/// Put this on a gameobject that is parented to the gameobject you wish to define the aim position for!
/// </summary>
public class AimComponent : Component
{
	public Ray AsRay()
	{
		return new Ray()
		{
			Forward = Transform.Rotation.Forward,
			Position = Transform.Position
		};
	}
}
