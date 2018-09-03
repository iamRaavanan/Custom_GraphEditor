using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    [CreateAssetMenu(menuName ="Actions/Test/Add Health")]
    public class ChangeHealth : StateActions
    {
        public override void Execute(StateManager pStateManager)
        {
            pStateManager._Health += 10;
        }
    }
}