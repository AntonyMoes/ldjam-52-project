using System;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.DragAndDrop {
    public class DropComponent : MonoBehaviour {
        private readonly Action<DragComponent, DropComponent> _onDrop;
        public readonly Event<DragComponent, DropComponent> OnDrop;

        public DropComponent() {
            OnDrop = new Event<DragComponent, DropComponent>(out _onDrop);
        }

        public void OnDragEnd(DragComponent dragComponent) {
            _onDrop(dragComponent, this);
        }
    }
}