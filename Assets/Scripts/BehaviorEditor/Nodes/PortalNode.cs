using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Raavanan.GraphEditor
{
    [CreateAssetMenu(menuName = "Editor/Nodes/Portal Node")]
    public class PortalNode : DrawNode
    {
        public override void DrawCurve(BaseNode pBaseNode)
        {
            
        }

        public override void DrawWindow(BaseNode pBaseNode)
        {
            pBaseNode._StateNodeRef._CurrentState = (State)EditorGUILayout.ObjectField(pBaseNode._StateNodeRef._CurrentState, typeof(State), false);
            pBaseNode._IsAssigned = (pBaseNode._StateNodeRef._CurrentState != null);

            if (pBaseNode._StateNodeRef._PreviousState != pBaseNode._StateNodeRef._CurrentState)
            {
                pBaseNode._StateNodeRef._PreviousState = pBaseNode._StateNodeRef._CurrentState;
                BehaviorEditor._ForceSetDirty = true;
            }
        }
    }
}