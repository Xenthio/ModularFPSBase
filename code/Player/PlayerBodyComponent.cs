using Sandbox.Citizen;

namespace FPSKit;

public class PlayerBodyComponent : Component
{
	[Property] public SkinnedModelRenderer Model { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }
}
