// FaceGlassesAnchor.cs
// Purpose: Owns the glasses models on a single tracked face. Instantiates each model once
//          and toggles visibility on swap, so switching is instant with no hitch.
// Attach:  Root of FacePrefab (the prefab assigned to ARFaceManager.facePrefab).

using System.Collections.Generic;
using UnityEngine;

namespace VTO
{
    public class FaceGlassesAnchor : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Empty child transform positioned at the bridge of the nose.")]
        [SerializeField] private Transform _anchor;

        private readonly Dictionary<GlassesItem, GameObject> _instances = new();
        private GameObject _active;

        private void Awake()
        {
            if (_anchor == null)
            {
                Debug.LogError($"[{nameof(FaceGlassesAnchor)}] Anchor not assigned on {name}.", this);
                enabled = false;
            }
        }

        /// <summary>Shows the given glasses, hiding whatever was shown before. Null hides everything.</summary>
        public void Show(GlassesItem item)
        {
            if (_active != null)
                _active.SetActive(false);

            _active = null;

            if (item == null || item.Prefab == null)
                return;

            if (!_instances.TryGetValue(item, out var instance))
            {
                instance = Instantiate(item.Prefab, _anchor);
                instance.transform.localPosition = item.PositionOffset;
                instance.transform.localRotation = Quaternion.Euler(item.RotationOffset);
                instance.transform.localScale = Vector3.one * item.Scale;
                _instances[item] = instance;
            }

            instance.SetActive(true);
            _active = instance;
        }

        /// <summary>Hides/shows the glasses without destroying them (used when tracking is lost).</summary>
        public void SetVisible(bool visible)
        {
            if (_active != null)
                _active.SetActive(visible);
        }
    }
}
