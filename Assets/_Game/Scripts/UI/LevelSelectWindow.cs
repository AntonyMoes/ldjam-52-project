using System;
using System.Collections.Generic;
using GeneralUtils.UI;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class LevelSelectWindow : UIElement {
        [SerializeField] private Transform _levelItemsParent;
        [SerializeField] private LevelItem _levelItemPrefab;
        [SerializeField] private BaseButton _backButton;

        private readonly List<LevelItem> _levelItems = new List<LevelItem>();
        private Action<int> _startLevel;

        protected override void Init() {
            _backButton.OnClick.Subscribe(OnBackClick);
        }

        public void Load(Action<int> startLevel) {
            _startLevel = startLevel;
        }

        protected override void PerformShow(Action onDone = null) {
            var levels = DataStorage.Instance.Levels;
            var completedLevels = SaveManager.GetInt(SaveManager.IntData.CompletedLevels);

            for (var i = 0; i < levels.Length; i++) {
                var item = Instantiate(_levelItemPrefab, _levelItemsParent);
                item.Load(i, OnItemClick, i <= completedLevels);
                _levelItems.Add(item);
            }

            base.PerformShow(onDone);
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
    }
}