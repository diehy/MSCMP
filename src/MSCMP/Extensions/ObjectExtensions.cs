using UnityEngine;

namespace MSCMP.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary> Instantiates a clone as a child of the specified parent. </summary>
        public static GameObject Instantiate( this GameObject original, Transform parent, bool worldPositionStays = false )
        {
            var clone = Object.Instantiate( original );
            clone.transform.SetParent( parent, worldPositionStays );
            return clone;
        }

        /// <summary> Instantiates a clone at the specified local position and rotation. </summary>
        public static GameObject Instantiate( this GameObject original, Transform parent, Vector3 localPosition, Quaternion localRotation )
        {
            var clone = original.Instantiate( parent, false );
            clone.transform.localPosition = localPosition;
            clone.transform.localRotation = localRotation;
            return clone;
        }

        /// <summary> Copies transform values from another object. </summary>
        public static void CopyFrom( this Transform target, Transform source )
        {
            target.position = source.position;
            target.rotation = source.rotation;
            target.localScale = source.localScale;
        }

        /// <summary> Copies transform values from another object in local space. </summary>
        public static void CopyLocalFrom( this Transform target, Transform source )
        {
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }
    }
}
