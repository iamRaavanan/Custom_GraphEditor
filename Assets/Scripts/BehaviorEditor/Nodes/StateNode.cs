using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Raavanan;

namespace Raavanan.GraphEditor
{
    [CreateAssetMenu (menuName = "Editor/Nodes/State Node")]
    public class StateNode : DrawNode
    {
        public override void DrawWindow(BaseNode pBaseNode)
        {
            if (pBaseNode._StateNodeRef._CurrentState == null)
            {
                EditorGUILayout.LabelField("Add state.");
            }
            else
            {
                if (!pBaseNode._Collapse)
                {

                }
                else
                {
                    pBaseNode._WindowRect.height = 100;
                }
                pBaseNode._Collapse = EditorGUILayout.Toggle(" ", pBaseNode._Collapse);
            }
            pBaseNode._StateNodeRef._CurrentState = (State)EditorGUILayout.ObjectField(pBaseNode._StateNodeRef._CurrentState, typeof(State), false);

            if (pBaseNode._PreviousCollapse != pBaseNode._Collapse)
            {
                pBaseNode._PreviousCollapse = pBaseNode._Collapse;
            }
            if (pBaseNode._StateNodeRef._PreviousState != pBaseNode._StateNodeRef._CurrentState)
            {
                pBaseNode._IsDuplicate = BehaviorEditor._EditorSettings._CurrentGraph.IsStateNodeDuplicate(pBaseNode);
                pBaseNode._StateNodeRef._PreviousState = pBaseNode._StateNodeRef._CurrentState;
                if (!pBaseNode._IsDuplicate)
                {
                    Vector3 InPosition = new Vector3(pBaseNode._WindowRect.x, pBaseNode._WindowRect.y, 0);
                    InPosition.x += pBaseNode._WindowRect.width * 2;

                    SetupReorderableList(pBaseNode);

                    for (int i = 0; i < pBaseNode._StateNodeRef._CurrentState._Transition.Count; i++)
                    {
                        InPosition.y += i * 100;
                        BehaviorEditor.AddTransitionNodeFromTransition(pBaseNode._StateNodeRef._CurrentState._Transition[i], pBaseNode, InPosition);
                    }
                    BehaviorEditor._ForceSetDirty = true;
                }
            }

            if (pBaseNode._IsDuplicate)
            {
                EditorGUILayout.LabelField("State is duplicate.");
                pBaseNode._WindowRect.height = 100;
                return;
            }

            if (pBaseNode._StateNodeRef._CurrentState != null)
            {
                pBaseNode._IsAssigned = true;

                if (!pBaseNode._Collapse)
                {
                    if (pBaseNode._StateNodeRef._SerializedState == null)
                    {
                        SetupReorderableList(pBaseNode);
                    }
                    float standard = 150;
                    pBaseNode._StateNodeRef._SerializedState.Update();
                    pBaseNode._ShowActions = EditorGUILayout.Toggle("Show Actions ", pBaseNode._ShowActions);
                    if (pBaseNode._ShowActions)
                    {
                        EditorGUILayout.LabelField("");
                        pBaseNode._StateNodeRef._OnFixedList.DoLayoutList();
                        EditorGUILayout.LabelField("");
                        pBaseNode._StateNodeRef._OnUpdateList.DoLayoutList();
                        standard += 100 + 40 + (pBaseNode._StateNodeRef._OnUpdateList.count + pBaseNode._StateNodeRef._OnFixedList.count) * 20;
                    }
                    pBaseNode._ShowEnterExit = EditorGUILayout.Toggle("Show Enter/Exit ", pBaseNode._ShowEnterExit);
                    if (pBaseNode._ShowEnterExit)
                    {
                        EditorGUILayout.LabelField("");
                        pBaseNode._StateNodeRef._OnEnterList.DoLayoutList();
                        EditorGUILayout.LabelField("");
                        pBaseNode._StateNodeRef._OnExitList.DoLayoutList();
                        standard += 100 + 40 + (pBaseNode._StateNodeRef._OnEnterList.count + pBaseNode._StateNodeRef._OnExitList.count) * 20;
                    }

                    pBaseNode._StateNodeRef._SerializedState.ApplyModifiedProperties();
                    pBaseNode._WindowRect.height = standard;
                }
            }
            else
            {
                pBaseNode._IsAssigned = false;
            }
        }

        private void SetupReorderableList(BaseNode pBaseNode)
        {
            pBaseNode._StateNodeRef._SerializedState = new SerializedObject(pBaseNode._StateNodeRef._CurrentState);

            pBaseNode._StateNodeRef._OnFixedList = new ReorderableList(pBaseNode._StateNodeRef._SerializedState, pBaseNode._StateNodeRef._SerializedState.FindProperty("_OnFixed"), true, true, true, true);
            pBaseNode._StateNodeRef._OnUpdateList = new ReorderableList(pBaseNode._StateNodeRef._SerializedState, pBaseNode._StateNodeRef._SerializedState.FindProperty("_OnUpdate"), true, true, true, true);
            pBaseNode._StateNodeRef._OnEnterList = new ReorderableList(pBaseNode._StateNodeRef._SerializedState, pBaseNode._StateNodeRef._SerializedState.FindProperty("_OnEnter"), true, true, true, true);
            pBaseNode._StateNodeRef._OnExitList = new ReorderableList(pBaseNode._StateNodeRef._SerializedState, pBaseNode._StateNodeRef._SerializedState.FindProperty("_OnExit"), true, true, true, true);

            HandleReorderableList(pBaseNode._StateNodeRef._OnFixedList, "On Fixed");
            HandleReorderableList(pBaseNode._StateNodeRef._OnUpdateList, "On State");
            HandleReorderableList(pBaseNode._StateNodeRef._OnEnterList, "On Enter");
            HandleReorderableList(pBaseNode._StateNodeRef._OnExitList, "On Exit");
        }

        private void HandleReorderableList(ReorderableList pList, string pTargetName)
        {
            pList.drawHeaderCallback = (Rect pRect) => 
            {
                EditorGUI.LabelField(pRect, pTargetName);
            };
            pList.drawElementCallback = (Rect pRect, int pIndex, bool pIsActive, bool pIsFocused) =>
            {
                var InElement = pList.serializedProperty.GetArrayElementAtIndex(pIndex);
                EditorGUI.ObjectField(new Rect(pRect.x, pRect.y, pRect.width, EditorGUIUtility.singleLineHeight), InElement, GUIContent.none);
            };
        }

        public override void DrawCurve(BaseNode pBaseNode)
        {
            
        }

        public Transition AddTransition(BaseNode pBaseNode)
        {
            return pBaseNode._StateNodeRef._CurrentState.AddTransition();
        }

        public void ClearReference()
        {
            //BehaviorEditor.ClearWindowFromList(mDependecyNodes);
            //mDependecyNodes.Clear();
        }
    }
}

