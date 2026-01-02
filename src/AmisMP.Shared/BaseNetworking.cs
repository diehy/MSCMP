using Steamworks;
using System;

namespace AmisMP.Shared
{
    /// <summary> Base class managing connections and routing. </summary>
    public abstract class BaseNetworking
    {
        /// <summary> Current version of the networking protocol. </summary>
        public const int PROTOCOL_VERSION = 1;
        /// <summary> Network updates per second. </summary>
        public const int TICK_RATE = 20;
        /// <summary> Time between network ticks. </summary>
        public const float TICK_INTERVAL = 1f / TICK_RATE;
        /// <summary> Conservative MTU to avoid fragmentation. </summary>
        public const int MAX_MESSAGE_SIZE = 1024;
        /// <summary> Maximum concurrent peer connections. </summary>
        public const int MAX_CONCURRENT_CONNECTIONS = 12;

        /// <summary> Whether Steamworks interfaces initialized successfully. </summary>
        public bool IsValid { get; private set; }
        /// <summary> Current lobby we're participating in. </summary>
        public CSteamID CurrentLobby { get; private set; }
        /// <summary> Whether we're hosting the current session. </summary>
        public bool IsHost { get; private set; }
        /// <summary> Whether we're currently in an active session. </summary>
        public bool InSession => CurrentLobby.IsValid() && CurrentLobby.IsLobby();

        private CallResult<LobbyCreated_t> crLobbyCreated;
        private CallResult<LobbyEnter_t> crLobbyEntered;
        private Callback<LobbyChatUpdate_t> cbLobbyChatUpdate;
        private Callback<P2PSessionRequest_t> cbP2PSessionRequest;
        private Callback<P2PSessionConnectFail_t> cbP2PSessionConnectFail;

        /// <summary> Calls SteamAPI_Init() for interfaces. </summary>
        protected bool InitializeSteamworksInterfaces()
        {
            // If Steamworks isn't available then we might as well just go kick rocks.
            IsValid = SteamAPI.Init();
            if ( !IsValid ) return false;

            // Create call results.
            crLobbyCreated = CallResult<LobbyCreated_t>.Create( OnLobbyCreatedInternal );
            crLobbyEntered = CallResult<LobbyEnter_t>.Create( OnLobbyEnteredInternal );

            // Create persistent callbacks.
            cbLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create( OnLobbyChatUpdateInternal );
            cbP2PSessionRequest = Callback<P2PSessionRequest_t>.Create( OnP2PSessionRequestInternal );
            cbP2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create( OnP2PSessionConnectFailInternal );
            return true;
        }

        /// <summary> Creates a new Steam peer-to-peer matchmaking lobby. </summary>
        protected void CreateMatchmakingLobby( int maxPlayers, bool isPublic )
        {
            if ( !IsValid ) return;

            IsHost = true;
            maxPlayers = Math.Min( MAX_CONCURRENT_CONNECTIONS, Math.Max( maxPlayers, 2 ) ); // Clamp, just in case.
            crLobbyCreated.Set( SteamMatchmaking.CreateLobby( isPublic ? ELobbyType.k_ELobbyTypePublic : ELobbyType.k_ELobbyTypeFriendsOnly, maxPlayers ) );
        }

        private void OnLobbyCreatedInternal( LobbyCreated_t param, bool bIOFailure )
        {
            throw new NotImplementedException();
        }

        private void OnLobbyEnteredInternal( LobbyEnter_t param, bool bIOFailure )
        {
            throw new NotImplementedException();
        }

        private void OnLobbyChatUpdateInternal( LobbyChatUpdate_t param )
        {
            throw new NotImplementedException();
        }

        private void OnP2PSessionRequestInternal( P2PSessionRequest_t param )
        {
            throw new NotImplementedException();
        }

        private void OnP2PSessionConnectFailInternal( P2PSessionConnectFail_t param )
        {
            throw new NotImplementedException();
        }
        protected virtual void OnSessionCreated() { }
        protected virtual void OnSessionJoined() { }
        protected virtual void OnSessionJoinFailed( string reason ) { }
    }
}
