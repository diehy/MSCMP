using UnityEngine;

namespace MSCMP.Game.Components.UI
{
    /// <summary> Allows the loading screen text to be toggled and updated on the fly. </summary>
    public class DynamicLoadingScreen : MonoBehaviour
    {
        private TextMesh loadingText;
        private GameObject loadingCamera;

        // Note that you must call GetComponentInParent when using GameObject.Find("Loading") as it will return the child object for it.
        private void Awake()
        {
            loadingCamera = transform.GetChild( 1 ).gameObject;
            loadingText = transform.GetChild( 2 ).GetComponent<TextMesh>();
            Hide();
        }

        /// <summary> Enables the loading screen and sets the specified text. </summary>
        public void Show( string text )
        {
            loadingCamera?.gameObject.SetActive( true );
            SetText( text );
        }

        /// <summary> Disables the loading screen. </summary>
        public void Hide() => loadingCamera?.gameObject.SetActive( false );

        /// <summary> Changes the currently displayed text in the loading screen. </summary>
        public void SetText( string text )
        {
            if ( string.IsNullOrEmpty( text ) ) return;
            loadingText.text = text;
        }
    }
}
