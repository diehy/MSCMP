using MSCLoader;
using Steamworks;
using System;
using UnityEngine;

namespace MSCMP.Network
{
    /// <summary> Underlying network manager responsible for session connections and routing. </summary>
    public static class Networking
    {
        /// <summary> Current version of the networking stack. </summary>
        public const int PROTOCOL_VERSION = 1;
        /// <summary> The rate at which network updates are sent. </summary>
        public const int TICK_RATE = 20;
        /// <summary> The rate at which network updates are sent. </summary>
        public const float TICK_INTERVAL = 1f / TICK_RATE;
        /// <summary> Conservative MTU to avoid fragmentation. </summary>
        public const int MAX_PACKET_BUFFER_SIZE = 1024;
        /// <summary> The maximum number of connections allowed at once. </summary>
        public const int MAX_CONCURRENT_CONNECTIONS = 12;

        /// <summary> Indicates whether Steamworks interfaces are available. </summary>
        public static bool IsValid { get; private set; }
        /// <summary> Current lobby we're in, if any. </summary>
        public static CSteamID CurrentLobby { get; private set; }
        /// <summary> Whether we're the host of the current session. </summary>
        public static bool IsHost { get; private set; }
        /// <summary> Whether we're currently in a session. </summary>
        public static bool InSession => CurrentLobby.IsValid() && CurrentLobby.IsLobby();

        // Steamworks CB/CR.
        private static CallResult<LobbyCreated_t> crLobbyCreated;
        private static CallResult<LobbyEnter_t> crLobbyEntered;

        internal static void Initialize()
        {
            // Initialize Steamworks interfaces.
            IsValid = SteamAPI.Init();
            if ( !IsValid )
            {
                ModConsole.Error( "Steamworks interfaces failed to initialize!" );
                return;
            }

            // Create call results and callbacks.
            crLobbyCreated = CallResult<LobbyCreated_t>.Create( OnSessionCreated );
            crLobbyEntered = CallResult<LobbyEnter_t>.Create( OnSessionJoined );
        }

        /// <summary> Begin hosting a new P2P session. </summary>
        internal static void HostSession( int maxCapacity, bool isPublic )
        {
            if ( !IsValid ) return;
            if ( InSession )
            {
                ModConsole.Error( "Already in a session! You must disconnect before hosting a new session." );
                return;
            }

            IsHost = true;

            // Call matchmaking API.
            crLobbyCreated.Set( SteamMatchmaking.CreateLobby( isPublic ? ELobbyType.k_ELobbyTypePublic : ELobbyType.k_ELobbyTypePrivate, maxCapacity ) );
        }

        /// <summary> Called when we've received a response from matchmaking API. </summary>
        private static void OnSessionCreated( LobbyCreated_t param, bool bIOFailure )
        {
            // Handle failure.
            if ( bIOFailure || param.m_eResult != EResult.k_EResultOK )
            {
                IsHost = false;
                ModUI.ShowMessage( $"Failed to host a new session!\nReason: {(bIOFailure ? "IO failure occurred." : param.m_eResult.ToString())}", "MSCMP" );
                return;
            }

            CurrentLobby = new CSteamID( param.m_ulSteamIDLobby );

            // Set lobby metadata.
            SteamMatchmaking.SetLobbyData( CurrentLobby, "protocol", PROTOCOL_VERSION.ToString() );
            Application.LoadLevelAsync( "GAME" );
        }

        private static void OnSessionJoined( LobbyEnter_t param, bool bIOFailure )
        {
            throw new NotImplementedException();
        }

        /// <summary> Runs the network loop. </summary>
        internal static void Update()
        {
            SteamAPI.RunCallbacks();
        }
    }
}
