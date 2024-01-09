using Sandbox.Citizen;

namespace FPSKit;

public class PlayerBodyComponent : Component
{
	[Property] public SkinnedModelRenderer Model { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }
	/// <summary>
	/// Should be disabled by default, used on death to ragdoll
	/// </summary>
	[Property] public ModelPhysics Physics { get; set; }
}
