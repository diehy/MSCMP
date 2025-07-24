using MSCMP.Core;
using UnityEngine;

namespace MSCMP
{
    internal static class Bootstrap
    {
        private static void Initialize()
        {
            new GameObject("MSCMP", typeof(Multiplayer));
        }
    }
}
