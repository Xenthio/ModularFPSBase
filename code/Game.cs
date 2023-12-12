using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace FPSKit;
public class GameController : GameObject
{ 
	public GameController()
	{
		Enabled = true;
		Parent = GameManager.ActiveScene;
		Components.GetOrCreate<NetworkManager>();
	}
}
public class NetworkManager : Component, Component.INetworkListener
{
	[Property] public GameObject PlayerPrefab { get; set; }
	[Property] public GameObject SpawnPoint { get; set; }
	public void OnActive( Connection connection )
	{
		Log.Info( $"Player '{connection.DisplayName}' is becoming active" );

		var player = SceneUtility.Instantiate( PlayerPrefab, SpawnPoint.Transform.World );

		//var nameTag = player.Components.Get<NameTagPanel>( FindMode.EverythingInSelfAndDescendants );
		//if ( nameTag is not null )
		//{
		//	nameTag.Name = channel.DisplayName;
		//}

		player.Network.Spawn( connection );
	}
}
