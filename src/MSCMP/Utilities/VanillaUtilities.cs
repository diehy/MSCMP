using MSCLoader;
using UnityEngine;

namespace MSCMP.Utilities
{
    public static class VanillaUtilities
    {
        /// <summary> Changes the string value used for TextMesh button and attempts to auto scale the interaction boundary. </summary>
        public static void ChangeTextMeshButtonString( GameObject button, string text )
        {
            if ( !button )
            {
                ModConsole.Error( "VanillaUtilities:ChangeTextMeshButtonString() - Button is null!" );
                return;
            }

            // Get the meshes and set the new text.
            var textMeshes = button.GetComponentsInChildren<TextMesh>();
            if ( !string.IsNullOrEmpty( text ) )
            {
                for ( int i = 0; i < textMeshes.Length; i++ )
                {
                    textMeshes[i].text = text;
                }
            }

            // Fetch the mesh renderer.
            var renderer = textMeshes[0].GetComponent<MeshRenderer>();
            if ( !renderer )
            {
                ModConsole.Error( $"VanillaUtilities:ChangeTextMeshButtonString() - First child of {button.name} does not have a mesh renderer!" );
                return;
            }

            // Try to scale the box collider.
            var collider = button.GetComponent<BoxCollider>();
            collider.size = new Vector3( (float)(renderer.bounds.size.x / button.transform.lossyScale.x), 0.08f, 0.01f );
            collider.center = button.transform.InverseTransformPoint( renderer.bounds.center );
        }
    }
}
