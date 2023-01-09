using System;
using _Game.Scripts.Model;
using _Game.Scripts.UI.PlantInfo;
using DG.Tweening;
using GeneralUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class PlantItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
        [SerializeField] private Image _backGround;
        [SerializeField] private Image _plant;
        [SerializeField] private TextMeshProUGUI _countText;

        public Plant Plant { get; private set; }
        
        private readonly UpdatedValue<bool> _onHover = new UpdatedValue<bool>(false);
        private Tween _scaleTween;
        private PlantInfoPanel _infoPanel;

        private Action<Plant, PointerEventData>  _onBeginDrag;
        private Action _onEndDrag;
        
        private int _count;
        private bool _main;

        public int Count {
            get => _count;
            set {
                _count = value;
                _countText.text = $"x{_count}";
            }
        }

        private void Awake() {
            HoverComponent.Create(_backGround, _onHover);
            _onHover.Subscribe(OnItemHover, true);
        }

        public void Load(Plant plant, int? count, Action<Plant, PointerEventData> onStartDrag, Action onStopDrag, bool main) {
            _plant.sprite = plant.Sprite;
            _main = main;
            Plant = plant;

            if (count is {} c) {
                Count = c;
            } else {
                _countText.text = string.Empty;
            }

            _onBeginDrag = onStartDrag;
            _onEndDrag = onStopDrag;
        }

        private void OnItemHover(bool hovering) {
            _scaleTween?.Kill();

            if (hovering) {
                _infoPanel = UIController.Instance.ShowPlantInfoPanel(Plant, _main);
            } else if (_infoPanel != null) {
                _infoPanel.TryHide();
            } else {
                return;
            }

            var parentRect = (RectTransform) transform.parent;
            void RebuildParent() => LayoutRebuilder.MarkLayoutForRebuild(parentRect);

            const float maxScale = 1.1f;
            const float duration = 0.2f;

            var scale = (hovering ? maxScale : 1f) * Vector3.one;
            DOTween.Sequence()
                .Insert(0f, transform.DOScale(scale, duration))
                .Insert(0f, DOVirtual.Float(0, duration, duration, _ => RebuildParent()));

        }

        public void OnBeginDrag(PointerEventData eventData) {
            _onBeginDrag?.Invoke(Plant, eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            _onEndDrag?.Invoke();
        }

        public void OnDrag(PointerEventData eventData) { }

        public void Clear() {
            _scaleTween?.Complete();
        }
    }
}