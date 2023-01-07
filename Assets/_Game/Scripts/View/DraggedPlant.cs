using System;
using _Game.Scripts.DragAndDrop;
using _Game.Scripts.Model;
using UnityEngine;

namespace _Game.Scripts.View {
    public class DraggedPlant : MonoBehaviour {
        [SerializeField] private SpriteRenderer _plant;
        [SerializeField] private DragComponent _dragComponent;
        public DragComponent DragComponent => _dragComponent;

        public void Load(Plant plant, Drag drag, Action<Plant, DragComponent, DropComponent> onDrop) {
            _plant.sprite = plant.Sprite;
            _dragComponent.SpawnDragged(drag);
            _dragComponent.OnDrop.Subscribe((dragComponent, dropComponent) =>
                onDrop?.Invoke(plant, dragComponent, dropComponent));
        }
    }
}