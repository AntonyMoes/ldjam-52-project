using System;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.DragAndDrop {
    public class Drag {
        private readonly Func<Vector3> _positionProvider;
        private Action<Vector3> _positionSetter;

        private readonly Action _onDrop;
        public readonly Event OnDrop;

        public Drag(Func<Vector3> positionProvider, out Action stopDrag) {
            _positionProvider = positionProvider;
            stopDrag = StopDrag;
            
            OnDrop = new Event(out _onDrop);
        }

        public void SetPositionSetter(Action<Vector3> positionSetter) {
            _positionSetter = positionSetter;
        }

        public void ProcessFrame() {
            _positionSetter?.Invoke( _positionProvider());
        }

        private void StopDrag() {
            _onDrop();
        }
    }
}