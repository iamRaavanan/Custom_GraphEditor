using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raavanan;
using UnityEditor;
using UnityEditorInternal;

namespace Raavanan.CustomUI
{
    [CustomEditor(typeof(State))]
    public class StateGUI : Editor
    {
        private SerializedObject mSerializedState;
        private ReorderableList mOnFixedList;
        private ReorderableList mOnUpdateList;
        private ReorderableList mOnEnterList;
        private ReorderableList mOnExitList;
        private ReorderableList mTransitions;

        private bool mShowDefaultGUI = false;
        private bool mShowActions = true;
        private bool mShowTransitions = true;

        private void OnEnable()
        {
            mSerializedState = null;
        }

        public override void OnInspectorGUI()
        {
            mShowDefaultGUI = EditorGUILayout.Toggle("DefaultGUI", mShowDefaultGUI);
            if (mShowDefaultGUI)
            {
                base.OnInspectorGUI();
                return;
            }

            mShowActions = EditorGUILayout.Toggle("Show Actions", mShowActions);

            if (mSerializedState == null)
                SetupReordableLists();

            mSerializedState.Update();

            if (mShowActions)
            {
                EditorGUILayout.LabelField("Actions that execute on FixedUpdate()");
                mOnFixedList.DoLayoutList();
                EditorGUILayout.LabelField("Actions that execute on Update()");
                mOnUpdateList.DoLayoutList();
                EditorGUILayout.LabelField("Actions that execute when entering this State");
                mOnEnterList.DoLayoutList();
                EditorGUILayout.LabelField("Actions that execute when exiting this State");
                mOnExitList.DoLayoutList();
            }

            mShowTransitions = EditorGUILayout.Toggle("Show Transitions", mShowTransitions);

            if (mShowTransitions)
            {
                EditorGUILayout.LabelField("Conditions to exit this State");
                mTransitions.DoLayoutList();
            }

            mSerializedState.ApplyModifiedProperties();
        }

        void SetupReordableLists()
        {
            State curState = (State)target;
            mSerializedState = new SerializedObject(curState);
            mOnFixedList = new ReorderableList(mSerializedState, mSerializedState.FindProperty("_OnFixed"), true, true, true, true);
            mOnUpdateList = new ReorderableList(mSerializedState, mSerializedState.FindProperty("_OnUpdate"), true, true, true, true);
            mOnEnterList = new ReorderableList(mSerializedState, mSerializedState.FindProperty("_OnEnter"), true, true, true, true);
            mOnExitList = new ReorderableList(mSerializedState, mSerializedState.FindProperty("_OnExit"), true, true, true, true);
            mTransitions = new ReorderableList(mSerializedState, mSerializedState.FindProperty("_Transition"), true, true, true, true);

            HandleReordableList(mOnFixedList, "On Fixed");
            HandleReordableList(mOnUpdateList, "On Update");
            HandleReordableList(mOnEnterList, "On Enter");
            HandleReordableList(mOnExitList, "On Exit");
            HandleTransitionReordable(mTransitions, "Condition --> New State");
        }

        void HandleReordableList(ReorderableList pList, string pTargetName)
        {
            pList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, pTargetName);
            };

            pList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = pList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        }

        void HandleTransitionReordable(ReorderableList pList, string pTargetName)
        {
            pList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, pTargetName);
            };

            pList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = pList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width * .3f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("_Condition"), GUIContent.none);
                EditorGUI.ObjectField(new Rect(rect.x + +(rect.width * .35f), rect.y, rect.width * .3f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("_TargetState"), GUIContent.none);
                EditorGUI.LabelField(new Rect(rect.x + +(rect.width * .75f), rect.y, rect.width * .2f, EditorGUIUtility.singleLineHeight), "Disable");
                EditorGUI.PropertyField(new Rect(rect.x + +(rect.width * .90f), rect.y, rect.width * .2f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("_Disable"), GUIContent.none);

            };
        }

    }
}