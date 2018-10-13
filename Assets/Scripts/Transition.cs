using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    [System.Serializable]
    public class Transition
    {
        public int                  _Id;
        public Condition            _Condition;
        public State                _TargetState;
        public bool                 _Disable;
    }
}