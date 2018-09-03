using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Behavior.GraphEditor
{
    [CreateAssetMenu(menuName = "Editor/Nodes/Transition Node")]
    public class TransitionNode : DrawNode
    {
        public void Init (StateNode pEnterState, Transition pTransition)
        {
            //_EnterState = pEnterState;
        }

        public override void DrawWindow(BaseNode pBaseNode)
        {
            EditorGUILayout.LabelField("");
            BaseNode InEnterBaseNode = BehaviorEditor._EditorSettings._CurrentGraph.GetNodeWithIndex(pBaseNode._EnterNode);
            if (InEnterBaseNode == null)
            {
                return;
            }
            if (InEnterBaseNode._StateNodeRef._CurrentState == null)
            {
                BehaviorEditor._EditorSettings._CurrentGraph.DeleteNode(pBaseNode._Id);
                return;
            }

            Transition InTransition = InEnterBaseNode._StateNodeRef._CurrentState.GetTransition(pBaseNode._TransitionNodeRef._TrasitionId);
            if (InTransition == null)
            {
                return;
            }
            InTransition._Condition = (Condition)EditorGUILayout.ObjectField(InEnterBaseNode._StateNodeRef._CurrentState.GetTransition(pBaseNode._TransitionNodeRef._TrasitionId)._Condition,
                typeof(Condition), false);

            if (InTransition._Condition == null)
            {
                EditorGUILayout.LabelField("No Condition.");
                pBaseNode._IsAssigned = false;
            }
            else
            {
                pBaseNode._IsAssigned = true;
                if (pBaseNode._IsDuplicate)
                {
                    EditorGUILayout.LabelField("Duplicate Condition.");
                }
                else
                {
                    GUILayout.Label(InTransition._Condition._Description);
                    BaseNode InTargetNode = BehaviorEditor._EditorSettings._CurrentGraph.GetNodeWithIndex(pBaseNode._TargetNode);
                    if (InTargetNode != null)
                    {
                        if (!InTargetNode._IsDuplicate)
                        {
                            InTransition._TargetState = InTargetNode._StateNodeRef._CurrentState;
                        }
                        else
                        {
                            InTransition._TargetState = null;
                        }
                    }
                    else
                    {
                        InTransition._TargetState = null;
                    }
                }                
            }
            if (pBaseNode._TransitionNodeRef._PreviousCondition != InTransition._Condition)
            {
                pBaseNode._TransitionNodeRef._PreviousCondition = InTransition._Condition;
                pBaseNode._IsDuplicate = BehaviorEditor._EditorSettings._CurrentGraph.IsTransitionDuplicate(pBaseNode);
                if (!pBaseNode._IsDuplicate)
                {
                    BehaviorEditor._ForceSetDirty = true;                 
                }                
            }
        }

        public override void DrawCurve(BaseNode pBaseNode)
        {
            Rect InRect = pBaseNode._WindowRect;
            InRect.y += pBaseNode._WindowRect.height * 0.5f;
            InRect.width = 1;
            InRect.height = 1;

            BaseNode InEnterNode = BehaviorEditor._EditorSettings._CurrentGraph.GetNodeWithIndex(pBaseNode._EnterNode);
            if (InEnterNode == null)
            {
                BehaviorEditor._EditorSettings._CurrentGraph.DeleteNode(pBaseNode._Id);
            }
            else
            {
                Color InTargetColor = Color.green;
                if (!pBaseNode._IsAssigned || pBaseNode._IsDuplicate)
                {
                    InTargetColor = Color.red;
                }
                Rect InRect1 = InEnterNode._WindowRect;
                BehaviorEditor.DrawNodeCurve(InRect1, InRect, true, InTargetColor);
            }

            if (pBaseNode._IsDuplicate)
            {
                return;
            }

            if (pBaseNode._TargetNode > 0)
            {
                BaseNode InTargetNode = BehaviorEditor._EditorSettings._CurrentGraph.GetNodeWithIndex(pBaseNode._TargetNode);
                if (InTargetNode == null)
                {
                    pBaseNode._TargetNode = -1;
                }
                else
                {
                    InRect = pBaseNode._WindowRect;
                    InRect.x += InRect.width;
                    Rect InEndRect = InTargetNode._WindowRect;
                    InEndRect.x -= InEndRect.width * 0.5f;
                    Color InTargetColor = Color.green;

                    if (InTargetNode._DrawNode is StateNode)
                    {
                        if (!InTargetNode._IsAssigned || InTargetNode._IsDuplicate)
                        {
                            InTargetColor = Color.red;
                        }
                    }
                    else
                    {
                        if (!InTargetNode._IsAssigned)
                        {
                            InTargetColor = Color.red;
                        }
                        else
                        {
                            InTargetColor = Color.yellow;
                        }
                    }
                    BehaviorEditor.DrawNodeCurve(InRect, InEndRect, false, InTargetColor);
                }
            }
        }
    }

}
