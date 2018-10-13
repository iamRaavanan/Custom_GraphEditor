using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Raavanan.GraphEditor
{
    [System.Serializable]
    public class BaseNode
    {
        public int                      _Id;
        public DrawNode                 _DrawNode;
        //[HideInInspector]
        public Rect                     _WindowRect;
        //[HideInInspector]
        public string                   _WindowTitle;
        public int                      _EnterNode;
        public int                      _TargetNode;
        public bool                     _IsDuplicate;
        public string                   _Comment;
        public bool                     _IsAssigned;
        public bool                     _Collapse;
        public bool                     _ShowActions = true;
        public bool                     _ShowEnterExit = false;
        [HideInInspector]
        public bool                     _PreviousCollapse;
        [HideInInspector]
        public StateNodeReferences      _StateNodeRef;
        [HideInInspector]
        public TransitionNodeReference _TransitionNodeRef;

        public void DrawWindow()
        {
            if (_DrawNode != null)
            {
                _DrawNode.DrawWindow(this);
            }
        }

        public void DrawCurve()
        {
            if (_DrawNode != null)
            {
                _DrawNode.DrawCurve(this);
            }
        }
    }

    [System.Serializable]
    public class StateNodeReferences
    {
        [HideInInspector]
        public State _CurrentState;
        [HideInInspector]
        public State _PreviousState;
        public SerializedObject _SerializedState;
        public ReorderableList _OnFixedList;
        public ReorderableList _OnUpdateList;
        public ReorderableList _OnEnterList;
        public ReorderableList _OnExitList;
    }

    [System.Serializable]
    public class TransitionNodeReference
    {
        [HideInInspector]
        public Condition        _PreviousCondition;
        public int              _TrasitionId;
    }
}

