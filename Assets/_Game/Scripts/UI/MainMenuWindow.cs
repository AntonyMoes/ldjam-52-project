using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class MainMenuWindow : GameUIElement {
        [SerializeField] private CanvasGroup _contents;
        [SerializeField] private BaseButton _levelSelectButton;
        [SerializeField] private BaseButton _settingsButton;
        [SerializeField] private RectTransform _center;
        [SerializeField] private RectTransform _emptySpaceCenter;

        public Vector2 EmptySpaceOffset => (_emptySpaceCenter.localPosition - _center.localPosition) / ((RectTransform) transform).rect.size;

        private Action<int> _startLevel;
        private Tween _tween;

        protected override void Init() {
            _levelSelectButton.OnClick.Subscribe(OnLevelSelectClick);
            _settingsButton.OnClick.Subscribe(OnSettingsClick);
        }

        public void Load(Action<int> startLevel) {
            _startLevel = startLevel;
        }

        private void OnLevelSelectClick() {
            Hide(() => UIController.Instance.ShowLevelSelectWindow(_startLevel));
        }

        private void OnSettingsClick() {
            Hide(() => UIController.Instance.ShowCreditsWindow());
        }

        protected override void PerformShow(Action onDone = null) {
            _tween?.Complete(true);

            const float duration = 0.3f;

            _contents.alpha = 0f;

            _tween = DOTween.Sequence() 
                .Insert(0f, _contents.DOFade(1f, duration))
                .AppendCallback(() => {
                    onDone?.Invoke();
                });
        }

        protected override void PerformHide(Action onDone = null) {
            _tween?.Complete(true);

            const float duration = 0.3f;

            _tween = DOTween.Sequence()
                .Insert(0f, _contents.DOFade(0f, duration))
                .AppendCallback(() => {
                    _contents.alpha = 1f;
                    onDone?.Invoke();
                });
        }
    }
}