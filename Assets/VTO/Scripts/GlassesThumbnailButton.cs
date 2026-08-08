// GlassesThumbnailButton.cs
// Purpose: A single tappable thumbnail in the bottom selector.
// Attach:  Root of GlassesThumbnailButton prefab (needs a Button component).

using System;
using UnityEngine;
using UnityEngine.UI;

namespace VTO
{
    [RequireComponent(typeof(Button))]
    public class GlassesThumbnailButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _selectionFrame;

        private Button _button;
        private int _index;
        private Action<int> _onClicked;

        private void Awake() => _button = GetComponent<Button>();

        /// <summary>Fills the button with data and registers the tap callback.</summary>
        public void Initialize(GlassesItem item, int index, Action<int> onClicked)
        {
            _index = index;
            _onClicked = onClicked;

            if (_icon != null)
                _icon.sprite = item.Thumbnail;

            _button.onClick.AddListener(HandleClick);
            SetSelected(false);
        }

        /// <summary>Toggles the highlight frame.</summary>
        public void SetSelected(bool selected)
        {
            if (_selectionFrame != null)
                _selectionFrame.SetActive(selected);
        }

        private void OnDestroy() => _button.onClick.RemoveListener(HandleClick);

        private void HandleClick() => _onClicked?.Invoke(_index);
    }
}
