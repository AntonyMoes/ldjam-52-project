using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class LevelSelectWindow : GameUIElement {
        [SerializeField] private CanvasGroup _contents;
        [SerializeField] private Transform _levelItemsParent;
        [SerializeField] private LevelItem _levelItemPrefab;
        [SerializeField] private BaseButton _backButton;

        private readonly List<LevelItem> _levelItems = new List<LevelItem>();
        private Action<int> _startLevel;
        private Tween _tween;

        protected override void Init() {
            _backButton.OnClick.Subscribe(OnBackClick);
        }

        public void Load(Action<int> startLevel) {
            _startLevel = startLevel;

            var levels = DataStorage.Instance.Levels;
            var completedLevels = SaveManager.GetInt(SaveManager.IntData.CompletedLevels);

            for (var i = 0; i < levels.Length; i++) {
                var item = Instantiate(_levelItemPrefab, _levelItemsParent);
                item.Load(i, OnItemClick, i <= completedLevels);
                _levelItems.Add(item);
            }
        }

        private void OnBackClick() {
            Hide(() => UIController.Instance.ShowMainMenuWindow());
        }

        private void OnItemClick(int levelIndex) {
            Hide(() => _startLevel?.Invoke(levelIndex));
        }
        
        public override void Clear() {
            foreach (var levelItem in _levelItems) {
                Destroy(levelItem.gameObject);
            }

            _levelItems.Clear();
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