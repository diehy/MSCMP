using MSCLoader;
using MSCMP.Network;
using MSCMP.Scene;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MSCMP
{
    /// <summary> Main mod class. </summary>
    public class MSCMP : Mod
    {
        /// <summary> Unique mod ID for this. </summary>
        public override string ID => "MSCMP";
        /// <summary> Full mod name. </summary>
        public override string Name => "My Summer Car - Multiplayer";
        /// <summary> Local version. </summary>
        public override string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        /// <summary> Main author/contributors. </summary>
        public override string Author => "diehy";
        /// <summary> Short description of the mod. </summary>
        public override string Description => "Cooperative multiplayer mod for My Summer Car.";

        /// <summary> Network manager for this client. </summary>
        public Networking NetManager { get; private set; }
        /// <summary> Singleton instance. </summary>
        public static MSCMP Instance { get; private set; }

        /// <summary> Container object holding any MonoBehaviours not attached to vanilla objects. </summary>
        private GameObject container;

        /// <summary> Initialization and configuration is performed here. </summary>
        public override void ModSetup()
        {
            // We shouldn't run alongside other MP mods (because that's just a disaster waiting to happen).
            if ( !ValidateMultiplayerCompatibility( out var otherMpMod ) )
            {
                var sb = new StringBuilder();
                sb.AppendLine( $"<color=orange>Error: Mod conflict detected with <color=cyan>{otherMpMod}</color>!</color>" );
                sb.Append( "MSCMP cannot be run alongside other multiplayer mods. Do not use external launchers and ensure UnityDoorstop only loads MSCModLoader before trying again!" );
                ModUI.ShowMessage( sb.ToString(), "MSCMP" );
                return;
            }

            // Register lifecycle hooks.
            SetupFunction( Setup.OnMenuLoad, Orchestrator.OnMainMenuLoaded );
            SetupFunction( Setup.Update, OnUpdate );

            // Create the network manager.
            NetManager = new Networking();
        }

        /// <summary> Main update loop. </summary>
        private void OnUpdate()
        {
        }

        /// <summary> Detects whether another multiplayer mod is running. </summary>
        private bool ValidateMultiplayerCompatibility( out string otherMpMod )
        {
            if ( GameObject.Find( "MSCO UI" ) || GameObject.Find( "MSCO Controller" ) )
            {
                otherMpMod = "MSCO";
                return false;
            }
            else if ( GameObject.Find( "WreckMP" ) )
            {
                otherMpMod = "WreckMP";
                return false;
            }

            otherMpMod = null;
            return true;
        }
    }
}
