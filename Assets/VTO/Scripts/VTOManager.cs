// VTOManager.cs
// Purpose: Application glue. Requests camera permission, listens to AR face tracking,
//          applies the selected glasses to the tracked face, and drives the status text.
// Attach:  An empty GameObject named "VTOManager" in the scene.
// Depends: ARFaceManager (on XR Origin), GlassesSelectorUI, TextMeshProUGUI status label.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace VTO
{
    public class VTOManager : MonoBehaviour
    {
        [Header("AR References")]
        [SerializeField] private ARFaceManager _faceManager;
        [SerializeField] private ARCameraManager _cameraManager;

        [Header("UI References")]
        [SerializeField] private GlassesSelectorUI _selector;
        [SerializeField] private TextMeshProUGUI _statusLabel;

        [Header("Content")]
        [SerializeField] private List<GlassesItem> _glasses = new();
        [SerializeField] private int _defaultIndex;

        [Header("Messages")]
        [SerializeField] private string _noFaceMessage = "Move your face into view";
        [SerializeField] private string _noPermissionMessage = "Camera permission is required.\nEnable it in Settings and restart the app.";
        [SerializeField] private string _unsupportedMessage = "This device does not support AR face tracking.";

        private FaceGlassesAnchor _activeAnchor;
        private int _selectedIndex;
        private bool _hasPermission;

        private void Awake()
        {
            if (_faceManager == null || _selector == null || _statusLabel == null)
            {
                Debug.LogError($"[{nameof(VTOManager)}] Missing Inspector references.", this);
                enabled = false;
                return;
            }

            _selectedIndex = Mathf.Clamp(_defaultIndex, 0, Mathf.Max(0, _glasses.Count - 1));
        }

        private void OnEnable() => _faceManager.trackablesChanged.AddListener(OnFacesChanged);

        private void OnDisable() => _faceManager.trackablesChanged.RemoveListener(OnFacesChanged);

        private IEnumerator Start()
        {
            // The front camera is what face tracking needs; set it before the session configures itself.
            if (_cameraManager != null)
                _cameraManager.requestedFacingDirection = CameraFacingDirection.User;

            _selector.Build(_glasses);
            _selector.SelectionRequested += OnSelectionRequested;
            _selector.SetSelected(_selectedIndex);

            yield return RequestCameraPermission();

            if (!_hasPermission)
            {
                SetStatus(_noPermissionMessage);
                yield break;
            }

            if (!IsFaceTrackingSupported())
            {
                SetStatus(_unsupportedMessage);
                yield break;
            }

            SetStatus(_noFaceMessage);
        }

        private void OnDestroy()
        {
            if (_selector != null)
                _selector.SelectionRequested -= OnSelectionRequested;
        }

        private void OnFacesChanged(ARTrackablesChangedEventArgs<ARFace> args)
        {
            foreach (var face in args.added)
                BindFace(face);

            foreach (var face in args.updated)
            {
                if (face == _activeAnchor?.GetComponentInParent<ARFace>())
                    UpdateTrackingState(face);
            }

            foreach (var removed in args.removed)
            {
                if (_activeAnchor != null && removed.Value == _activeAnchor.GetComponentInParent<ARFace>())
                {
                    _activeAnchor = null;
                    SetStatus(_noFaceMessage);
                }
            }
        }

        private void BindFace(ARFace face)
        {
            if (!face.TryGetComponent(out FaceGlassesAnchor anchor))
                anchor = face.GetComponentInChildren<FaceGlassesAnchor>();

            if (anchor == null)
            {
                Debug.LogError($"[{nameof(VTOManager)}] FacePrefab is missing a {nameof(FaceGlassesAnchor)}.", this);
                return;
            }

            _activeAnchor = anchor;
            _activeAnchor.Show(GetSelected());
            UpdateTrackingState(face);
        }

        private void UpdateTrackingState(ARFace face)
        {
            bool tracking = face.trackingState == TrackingState.Tracking;
            _activeAnchor?.SetVisible(tracking);
            SetStatus(tracking ? string.Empty : _noFaceMessage);
        }

        private void OnSelectionRequested(int index)
        {
            if (index < 0 || index >= _glasses.Count || index == _selectedIndex)
                return;

            _selectedIndex = index;
            _selector.SetSelected(index);
            _activeAnchor?.Show(GetSelected());
        }

        private GlassesItem GetSelected() =>
            _glasses.Count == 0 ? null : _glasses[_selectedIndex];

        private void SetStatus(string message)
        {
            _statusLabel.text = message;
            _statusLabel.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }

        private static bool IsFaceTrackingSupported()
        {
#if UNITY_EDITOR
            return true; // Nothing to check in the Editor; face tracking only runs on device.
#else
            var loader = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader;
            return loader != null && loader.GetLoadedSubsystem<XRFaceSubsystem>() != null;
#endif
        }

        private IEnumerator RequestCameraPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                _hasPermission = true;
                yield break;
            }

            Permission.RequestUserPermission(Permission.Camera);

            // The dialog pauses the app; poll briefly until the user answers.
            const float timeout = 30f;
            float elapsed = 0f;
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera) && elapsed < timeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            _hasPermission = Permission.HasUserAuthorizedPermission(Permission.Camera);
#else
            _hasPermission = true;
            yield break;
#endif
        }
    }
}
