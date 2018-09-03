using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    public class StateManager : MonoBehaviour
    {
        public int _Health;
        public State _CurrentState;

        [HideInInspector]
        public float _delta;
        [HideInInspector]
        public Transform _Transform;

        private void Start()
        {
            _Transform = this.transform;
        }

        private void Update()
        {
            _delta = Time.deltaTime;
            if (_CurrentState != null)
            {
                _CurrentState.Tick(this);
            }
        }
    }
}