using System;
using System.Collections.Generic;
using _Game.Scripts.Model;
using GeneralUtils;
using GeneralUtils.UI;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class UIController : SingletonBehaviour<UIController> {
        [SerializeField] private PlantsPanel _plantsPanel;
        [SerializeField] private Transform _hider;
        [SerializeField] private Transform _windows;

        public PlantsPanel ShowPlantsPanel(Dictionary<Plant, int> plants, Plant targetPlant, Action onTargetPlanted) {
            _plantsPanel.Load(plants, targetPlant, onTargetPlanted);
            _plantsPanel.Show();
            return _plantsPanel;
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