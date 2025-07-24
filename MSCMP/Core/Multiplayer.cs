using UnityEngine;

namespace MSCMP.Core
{
    internal class Multiplayer : MonoBehaviour
    {
        public Multiplayer Instance { get; private set; }

        private void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(5f, 5f, 150f, 22f), "MSCMP");
        }
    }
}
