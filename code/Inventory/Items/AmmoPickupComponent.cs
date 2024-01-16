namespace FPSKit;

public class AmmoPickupComponent : CollectableComponent
{

	[Property] public AmmoType AmmoType { get; set; }
	[Property] public int AmmoCount { get; set; } = 17;
	public override void OnPickup( PlayerComponent Player )
	{
		Player.Ammo.GiveAmmo( AmmoType, AmmoCount );
	}
}
