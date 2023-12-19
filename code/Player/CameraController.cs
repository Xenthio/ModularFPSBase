namespace FPSKit;

public class CameraController : Component
{
	[Property, Group( "Settings" )] public bool FirstPerson { get; set; } = true;

	[Property] public GameObject Eye { get; set; }
	[Property] public CameraComponent Camera { get; set; }

	public Angles EyeAngles;
	protected override void OnUpdate()
	{
		Camera.Enabled = !IsProxy;
		base.OnUpdate();
		if ( !IsProxy )
		{

			if ( Input.Pressed( "View" ) ) FirstPerson = !FirstPerson;
			EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
			EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
			EyeAngles.roll = 0;
			EyeAngles.pitch = EyeAngles.pitch.Clamp( -89f, 89f );

			var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();

			Eye.Transform.LocalRotation = EyeAngles.ToRotation();

			if ( FirstPerson )
			{
				cam.Transform.Position = Eye.Transform.Position;
				cam.Transform.Rotation = Eye.Transform.Rotation;
			}
			else
			{
				cam.Transform.Position = Eye.Transform.Position + Eye.Transform.Rotation.Backward * 200 + Vector3.Up * 0.0f;
				cam.Transform.Rotation = Eye.Transform.Rotation;
			}

			foreach ( var i in GameObject.Components.GetAll<ModelRenderer>() )
			{
				i.RenderType = FirstPerson && !GameObject.IsProxy ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On;
			}
			//var mdl = Body.Components.Get<SkinnedModelRenderer>();

			//mdl.RenderType = FirstPerson && !mdl.IsProxy ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On;
			//mdl.SceneModel.Flags.IsOpaque = !(FirstPerson && !mdl.IsProxy);
			//mdl.Tint = FirstPerson && !mdl.IsProxy ? Color.Transparent : Color.White;
		}
	}
	public void Write( ref ByteStream stream )
	{
		stream.Write( EyeAngles );
	}

	public void Read( ByteStream stream )
	{
		EyeAngles = stream.Read<Angles>();
	}
}
