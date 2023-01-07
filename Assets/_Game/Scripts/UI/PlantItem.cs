using System;
using _Game.Scripts.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class PlantItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
        [SerializeField] private Image _plant;
        [SerializeField] private TextMeshProUGUI _countText;

        public Plant Plant { get; private set; }

        private Action<Plant, PointerEventData>  _onBeginDrag;
        private Action _onEndDrag;
        
        private int _count;
        public int Count {
            get => _count;
            set {
                _count = value;
                _countText.text = $"x{_count}";
            }
        }

        public void Load(Plant plant, int count, Action<Plant, PointerEventData> onStartDrag, Action onStopDrag) {
            _plant.sprite = plant.Sprite;
            Plant = plant;
            Count = count;
            _onBeginDrag = onStartDrag;
            _onEndDrag = onStopDrag;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            _onBeginDrag?.Invoke(Plant, eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            _onEndDrag?.Invoke();
        }

        public void OnDrag(PointerEventData eventData) { }
    }
}