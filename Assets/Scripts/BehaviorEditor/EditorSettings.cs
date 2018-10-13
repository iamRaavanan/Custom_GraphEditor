using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan.GraphEditor
{
    [CreateAssetMenu(menuName ="Editor/Settings")]
    public class EditorSettings : ScriptableObject
    {
        public BehaviorGraph                _CurrentGraph;
        public StateNode                    _StateNode;
        public PortalNode                   _PortalNode;
        public TransitionNode               _TransitionNode;
        public CommentNode                  _CommentNode;
        public bool                         _MakeTransition;
        public GUISkin                      _Skin;
        public GUISkin                      _ActiveSkin;

        public BaseNode AddNodeOnGraph(DrawNode pType, float pWidth, float pHeight, string pTitle, Vector3 pPosition)
        {
            BaseNode InBaseNode = new BaseNode();
            InBaseNode._DrawNode = pType;
            InBaseNode._WindowRect.width = pWidth;
            InBaseNode._WindowRect.height = pHeight;
            InBaseNode._WindowTitle = pTitle;
            InBaseNode._WindowRect.x = pPosition.x;
            InBaseNode._WindowRect.y = pPosition.y;
            _CurrentGraph._Windows.Add(InBaseNode);
            InBaseNode._TransitionNodeRef = new TransitionNodeReference();
            InBaseNode._StateNodeRef = new StateNodeReferences();
            InBaseNode._Id = _CurrentGraph._IdCount;
            _CurrentGraph._IdCount++;            
            return InBaseNode;
        }
    }
}
