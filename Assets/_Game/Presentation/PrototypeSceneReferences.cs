using UnityEngine;

namespace TerraToss.Presentation
{
    /// <summary>
    /// Holds explicit references to the main prototype scene objects so that no scene-wide runtime
    /// search is ever needed. Populated by the Editor scene builder.
    /// </summary>
    public class PrototypeSceneReferences : MonoBehaviour
    {
        /// <summary>Radius of the prototype Earth sphere, in Unity world units. Single source of truth.</summary>
        public const float EarthRadiusUnits = 5f;

        [SerializeField] private Transform earth;
        [SerializeField] private Transform originMarker;
        [SerializeField] private Transform targetMarker;
        [SerializeField] private Camera prototypeCamera;

        public Transform Earth => earth;
        public Transform OriginMarker => originMarker;
        public Transform TargetMarker => targetMarker;
        public Camera PrototypeCamera => prototypeCamera;

        /// <summary>Assigns the references. Intended to be called by the Editor scene builder.</summary>
        public void SetReferences(Transform earth, Transform originMarker, Transform targetMarker, Camera prototypeCamera)
        {
            this.earth = earth;
            this.originMarker = originMarker;
            this.targetMarker = targetMarker;
            this.prototypeCamera = prototypeCamera;
        }
    }
}
