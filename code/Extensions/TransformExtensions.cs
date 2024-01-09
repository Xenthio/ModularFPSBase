namespace FPSKit;

public static class TransformExtensions
{
	public static Ray Ray( this GameTransform self )
	{
		return new Ray()
		{
			Position = self.Position,
			Forward = self.Rotation.Forward,
		};
	}
}
