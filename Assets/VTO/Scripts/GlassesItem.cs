// GlassesItem.cs
// Purpose: Data definition for one pair of glasses (name, model, thumbnail, fit tuning).
// Usage:   Assets > Create > VTO > Glasses Item. Assign to VTOManager's list.

using UnityEngine;

namespace VTO
{
    [CreateAssetMenu(fileName = "Glasses_", menuName = "VTO/Glasses Item")]
    public class GlassesItem : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string _displayName = "Glasses";
        [SerializeField] private Sprite _thumbnail;

        [Header("Model")]
        [SerializeField] private GameObject _prefab;

        [Header("Fit (relative to the face anchor)")]
        [Tooltip("Fine-tuning offset in metres. Positive Z pushes the glasses away from the head.")]
        [SerializeField] private Vector3 _positionOffset = Vector3.zero;
        [SerializeField] private Vector3 _rotationOffset = Vector3.zero;
        [Tooltip("Uniform scale multiplier. Use this to match the model's real-world size (~0.14 m wide).")]
        [SerializeField] private float _scale = 1f;

        /// <summary>Name shown in UI / logs.</summary>
        public string DisplayName => _displayName;

        /// <summary>Icon shown in the bottom selection bar.</summary>
        public Sprite Thumbnail => _thumbnail;

        /// <summary>3D model instantiated on the tracked face.</summary>
        public GameObject Prefab => _prefab;

        /// <summary>Per-model positional correction applied on top of the shared anchor offset.</summary>
        public Vector3 PositionOffset => _positionOffset;

        /// <summary>Per-model rotational correction, in Euler degrees.</summary>
        public Vector3 RotationOffset => _rotationOffset;

        /// <summary>Per-model uniform scale multiplier.</summary>
        public float Scale => _scale;
    }
}
