using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    [CreateAssetMenu(menuName ="Conditions/IsDead")]
    public class IsDead : Condition
    {
        private void OnEnable()
        {
            _Description = "Is health 0 or less?";
        }

        public override bool CheckCondition(StateManager pStateManager)
        {
            return pStateManager._Health <= 0;
        }
    }
}