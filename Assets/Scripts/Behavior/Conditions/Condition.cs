using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    public abstract class Condition : ScriptableObject
    {
        public string _Description;

        public abstract bool CheckCondition(StateManager pStateManager);
    }
}