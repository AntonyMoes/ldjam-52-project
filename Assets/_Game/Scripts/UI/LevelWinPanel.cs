using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class LevelWinPanel : GameUIElement {
        [SerializeField] private CanvasGroup _contents;
        [SerializeField] private BaseButton _nextLevelButton;
        [SerializeField] private BaseButton _mainMenuButton;

        private Action _startLevel;
        private Action _endLevel;
        private Tween _tween;

        protected override void Init() {
            _nextLevelButton.OnClick.Subscribe(OnNextLevelClick);
            _mainMenuButton.OnClick.Subscribe(OnMainMenuClick);
        }

        public void Load(int? nextLevelIndex, Action endLevel, Action<int> startLevel) {
            _endLevel = endLevel;
            
            if (nextLevelIndex is { } index) {
                _nextLevelButton.gameObject.SetActive(true);
                _startLevel = () => startLevel?.Invoke(index);
            } else {
                _nextLevelButton.gameObject.SetActive(false);
                _startLevel = null;
            }
        }

        private void OnNextLevelClick() {
            Hide(() => _startLevel?.Invoke());
        }

        private void OnMainMenuClick() {
            _endLevel?.Invoke();
            Hide(() => UIController.Instance.ShowMainMenuWindow());
        }

        protected override void PerformShow(Action onDone = null) {
            _tween?.Complete(true);

            const float duration = 0.3f;

            _contents.alpha = 0f;
            var ct = (RectTransform) _contents.transform;
            var initialPosition = ct.anchoredPosition;
            ct.anchoredPosition = initialPosition + Vector2.up * ct.sizeDelta * 0.5f;

            _tween = DOTween.Sequence()
                .Insert(0f, _contents.DOFade(1f, duration))
                .Insert(0f, ct.DOAnchorPos(initialPosition, duration))
                .AppendCallback(() => {
                    _contents.alpha = 1f;
                    ct.anchoredPosition = initialPosition;
                    onDone?.Invoke();
                });
        }

        protected override void PerformHide(Action onDone = null) {
            _tween?.Complete(true);

            const float duration = 0.3f;
            var ct = (RectTransform) _contents.transform;
            var initialPosition = ct.anchoredPosition;

            _tween = DOTween.Sequence()
                .Insert(0f, _contents.DOFade(0f, duration))
                .Insert(0f, ct.DOAnchorPos(initialPosition + Vector2.down * ct.sizeDelta * 0.5f, duration))
                .AppendCallback(() => {
                    _contents.alpha = 1f;
                    ct.anchoredPosition = initialPosition;
                    onDone?.Invoke();
                });
        }
    }
}