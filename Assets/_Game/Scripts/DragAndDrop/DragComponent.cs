using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.DragAndDrop {
    public class DragComponent : MonoBehaviour {
        private readonly UpdatedValue<bool> _dragging = new UpdatedValue<bool>();
        public IUpdatedValue<bool> Dragging => _dragging;

        private Drag _drag;
        private Action _stopDrag;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[10];

        private readonly Action<DragComponent, DropComponent> _onDrop;
        public readonly Event<DragComponent, DropComponent> OnDrop;

        public static Vector3 MouseWorldPoint => Camera.main.ScreenToWorldPoint(Input.mousePosition);

        public DragComponent() {
            OnDrop = new Event<DragComponent, DropComponent>(out _onDrop);

            Dragging.Subscribe(SetLayer);
        }

        private void SetLayer(bool dragging) {
            transform.SetParent(!dragging ? DragConfig.Instance.DropLayer : DragConfig.Instance.DragLayer);
        }

        public void SpawnDragged(Drag drag) {
            RegisterDrag(drag);
        }

        private void RegisterDrag(Drag drag) {
            _drag = drag;
            _dragging.Value = true;
            _drag.SetPositionSetter(SetPosition);
            _drag.OnDrop.Subscribe(OnDragStop);
        }

        private void SetPosition(Vector3 worldPoint) {
            worldPoint.z = transform.position.z;
            transform.position = worldPoint;
        }

        private void OnDragStop() {
            _drag = null;
            _dragging.Value = false;

            var drop = GetDraggedOver<DropComponent>().FirstOrDefault();

            _onDrop(this, drop);
            if (drop != null) {
                drop.OnDragEnd(this);
            }
        }

        public IEnumerable<T> GetDraggedOver<T>() {
            var hitCount = Physics2D.RaycastNonAlloc(MouseWorldPoint, Vector2.zero, _hits);
            return _hits
                .Take(hitCount)
                .Where(h => h.transform.TryGetComponent<T>(out _))
                .Select(h => h.transform.GetComponent<T>());
        }

        private void OnMouseDrag() {
            if (_drag == null) {
                RegisterDrag(new Drag(() => MouseWorldPoint, out _stopDrag));
            }
        }

        private void Update() {
            _drag?.ProcessFrame();
        }
    
        private void OnMouseUp() {
            _stopDrag?.Invoke();
            _stopDrag = null;
        }
    }
}