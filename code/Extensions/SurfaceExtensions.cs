namespace FPSKit;

public static class SurfaceExtensions
{

	public static void DoBulletImpact( this Surface self, SceneTraceResult tr, bool doDecal = true, bool doSound = true, bool doParticle = true, bool doFlavourSound = true )
	{
		var surf = self.GetBaseSurface();
		if ( doDecal )
		{
			//
			// Drop a decal
			//
			var decalPath = Game.Random.FromArray( self.ImpactEffects.BulletDecal );

			while ( string.IsNullOrWhiteSpace( decalPath ) && surf != null )
			{
				decalPath = Game.Random.FromArray( surf.ImpactEffects.BulletDecal );
				surf = surf.GetBaseSurface();
			}

			if ( !string.IsNullOrWhiteSpace( decalPath ) && !tr.Tags.Contains( "water" ) )
			{
				if ( ResourceLibrary.TryGet<DecalDefinition>( decalPath, out var decal ) )
				{
					DropDecal( decal, tr );
				}
			}
		}
		if ( doSound )
		{
			//
			// Make an impact sound
			//
			var sound = self.Sounds.Bullet;

			surf = self.GetBaseSurface();
			while ( string.IsNullOrWhiteSpace( sound ) && surf != null )
			{
				sound = surf.Sounds.Bullet;
				surf = surf.GetBaseSurface();
			}

			if ( !string.IsNullOrWhiteSpace( sound ) )
			{
				Sound.Play( sound, tr.EndPosition );
			}

		}
		if ( doParticle )
		{
			//
			// Get us a particle effect
			//

			// Xenthio self note: Garry's new mesh tracing stuff (when it works) could allow us to tint the flecks of debris to the colour of the material it is coming off, say we shoot red bricks the particle would be coloured red.

			string particleName = Game.Random.FromArray( self.ImpactEffects.Bullet );
			if ( string.IsNullOrWhiteSpace( particleName ) ) particleName = Game.Random.FromArray( self.ImpactEffects.Regular );

			surf = self.GetBaseSurface();
			while ( string.IsNullOrWhiteSpace( particleName ) && surf != null )
			{
				particleName = Game.Random.FromArray( surf.ImpactEffects.Bullet );
				if ( string.IsNullOrWhiteSpace( particleName ) ) particleName = Game.Random.FromArray( surf.ImpactEffects.Regular );

				surf = surf.GetBaseSurface();
			}

			if ( !string.IsNullOrWhiteSpace( particleName ) )
			{
				var ps = LegacyParticle.Create( particleName, tr.HitPosition, tr.Normal.EulerAngles.ToRotation() );
				ps.SetVector( 1, tr.Normal );
				ps.SetVector( 2, tr.Direction );
			}
		}
	}
	static void DropDecal( DecalDefinition decal, SceneTraceResult tr )
	{
		var b = tr.Scene.CreateObject();

		//if ( tr.Body != null && tr.Body.BodyType == PhysicsBodyType.Dynamic )
		//{
		// TODO: parent to bone somehow
		//b.Parent = tr.Body.GetGameObject(); 
		//}

		var localpos = Vector3.Zero;

		if ( tr.GameObject != null )
		{
			b.Parent = tr.GameObject;
		}
		var dc = b.Components.GetOrCreate<DecalRenderer>();

		var decentry = Game.Random.FromList<DecalDefinition.DecalEntry>( decal.Decals );
		dc.Material = decentry.Material;

		var width = decentry.Width.GetValue();
		var height = decentry.Height.GetValue();
		var depth = decentry.Depth.GetValue();

		if ( decentry.KeepAspect ) height = width;
		dc.Size = new Vector3( width, height, depth );

		var rot1 = tr.Normal.EulerAngles.ToRotation();
		rot1 = rot1.RotateAroundAxis( Vector3.Forward, decentry.Rotation.GetValue() );

		var finalrotation = Rotation.LookAt( rot1.Down, rot1.Forward );
		var finalposition = tr.HitPosition + (tr.Normal * (depth / 2));


		if ( tr.GameObject == null )
		{
			b.Transform.Scale = Vector3.One;
			b.Transform.Rotation = finalrotation;
			b.Transform.Position = finalposition;
		}
		else
		{
			//HACKHACK: Workaround for bullshit of the year: scales and local positions being absolutely fucked
			localpos = b.Transform.World.PointToLocal( finalposition );

			b.Transform.Rotation = finalrotation;

			var scale = tr.GameObject.Transform.Scale;
			var locscale = Vector3.One / scale;

			b.Transform.LocalScale = locscale;
			b.Transform.LocalPosition = (localpos * 1);
		}


	}
}
