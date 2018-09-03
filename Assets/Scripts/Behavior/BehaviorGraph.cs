using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior.GraphEditor;

namespace Behavior
{
    [CreateAssetMenu]
    public class BehaviorGraph : ScriptableObject
    {
        public List<BaseNode>               _Windows = new List<BaseNode>();
        public int                          _IdCount;
        private List<int>                   mIndexToDelete = new List<int>();

        #region Checkers
        public BaseNode GetNodeWithIndex (int pIndex)
        {
            int count = _Windows.Count;
            for (int i = 0; i < count; i++)
            {
                if (_Windows[i]._Id == pIndex)
                {
                    return _Windows[i];
                }
            }
            return null;
        }

        public void DeleteWindowsThatNeedTo ()
        {
            for (int i = 0; i < mIndexToDelete.Count; i++)
            {
                BaseNode InBaseNode = GetNodeWithIndex(mIndexToDelete[i]);
                if (InBaseNode != null)
                {
                    _Windows.Remove(InBaseNode);
                }
            }
            mIndexToDelete.Clear();
        }

        public void DeleteNode (int pIndex)
        {
            if (!mIndexToDelete.Contains (pIndex))
                mIndexToDelete.Add(pIndex);
        }

        public bool IsStateNodeDuplicate (BaseNode pBaseNode)
        {
            for (int i = 0; i < _Windows.Count; i++)
            {
                if (_Windows[i]._Id == pBaseNode._Id)
                {
                    continue;
                }
                if (_Windows[i]._StateNodeRef._CurrentState == pBaseNode._StateNodeRef._CurrentState && !_Windows[i]._IsDuplicate)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsTransitionDuplicate (BaseNode pBaseNode)
        {
            BaseNode InBaseNode = GetNodeWithIndex(pBaseNode._EnterNode);
            if (InBaseNode == null)
            {
                return false;
            }

            for (int i = 0; i < InBaseNode._StateNodeRef._CurrentState._Transition.Count; i++)
            {
                Transition InTransition = InBaseNode._StateNodeRef._CurrentState._Transition[i];
                if (InTransition._Condition == pBaseNode._TransitionNodeRef._PreviousCondition && pBaseNode._TransitionNodeRef._TrasitionId != InTransition._Id)
                {
                    return true;
                }
            }
            return false; 
        }
        #endregion
    }
}
