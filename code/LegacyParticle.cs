namespace FPSKit;

public class LegacyParticle
{
	public GameObject GameObject;
	public LegacyParticleSystem LegacyParticleSystem;
	public Vector3 Position { get => GameObject.Transform.Position; set => GameObject.Transform.Position = value; }
	public Rotation Rotation { get => GameObject.Transform.Rotation; set => GameObject.Transform.Rotation = value; }
	public static LegacyParticle Create( string name, Vector3 position = default, Rotation rotation = default )
	{
		var lp = new LegacyParticle();
		lp.GameObject = GameManager.ActiveScene.Scene.CreateObject();
		lp.GameObject.Transform.Position = position;
		lp.GameObject.Transform.Rotation = rotation;

		lp.LegacyParticleSystem = lp.GameObject.Components.GetOrCreate<LegacyParticleSystem>();
		lp.LegacyParticleSystem.ControlPoints = new List<ParticleControlPoint>();

		var cp0 = new ParticleControlPoint() { Value = ParticleControlPoint.ControlPointValueInput.Vector3, VectorValue = position };
		lp.LegacyParticleSystem.ControlPoints.Append( cp0 );

		lp.LegacyParticleSystem.Particles = ParticleSystem.Load( name );

		return lp;
	}
	public void SetGameObject( int index, GameObject obj )
	{
		var cpv = new ParticleControlPoint() { Value = ParticleControlPoint.ControlPointValueInput.GameObject, GameObjectValue = obj };
		if ( LegacyParticleSystem.ControlPoints.Count < index )
			LegacyParticleSystem.ControlPoints.Add( cpv );
		else
			LegacyParticleSystem.ControlPoints[index] = cpv;
	}
	public void SetVector( int index, Vector3 vec )
	{
		var cpv = new ParticleControlPoint() { Value = ParticleControlPoint.ControlPointValueInput.Vector3, VectorValue = vec };
		if ( LegacyParticleSystem.ControlPoints.Count < index )
			LegacyParticleSystem.ControlPoints.Add( cpv );
		else
			LegacyParticleSystem.ControlPoints[index] = cpv;
	}
	public void SetFloat( int index, float vec )
	{
		var cpv = new ParticleControlPoint() { Value = ParticleControlPoint.ControlPointValueInput.Float, FloatValue = vec };
		if ( LegacyParticleSystem.ControlPoints.Count < index )
			LegacyParticleSystem.ControlPoints.Add( cpv );
		else
			LegacyParticleSystem.ControlPoints[index] = cpv;
	}
}
