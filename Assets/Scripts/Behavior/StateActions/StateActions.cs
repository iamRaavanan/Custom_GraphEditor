using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    public abstract class StateActions : ScriptableObject
    {
        public abstract void Execute(StateManager pStateManager);
    }
}