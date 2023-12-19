namespace FPSKit;

public class CameraController : Component, INetworkSerializable
{
	[Property, Group( "Settings" )] public bool FirstPerson { get; set; } = true;

	[Property] public GameObject Eye { get; set; }
	[Property] public CameraComponent Camera { get; set; }
	[Property] public CameraComponent ViewmodelCamera { get; set; }

	public Angles EyeAngles;
	protected override void OnUpdate()
	{
		base.OnUpdate();
		Camera.Enabled = !IsProxy;
		ViewmodelCamera.Enabled = !IsProxy && FirstPerson;
		if ( !IsProxy )
		{

			if ( Input.Pressed( "View" ) ) FirstPerson = !FirstPerson;
			EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
			EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
			EyeAngles.roll = 0;
			EyeAngles.pitch = EyeAngles.pitch.Clamp( -89f, 89f );

			Eye.Transform.LocalRotation = EyeAngles.ToRotation();

			if ( FirstPerson )
			{
				Camera.Transform.Position = Eye.Transform.Position;
				Camera.Transform.Rotation = Eye.Transform.Rotation;
			}
			else
			{
				Camera.Transform.Position = Eye.Transform.Position + Eye.Transform.Rotation.Backward * 200 + Vector3.Up * 0.0f;
				Camera.Transform.Rotation = Eye.Transform.Rotation;
			}
			ViewmodelCamera.Transform.Position = Camera.Transform.Position;
			ViewmodelCamera.Transform.Rotation = Camera.Transform.Rotation;


			//var mdl = Body.Components.Get<SkinnedModelRenderer>();

			//mdl.RenderType = FirstPerson && !mdl.IsProxy ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On;
			//mdl.SceneModel.Flags.IsOpaque = !(FirstPerson && !mdl.IsProxy);
			//mdl.Tint = FirstPerson && !mdl.IsProxy ? Color.Transparent : Color.White;
		}
		else
		{
			Eye.Transform.LocalRotation = EyeAngles.ToRotation();
		}

		foreach ( var i in GameObject.Components.GetAll<ModelRenderer>( FindMode.EverythingInSelfAndDescendants ) )
		{
			if ( i.GameObject.Tags.Has( "viewmodel" ) == false )
			{
				i.RenderType = FirstPerson && !GameObject.IsProxy ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On;
			}
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
