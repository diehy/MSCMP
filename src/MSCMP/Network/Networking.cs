using AmisMP.Shared;
using MSCLoader;
using MSCMP.Game.Components.UI;
using UnityEngine;

namespace MSCMP.Network
{
    /// <summary> Network manager for this client. </summary>
    public class Networking : BaseNetworking
    {
        public Networking()
        {
            // Most of the time this will fail because the user is running a pirated game.
            if ( !InitializeSteamworksInterfaces() )
            {
                ModUI.ShowMessage( "<color=orange>Error: Failed to initialize Steamworks interfaces!</color>", "MSCMP" );
                return;
            }
        }

        /// <summary> Begin hosting a new P2P session. </summary>
        public void HostSession( int maxPlayers, bool isPublic )
        {
            // Ensure we're not already hosting.
            if ( IsHost ) return;

            // Show the loading screen and call the matchmaking API.
            GameObject.Find( "Loading" ).GetComponentInParent<DynamicLoadingScreen>().Show( "CREATING SESSION..." );
            CreateMatchmakingLobby( maxPlayers, isPublic );
        }
    }
}
