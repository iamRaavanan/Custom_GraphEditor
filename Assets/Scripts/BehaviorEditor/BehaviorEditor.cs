using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Raavanan.GraphEditor
{
    public class BehaviorEditor : EditorWindow
    {
        #region Variables
        private Vector3                         mMousePosition;
        private bool                            mClickedOnWindow;
        private int                             mSelectedIndex;
        private BaseNode                        mSelectedNode;
        private int                             mTrasitFromId;
        private Rect                            mMouseRect = new Rect(0, 0, 1, 1);
        private GUIStyle                        mStyle;
        private GUIStyle                        mActiveStyle;
        private Vector2                         mScrollPos;
        private Vector2                         mScrollStartPos;
        private Rect                            mAllRect = new Rect(-5, -5, 10000, 10000);

        public static EditorSettings            _EditorSettings;
        public static StateManager              _CurrentStateManager;
        private static StateManager             mPreviousStateManager;
        public static bool                      _ForceSetDirty;

        public enum _UserActions
        {
            E_AddState,
            E_AddTransitionNode,
            E_DeleteNode,
            E_CommentNode,
            E_MakeTransitionNode,
            E_MakePortalNode,
            E_ResetPan
        }
        #endregion

        #region Initialize
        [MenuItem("Behaviour Editor/Editor")]
        private static void ShowEditor()
        {
            BehaviorEditor InEditor = EditorWindow.GetWindow<BehaviorEditor>();
            InEditor.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            _EditorSettings = Resources.Load("EditorSettings") as EditorSettings;
            mStyle = _EditorSettings._Skin.GetStyle("window");
            mActiveStyle = _EditorSettings._ActiveSkin.GetStyle("window");
        }
        #endregion

        private void Update()
        {
            if (Selection.activeTransform != null)
            {
                if (mPreviousStateManager != _CurrentStateManager)
                {
                    mPreviousStateManager = _CurrentStateManager;
                    Repaint();
                }
            }
        }

        #region GUI Methods
        private void OnGUI()
        {
            if (Selection.activeTransform != null)
            {
                _CurrentStateManager = Selection.activeTransform.GetComponentInChildren<StateManager>();
                if (mPreviousStateManager != _CurrentStateManager)
                {
                    mPreviousStateManager = _CurrentStateManager;
                    Repaint();
                }
            }
            Event e = Event.current;
            mMousePosition = e.mousePosition;
            UserInput(e);
            DrawWindows();

            if (e.type == EventType.MouseDrag)
            {
                if (_EditorSettings._CurrentGraph != null)
                {
                    _EditorSettings._CurrentGraph.DeleteWindowsThatNeedTo();
                    Repaint();
                }
            }
            if(GUI.changed)
            {
                _EditorSettings._CurrentGraph.DeleteWindowsThatNeedTo();
                Repaint();
            }
            if (_EditorSettings._MakeTransition)
            {
                mMouseRect.x = mMousePosition.x;
                mMouseRect.y = mMousePosition.y;

                Rect InFromNode = _EditorSettings._CurrentGraph.GetNodeWithIndex(mTrasitFromId)._WindowRect;
                DrawNodeCurve(InFromNode, mMouseRect, true, Color.blue);
                Repaint();
            }
            if (_ForceSetDirty)
            {
                _ForceSetDirty = false;
                EditorUtility.SetDirty(_EditorSettings);
                EditorUtility.SetDirty(_EditorSettings._CurrentGraph);
                for (int i = 0; i < _EditorSettings._CurrentGraph._Windows.Count; i++)
                {
                    BaseNode InBaseNode = _EditorSettings._CurrentGraph._Windows[i];
                    if (InBaseNode._StateNodeRef._CurrentState != null)
                    {
                        EditorUtility.SetDirty(InBaseNode._StateNodeRef._CurrentState);
                    }
                }
            }
        }
        
        private void DrawWindows()
        {
            GUILayout.BeginArea(mAllRect, mStyle);
            BeginWindows();
            EditorGUILayout.LabelField(" ", GUILayout.Width(100));
            EditorGUILayout.LabelField("Assign Graph: ", GUILayout.Width(100));

            _EditorSettings._CurrentGraph = (BehaviorGraph)EditorGUILayout.ObjectField(_EditorSettings._CurrentGraph, typeof(BehaviorGraph), false, GUILayout.Width(200));
            if (_EditorSettings._CurrentGraph != null)
            {
                foreach (BaseNode node in _EditorSettings._CurrentGraph._Windows)
                {
                    node.DrawCurve();
                }
                int InLength = _EditorSettings._CurrentGraph._Windows.Count;
                for (int i = 0; i < InLength; i++)
                {
                    //_EditorSettings._CurrentGraph._Windows[i]._WindowRect = GUI.Window(i, _EditorSettings._CurrentGraph._Windows[i]._WindowRect, DrawNodeWindow, _EditorSettings._CurrentGraph._Windows[i]._WindowTitle);
                    BaseNode InBaseNode = _EditorSettings._CurrentGraph._Windows[i];

                    if (InBaseNode._DrawNode is StateNode)
                    {
                        if (_CurrentStateManager != null && InBaseNode._StateNodeRef._CurrentState == _CurrentStateManager._CurrentState)
                        {
                            InBaseNode._WindowRect = GUI.Window(i, InBaseNode._WindowRect, DrawNodeWindow, InBaseNode._WindowTitle, mActiveStyle);
                        }
                        else
                        {
                            InBaseNode._WindowRect = GUI.Window(i, InBaseNode._WindowRect, DrawNodeWindow, InBaseNode._WindowTitle);
                        }
                    }
                    else
                    {
                        InBaseNode._WindowRect = GUI.Window(i, InBaseNode._WindowRect, DrawNodeWindow, InBaseNode._WindowTitle);
                    }
                }
            }
            EndWindows();
            GUILayout.EndArea();            
        }

        private void DrawNodeWindow(int id)
        {
            _EditorSettings._CurrentGraph._Windows[id].DrawWindow();
            GUI.DragWindow();
        }

        private void UserInput(Event e)
        {
            if (e.button == 1 && !_EditorSettings._MakeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    RighClicked(e);
                }                
            }
            if (e.button == 0 && !_EditorSettings._MakeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    LeftClicked(e);
                }
            }
            if (e.button == 0 && _EditorSettings._MakeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    MakeTransition();
                }
            }
            if (e.button == 2)
            {
                if (e.type == EventType.MouseDown)
                {
                    mScrollStartPos = e.mousePosition;
                }
                else if (e.type == EventType.MouseDrag)
                {
                    HandlePanning(e);
                }
                else if (e.type == EventType.MouseUp)
                {

                }
            }
        }

        private void HandlePanning(Event e)
        {
            Vector2 InDifference = e.mousePosition - mScrollStartPos;
            InDifference *= 0.6f;
            mScrollStartPos = e.mousePosition;
            mScrollPos += InDifference;

            int InCount = _EditorSettings._CurrentGraph._Windows.Count;
            for (int i = 0; i < InCount; i++)
            {
                BaseNode InBaseNode = _EditorSettings._CurrentGraph._Windows[i];
                InBaseNode._WindowRect.x += InDifference.x;
                InBaseNode._WindowRect.y += InDifference.y;
            }
        }

        private void ResetScroll ()
        {
            int InCount = _EditorSettings._CurrentGraph._Windows.Count;
            for (int i = 0; i < InCount; i++)
            {
                BaseNode InBaseNode = _EditorSettings._CurrentGraph._Windows[i];
                InBaseNode._WindowRect.x -= mScrollPos.x;
                InBaseNode._WindowRect.y -= mScrollPos.y;
            }
            mScrollPos = Vector2.zero;
        }

        private void RighClicked(Event e)
        {
            if (_EditorSettings._CurrentGraph == null)
                return;
            mSelectedIndex = -1;
            mClickedOnWindow = false;
            int Length = _EditorSettings._CurrentGraph._Windows.Count;
            for (int i = 0; i < Length; i++)
            {
                if (_EditorSettings._CurrentGraph._Windows[i]._WindowRect.Contains(e.mousePosition))
                {
                    mClickedOnWindow = true;
                    mSelectedNode = _EditorSettings._CurrentGraph._Windows[i];
                    mSelectedIndex = i;
                    break;
                }
            }

            if (!mClickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyExistingNode(e);
            }
        }

        private void MakeTransition ()
        {
            _EditorSettings._MakeTransition = mClickedOnWindow = false;
            int Length = _EditorSettings._CurrentGraph._Windows.Count;
            for (int i = 0; i < Length; i++)
            {
                if (_EditorSettings._CurrentGraph._Windows[i]._WindowRect.Contains(mMousePosition))
                {
                    mClickedOnWindow = true;
                    mSelectedNode = _EditorSettings._CurrentGraph._Windows[i];
                    mSelectedIndex = i;
                    break;
                }
            }

            if (mClickedOnWindow)
            {
                if (mSelectedNode._DrawNode is StateNode || mSelectedNode._DrawNode is PortalNode)
                {
                    if (mSelectedNode._Id != mTrasitFromId)
                    {
                        BaseNode InTransitionNode = _EditorSettings._CurrentGraph.GetNodeWithIndex(mTrasitFromId);
                        InTransitionNode._TargetNode = mSelectedNode._Id;

                        BaseNode InEnterNode = BehaviorEditor._EditorSettings._CurrentGraph.GetNodeWithIndex(InTransitionNode._EnterNode);
                        Transition InTransition = InEnterNode._StateNodeRef._CurrentState.GetTransition(InEnterNode._TransitionNodeRef._TrasitionId);
                        InTransition._TargetState = mSelectedNode._StateNodeRef._CurrentState;
                    }
                }
            }
        }
        #endregion

        #region Context Menus

        private void AddNewNode(Event e)
        {
            GenericMenu InMenu = new GenericMenu();
            InMenu.AddSeparator("");
            if (_EditorSettings._CurrentGraph != null)
            {
                InMenu.AddItem(new GUIContent("Add State"), false, MenuContextCallback, _UserActions.E_AddState);
                InMenu.AddItem(new GUIContent("Add Portal"), false, MenuContextCallback, _UserActions.E_MakePortalNode);
                InMenu.AddItem(new GUIContent("Add Comment"), false, MenuContextCallback, _UserActions.E_CommentNode);
                InMenu.AddSeparator("");
                InMenu.AddItem(new GUIContent("Reset Panning"), false, MenuContextCallback, _UserActions.E_ResetPan);
            }
            else
            {
                InMenu.AddDisabledItem(new GUIContent("Add State"));
                InMenu.AddDisabledItem(new GUIContent("Add Comment"));
            }
            InMenu.ShowAsContext();
            e.Use();
        }

        private void ModifyExistingNode(Event e)
        {
            GenericMenu InMenu = new GenericMenu();
            if (mSelectedNode._DrawNode is StateNode)
            {
                if (mSelectedNode._StateNodeRef._CurrentState != null)
                {
                    InMenu.AddSeparator("");
                    InMenu.AddItem(new GUIContent("Add Condition"), false, MenuContextCallback, _UserActions.E_AddTransitionNode);
                }
                else
                {
                    InMenu.AddSeparator("");
                    InMenu.AddDisabledItem(new GUIContent("Add Condition"));
                }
                InMenu.AddSeparator("");
                InMenu.AddItem(new GUIContent("Delete"), false, MenuContextCallback, _UserActions.E_DeleteNode);

            }

            if (mSelectedNode._DrawNode is PortalNode)
            {
                InMenu.AddSeparator("");
                InMenu.AddItem(new GUIContent("Delete"), false, MenuContextCallback, _UserActions.E_DeleteNode);

            }

            if (mSelectedNode._DrawNode is TransitionNode)
            {
                if (mSelectedNode._IsDuplicate || !mSelectedNode._IsAssigned)
                {
                    InMenu.AddSeparator("");
                    InMenu.AddDisabledItem(new GUIContent("Make Transition"));                    
                }
                else
                {
                    InMenu.AddSeparator("");
                    InMenu.AddItem(new GUIContent("Make Transition"), false, MenuContextCallback, _UserActions.E_MakeTransitionNode);
                }
                InMenu.AddSeparator("");
                InMenu.AddItem(new GUIContent("Delete"), false, MenuContextCallback, _UserActions.E_DeleteNode);
            }

            if (mSelectedNode._DrawNode is CommentNode)
            {
                InMenu.AddSeparator("");
                InMenu.AddItem(new GUIContent("Delete"), false, MenuContextCallback, _UserActions.E_DeleteNode);
            }
            InMenu.ShowAsContext();
            e.Use();
        }

        private void MenuContextCallback(object obj)
        {
            _UserActions InAction = (_UserActions)obj;
            switch (InAction)
            {
                case _UserActions.E_AddState:
                    _EditorSettings.AddNodeOnGraph(_EditorSettings._StateNode, 200, 100, "State", mMousePosition);
                    break;
                case _UserActions.E_MakePortalNode:
                    _EditorSettings.AddNodeOnGraph(_EditorSettings._PortalNode, 100, 80, "State", mMousePosition);
                    break;
                case _UserActions.E_AddTransitionNode:
                    AddTransitionNode(mSelectedNode, mMousePosition);
                    break;
                case _UserActions.E_CommentNode:
                    BaseNode InCommentNode = _EditorSettings.AddNodeOnGraph(_EditorSettings._CommentNode, 200, 100, "Comment", mMousePosition);
                    InCommentNode._Comment = "This is comment.";
                    break;
                case _UserActions.E_MakeTransitionNode:
                    mTrasitFromId = mSelectedNode._Id;
                    _EditorSettings._MakeTransition = true;
                    break;
                case _UserActions.E_ResetPan:
                    ResetScroll();
                    break;
                case _UserActions.E_DeleteNode:
                    if (mSelectedNode._DrawNode is TransitionNode)
                    {
                        BaseNode InEnterNode = _EditorSettings._CurrentGraph.GetNodeWithIndex(mSelectedNode._EnterNode);
                        //InEnterNode._StateNodeRef._CurrentState.RemoveTransition(mSelectedNode._TransitionNodeRef._TrasitionId);
                    }
                    _EditorSettings._CurrentGraph.DeleteNode(mSelectedNode._Id);
                    break;
                default:
                    break;
            }
            _ForceSetDirty = true;
        }

        private void LeftClicked(Event e)
        {
            mSelectedNode = null;
        }

        public static BaseNode AddTransitionNode (BaseNode pEnterNode, Vector3 pPosition)
        {
            BaseNode InTransitionNode = _EditorSettings.AddNodeOnGraph(_EditorSettings._TransitionNode, 200, 100, "Condition", pPosition);
            InTransitionNode._EnterNode = pEnterNode._Id;
            Transition InTransition = _EditorSettings._StateNode.AddTransition(pEnterNode);
            InTransitionNode._TransitionNodeRef._TrasitionId = InTransition._Id;
            return InTransitionNode;
        }

        public static BaseNode AddTransitionNodeFromTransition(Transition pTransition, BaseNode pEnterNode, Vector3 pPosition)
        {
            BaseNode InTransitionNode = _EditorSettings.AddNodeOnGraph(_EditorSettings._TransitionNode, 200, 100, "Condition", pPosition);
            InTransitionNode._EnterNode = pEnterNode._Id;
            InTransitionNode._TransitionNodeRef._TrasitionId = pTransition._Id;
            return InTransitionNode;
        }
        #endregion

        #region Helper Methods
        public static void DrawNodeCurve(Rect pStart, Rect pEnd, bool pLeft, Color pCurveColor)
        {
            Vector3 InStartPos = new Vector3(
                (pLeft) ? pStart.x + pStart.width : pStart.x,
                pStart.y + (pStart.height * 0.5f),
                0);
            Vector3 InEndPos = new Vector3(
                pEnd.x + (pEnd.width * 0.5f), 
                pEnd.y + (pEnd.height * 0.5f), 
                0);
            Vector3 InStartTan = InStartPos + Vector3.right * 50;
            Vector3 InEndTan = InEndPos + Vector3.left * 50;
            Color InShadow = new Color(0, 0, 0, 0.06f);
            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(InStartPos, InEndPos, InStartTan, InEndTan, new Color(0, 0, 0, 0.15f), null, (i + 1) * 1);
            }
            Handles.DrawBezier(InStartPos, InEndPos, InStartTan, InEndTan, pCurveColor, null, 3);
        }

        public static void ClearWindowFromList(List<BaseNode> pBaseNode)
        {
            int count = pBaseNode.Count;
            for (int i = 0; i < count; i++)
            {
                if (_EditorSettings._CurrentGraph._Windows.Contains(pBaseNode[i]))
                {
                    _EditorSettings._CurrentGraph._Windows.Remove(pBaseNode[i]);
                }
            }
        }

        #endregion
    }
}
