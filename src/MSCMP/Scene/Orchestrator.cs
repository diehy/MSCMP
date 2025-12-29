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
            // Objects.
            var scene = GameObject.Find( "Scene" );
            var userInterface = GameObject.Find( "Interface" );
            var activeInterface = GameObject.Find( "InterfaceActive" );

            // Grab the buttons.
            var buttonStack = userInterface.transform.GetChild( 2 ).gameObject;
            var continueButton = buttonStack.transform.GetChild( 1 ).gameObject;
            var newGameButton = buttonStack.transform.GetChild( 2 ).gameObject;

            // We're going to add a dedicated multiplayer button, so that people aren't forced to play this.
            var multiplayerButton = newGameButton.Instantiate( buttonStack.transform );
            multiplayerButton.name = "MultiplayerButton";

            // Calculate shift value for the top buttons to ensure same spacing.
            var shiftValue = -(newGameButton.transform.localPosition - continueButton.transform.localPosition);

            // Set the new positions.
            continueButton.transform.localPosition += shiftValue;
            newGameButton.transform.localPosition += shiftValue;

            // Set the new text.
            VanillaUtilities.ChangeTextMeshButtonString( multiplayerButton, "MULTIPLAYER" );
        }
    }
}
