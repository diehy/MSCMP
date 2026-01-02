using MSCMP.Extensions;
using MSCMP.Utilities;
using UnityEngine;

namespace MSCMP.Scene
{
    /// <summary> Scene specific modifications needed for multiplayer. </summary>
    internal static class Orchestrator
    {
        /// <summary> Called when the main menu scene loads. </summary>
        public static void OnMainMenuLoaded()
        {
            // Main scene objects we need.
            var scene = GameObject.Find( "Scene" );
            var userInterface = GameObject.Find( "Interface" );
            var activeInterface = GameObject.Find( "InterfaceActive" );

            // Grab the buttons.
            var buttonStack = userInterface.transform.GetChild( 2 ).gameObject;
            var continueButton = buttonStack.transform.GetChild( 1 ).gameObject;
            var newGameButton = buttonStack.transform.GetChild( 2 ).gameObject;

            // Add a dedicated multiplayer button so that people aren't forced to play this.
            var multiplayerButton = newGameButton.Instantiate( buttonStack.transform );
            multiplayerButton.name = "MultiplayerButton";

            // Shift the top two buttons up with identical spacing.
            var shiftValue = -(newGameButton.transform.localPosition - continueButton.transform.localPosition);
            continueButton.transform.localPosition += shiftValue;
            newGameButton.transform.localPosition += shiftValue;

            // Set the new button text.
            VanillaUtilities.ChangeTextMeshButtonString( multiplayerButton, "MULTIPLAYER" );
        }
    }
}
