namespace FPSKit;

public class LegacyParticle
{
	GameObject _Object;
	LegacyParticleSystem _ParticleSystem;
	public Vector3 Position { get => _Object.Transform.Position; set => _Object.Transform.Position = value; }
	public Rotation Rotation { get => _Object.Transform.Rotation; set => _Object.Transform.Rotation = value; }
	public static LegacyParticle Create( string name, Vector3 position = default, Rotation rotation = default )
	{
		var lp = new LegacyParticle();
		lp._Object = GameManager.ActiveScene.Scene.CreateObject();
		lp._Object.Transform.Position = position;
		lp._Object.Transform.Rotation = rotation;

		lp._ParticleSystem = lp._Object.Components.GetOrCreate<LegacyParticleSystem>();
		lp._ParticleSystem.ControlPoints = new List<ParticleControlPoint>();

		var cp0 = new ParticleControlPoint() { Value = ParticleControlPoint.ControlPointValueInput.Vector3, VectorValue = position };
		lp._ParticleSystem.ControlPoints.Append( cp0 );

		lp._ParticleSystem.Particles = ParticleSystem.Load( name );

		return lp;
	}
	public void SetGameObject( int index, GameObject obj )
	{
		var cpv = new ParticleControlPoint() { Value = ParticleControlPoint.ControlPointValueInput.GameObject, GameObjectValue = obj };
		if ( _ParticleSystem.ControlPoints.Count < index )
			_ParticleSystem.ControlPoints.Add( cpv );
		else
			_ParticleSystem.ControlPoints[index] = cpv;
	}
	public void SetVector( int index, Vector3 vec )
	{
		var cpv = new ParticleControlPoint() { Value = ParticleControlPoint.ControlPointValueInput.Vector3, VectorValue = vec };
		if ( _ParticleSystem.ControlPoints.Count < index )
			_ParticleSystem.ControlPoints.Add( cpv );
		else
			_ParticleSystem.ControlPoints[index] = cpv;
	}
}
