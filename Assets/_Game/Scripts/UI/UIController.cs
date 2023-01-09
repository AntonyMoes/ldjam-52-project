using System;
using System.Collections.Generic;
using _Game.Scripts.DragAndDrop;
using _Game.Scripts.Model;
using _Game.Scripts.UI.PlantInfo;
using GeneralUtils;
using GeneralUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class UIController : SingletonBehaviour<UIController> {
        [SerializeField] private MainMenuWindow _mainMenuWindow;
        [SerializeField] private LevelSelectWindow _levelSelectWindow;
        [SerializeField] private CreditsWindow _creditsWindow;
        [SerializeField] private PlantsPanel _plantsPanel;
        [SerializeField] private ResourceShowPanel _resourceShowPanel;
        [SerializeField] private LevelWinPanel _levelWinPanel;
        [SerializeField] private RestartPanel _restartPanel;
        [SerializeField] private MainMenuPanel _mainMenuPanel;
        [SerializeField] private PlantInfoPanel _plantInfoPanel;

        [SerializeField] private Transform _hider;
        [SerializeField] private Transform _windows;

        public MainMenuWindow ShowMainMenuWindow(Action<int> startLevel = null) {
            if (startLevel != null) {
                _mainMenuWindow.Load(startLevel);
            }

            _mainMenuWindow.Show();
            return _mainMenuWindow;
        }

        public LevelSelectWindow ShowLevelSelectWindow(Action<int> startLevel) {
            _levelSelectWindow.Load(startLevel);
            _levelSelectWindow.Show();
            return _levelSelectWindow;
        }

        public CreditsWindow ShowCreditsWindow() {
            _creditsWindow.Show();
            return _creditsWindow;
        }

        public PlantsPanel ShowPlantsPanel(Dictionary<Plant, int> plants, Plant targetPlant, Action<Plant, DragComponent> onDrag, Action<Plant, DragComponent, DropComponent> onDrop) {
            _plantsPanel.Load(plants, targetPlant, onDrag, onDrop);
            _plantsPanel.Show();
            return _plantsPanel;
        }

        public ResourceShowPanel ShowResourceShowPanel() {
            _resourceShowPanel.Show();
            return _resourceShowPanel;
        }

        public LevelWinPanel ShowLevelWinPanel(int? nextLevelIndex, Action endLevel, Action<int> startLevel) {
            _levelWinPanel.Load(nextLevelIndex, endLevel, startLevel);
            _levelWinPanel.Show();
            return _levelWinPanel;
        }

        public MainMenuPanel ShowMainMenuPanel(Action endLevel) {
            _mainMenuPanel.Load(endLevel);
            _mainMenuPanel.Show();
            return _mainMenuPanel;
        }

        public RestartPanel ShowRestartPanel(Action restart) {
            _restartPanel.Load(restart);
            _restartPanel.Show();
            return _restartPanel;
        }

        public PlantInfoPanel ShowPlantInfoPanel(Plant plant, bool main) {
            _plantInfoPanel.Load(plant, main);
            _plantInfoPanel.Show();
            return _plantInfoPanel;
        }

        private void PrepareWindow(UIElement window) {
            window.OnShowing.Unsubscribe(OnShowing);
            window.OnShowing.Subscribe(OnShowing);
            window.OnHiding.Unsubscribe(OnHiding);
            window.OnHiding.Subscribe(OnHiding);

            void OnShowing() {
                window.OnHiding.Unsubscribe(OnShowing);
                ShowWindow(window);
            }

            void OnHiding() {
                window.OnHiding.Unsubscribe(OnHiding);
                HideWindow(window);
            }
        }

        private void ShowWindow(UIElement window) {
            _hider.gameObject.SetActive(true);
            _hider.SetAsLastSibling();
            window.transform.SetAsLastSibling();
        }

        private void HideWindow(UIElement window) {
            UIElement lastActiveWindow = null;
            foreach (Transform child in _windows) {
                if (child.gameObject.activeSelf && child.TryGetComponent<UIElement>(out var activeWindow) && activeWindow != window)
                    lastActiveWindow = activeWindow;
            }

            if (lastActiveWindow != null) {
                _hider.SetSiblingIndex(lastActiveWindow.transform.GetSiblingIndex());
            } else {
                _hider.gameObject.SetActive(false);
            }
        }
    }
}