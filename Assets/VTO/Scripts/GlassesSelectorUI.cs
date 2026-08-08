// GlassesSelectorUI.cs
// Purpose: Builds the horizontal glasses bar from a list and reports the tapped index.
// Attach:  The "Selector" panel under the Canvas.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace VTO
{
    public class GlassesSelectorUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GlassesThumbnailButton _buttonPrefab;
        [SerializeField] private RectTransform _content;

        private readonly List<GlassesThumbnailButton> _buttons = new();

        /// <summary>Raised with the index of the glasses the user tapped.</summary>
        public event Action<int> SelectionRequested;

        /// <summary>Creates one button per item. Call once at startup.</summary>
        public void Build(IReadOnlyList<GlassesItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                    continue;

                var button = Instantiate(_buttonPrefab, _content);
                button.Initialize(items[i], i, HandleSelectionRequested);
                _buttons.Add(button);
            }
        }

        /// <summary>Moves the highlight to the given index.</summary>
        public void SetSelected(int index)
        {
            for (int i = 0; i < _buttons.Count; i++)
                _buttons[i].SetSelected(i == index);
        }

        private void HandleSelectionRequested(int index) => SelectionRequested?.Invoke(index);
    }
}
