using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MSCMP
{
    /// <summary>
    /// Main class of the mod.
    /// </summary>
    public class Client
    {

        /// <summary>
        /// Asset bundle containing multiplayer mod content.
        /// </summary>
        static AssetBundle assetBundle = null;

        /// <summary>
        /// The my summer car game app id.
        /// </summary>
        public static readonly Steamworks.AppId_t GAME_APP_ID = new Steamworks.AppId_t( 516750 );

        public static void Init()
        {
            ShowMessageBox( "Shut up", "Gremlin" );
            if ( !SetupLogger() )
            {
                return;
            }
        }

        /// <summary>
        /// Starts the mod. Called from Injector.
        /// </summary>
        public static void Start()
        {
            if ( !SetupLogger() )
            {
                return;
            }

            Logger.SetAutoFlush( true );

            Game.Hooks.PlayMakerActionHooks.Install();

            string assetBundlePath = null;
            if ( !File.Exists( assetBundlePath ) )
            {
                FatalError( "Cannot find mpdata asset bundle." );
                return;
            }

            assetBundle = AssetBundle.CreateFromFile( assetBundlePath );

            var go = new GameObject( "Multiplayer GUI Controller" );
            go.AddComponent<UI.MPGUI>();

            go = new GameObject( "Multiplayer Controller" );
            go.AddComponent<MPController>();

            UI.Console.RegisterCommand( "quit", ( string[] args ) =>
            {
                Application.Quit();
            } );
        }

        /// <summary>
        /// Gets absolute path for the specified file relative to mod installation folder.
        /// </summary>
        /// <param name="file">The file to get path for.</param>
        /// <returns>Absolute path for the specified file relative to mod instalation folder.</returns>
        public static string GetPath( string file )
        {
            return Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\" + file;
        }

        /// <summary>
        /// Loads asset from multiplayer mod asset bundle.
        /// </summary>
        /// <typeparam name="T">The type of the asset to load.</typeparam>
        /// <param name="name">The name of the asset to load.</param>
        /// <returns>Loaded asset.</returns>
        public static T LoadAsset<T>( string name ) where T : UnityEngine.Object
        {
            return assetBundle.LoadAsset<T>( name );
        }

        /// <summary>
        /// Call this when fatal error occurs. This will print error into the log and close the game.
        /// </summary>
        /// <param name="message">The message to print to console.</param>
        public static void FatalError( string message )
        {
            Logger.Log( message );
            Logger.Log( Environment.StackTrace );
            ShowMessageBox( message, "MSCMP - Fatal error" );

#if DEBUG
            if ( Debugger.IsAttached )
            {
                throw new Exception( message );
            }
            else
            {
#endif
                Process.GetCurrentProcess().Kill();
#if DEBUG
            }
#endif
        }

        /// <summary>
        /// Standard assertion. If given condition is not true then prints message to the log and closes game.
        /// </summary>
        /// <param name="condition">Condition to chec.</param>
        /// <param name="message">The message to print to console.</param>
        public static void Assert( bool condition, string message )
        {
            if ( condition )
            {
                return;
            }
            Logger.Log( "[ASSERTION FAILED]" );
            FatalError( message );
        }

        /// <summary>
        /// Shows system message box to the user. Should be used only during initialization when no ui can be shown in game.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="title">The title of the message box.</param>
        public static void ShowMessageBox( string message, string title )
        {
            MessageBox( IntPtr.Zero, message, title, (uint)0x00000010L );
        }

        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern int MessageBox( IntPtr hWnd, String text, String caption, uint type );

        /// <summary>
        /// The current mod development stage.
        /// </summary>
        public const string MOD_DEVELOPMENT_STAGE = "Pre-Alpha";

        /// <summary>
        /// Get display version of the mod.
        /// </summary>
        /// <returns></returns>
        public static string GetMODDisplayVersion()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version += " " + MOD_DEVELOPMENT_STAGE;
            return version;
        }

        /// <summary>
        /// Add message to the console.
        /// </summary>
        /// <param name="message">The message to add.</param>
        static public void ConsoleMessage( string message )
        {
            if ( UI.Console.Instance != null )
            {
                UI.Console.Instance.AddMessage( message );
            }
        }

        /// <summary>
        /// Initializes logger.
        /// </summary>
        /// <returns>true if logger initialization has succeeded, false otherwise</returns>
        static private bool SetupLogger()
        {
            string logPath;

            // First try create clientLog in app data.

            string appData = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
            string mscmpData = appData + "/MSCMP";
            bool mscmpDataExists = Directory.Exists( mscmpData );
            if ( !mscmpDataExists )
            {
                try
                {
                    mscmpDataExists = Directory.CreateDirectory( mscmpData ).Exists;
                }
                catch
                {
                    // Nothing.. let us fallback below.
                }
            }

            if ( mscmpDataExists )
            {
                logPath = mscmpData + "/clientLog.txt";
                if ( Logger.SetupLogger( logPath ) )
                {
                    return true;
                }
            }

            // The last chance, setup logger next to the .exe.

            logPath = GetPath( "clientLog.txt" );
            if ( !Logger.SetupLogger( logPath ) )
            {
                FatalError( $"Cannot create log file. Log file path: {logPath}\n\nTry running game as administrator." );
                return false;
            }

            return true;
        }

    }
}
