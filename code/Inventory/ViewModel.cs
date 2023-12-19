﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

/// <summary>
/// A weapon's viewmodel. It's responsibility is to listen to events from a weapon.
/// </summary>
public partial class ViewModel : Component
{
	/// <summary>
	/// A reference to the <see cref="Equippable"/> we want to listen to.
	/// </summary>
	public BaseEquippableComponent Equippable { get; set; }

	/// <summary>
	/// Look up the tree to find the camera.
	/// </summary>
	CameraController CameraController => Components.Get<CameraController>( FindMode.InAncestors );

	protected override void OnStart()
	{
		if ( IsProxy )
		{
			// Disable ourselves if we're proxy. We don't want to see viewmodels of other people's stuff.
			// We might be spectating in the future - so work that out...
			Enabled = false;
		}
	}

	protected override void OnUpdate()
	{
		//var camera = CameraController.Camera;

		// Try to attach
		// if ( camera != null )
		//{
		// Move the ViewModel's gameobject to match the camera position. This won't be a problem once we have camera tags and multiple cameras.
		// Transform.World = camera.Transform.World;
		//}
	}
}
