namespace FPSKit;

public class CameraController : Component
{
	[Property, Group( "Settings" )] public bool FirstPerson { get; set; } = true;

	[Property] public PlayerComponent Player { get; set; }

	[Sync] public Angles EyeAngles { get; set; }
	protected override void OnUpdate()
	{
		base.OnUpdate();
		Player.Camera.Enabled = !IsProxy && GameObject.Network.IsOwner;
		Player.Viewmodel.Camera.Enabled = !IsProxy && FirstPerson && GameObject.Network.IsOwner && Player.Viewmodel.Camera.Components.GetAll<ModelRenderer>().Where( x => x.Model != null && x.Model.ResourcePath != "models/dev/new_model/new_model.vmdl" ).Any();
		if ( !IsProxy )
		{
			if ( Input.Pressed( "View" ) ) FirstPerson = !FirstPerson;

			var eyeAngles = EyeAngles;
			eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
			eyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
			eyeAngles.roll = 0;
			eyeAngles.pitch = eyeAngles.pitch.Clamp( -89f, 89f );
			EyeAngles = eyeAngles;

			Player.Eye.Transform.LocalRotation = EyeAngles.ToRotation();

			if ( FirstPerson )
			{
				Player.Camera.Transform.Position = Player.Eye.Transform.Position;
				Player.Camera.Transform.Rotation = Player.Eye.Transform.Rotation;
			}
			else
			{
				Player.Camera.Transform.Position = Player.Eye.Transform.Position + Player.Eye.Transform.Rotation.Backward * 200 + Vector3.Up * 0.0f;
				Player.Camera.Transform.Rotation = Player.Eye.Transform.Rotation;
			}
			Player.Viewmodel.Camera.Transform.Position = Player.Camera.Transform.Position;
			Player.Viewmodel.Camera.Transform.Rotation = Player.Camera.Transform.Rotation;


			//var mdl = Body.Components.Get<SkinnedModelRenderer>();

			//mdl.RenderType = FirstPerson && !mdl.IsProxy ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On;
			//mdl.SceneModel.Flags.IsOpaque = !(FirstPerson && !mdl.IsProxy);
			//mdl.Tint = FirstPerson && !mdl.IsProxy ? Color.Transparent : Color.White;
		}
		else
		{
			Player.Eye.Transform.LocalRotation = EyeAngles.ToRotation();
		}

		foreach ( var i in GameObject.Components.GetAll<ModelRenderer>( FindMode.EverythingInSelfAndDescendants ) )
		{
			if ( i.GameObject.Tags.Has( "viewmodel" ) == false )
			{
				i.RenderType = FirstPerson && !GameObject.IsProxy && GameObject.Network.IsOwner ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On;
			}
		}
	}
}
