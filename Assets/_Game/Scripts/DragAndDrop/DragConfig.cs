using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.DragAndDrop {
    public class DragConfig : SingletonBehaviour<DragConfig> {
        [SerializeField] private Transform _dragLayer;
        public Transform DragLayer => _dragLayer;
        [SerializeField] private Transform _dropLayer;
        public Transform DropLayer => _dropLayer;
    }
}