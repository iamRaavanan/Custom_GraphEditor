using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raavanan
{
    [CreateAssetMenu]
    public class State : ScriptableObject
    {
        public StateActions[]                   _OnFixed;
        public StateActions[]                   _OnUpdate;
        public StateActions[]                   _OnEnter;
        public StateActions[]                   _OnExit;
        public List<Transition>                 _Transition = new List<Transition>();
        public int                              _IdCount;

        public void OnEnter(StateManager pStateManager)
        {
            ExecuteActions(pStateManager, _OnEnter);
        }

        public void Tick(StateManager pStateManager)
        {
            ExecuteActions(pStateManager, _OnUpdate);
            CheckTransition(pStateManager);
        }

        public void OnExit(StateManager pStateManager)
        {
            ExecuteActions(pStateManager, _OnExit);
        }

        public void CheckTransition(StateManager pStateManager)
        {
            for (int i = 0; i < _Transition.Count; i++)
            {
                if (_Transition[i]._Disable)
                {
                    continue;
                }
                if (_Transition[i]._Condition.CheckCondition(pStateManager))
                {
                    if (_Transition[i]._TargetState != null)
                    {
                        pStateManager._CurrentState = _Transition[i]._TargetState;
                        OnExit(pStateManager);
                        pStateManager._CurrentState.OnEnter(pStateManager);
                    }
                    return;
                }
            }
        }

        public void ExecuteActions (StateManager pStateManager, StateActions[] pStateActions)
        {
            int length = pStateActions.Length;
            for (int i = 0; i < length; i++)
            {
                if (pStateActions[i] != null)
                {
                    pStateActions[i].Execute(pStateManager);
                }
            }
        }

        public Transition AddTransition()
        {
            Transition InTransition = new Transition(); 
            _Transition.Add(InTransition);
            InTransition._Id = _IdCount;
            _IdCount++;
            return InTransition;
        }

        public Transition GetTransition (int pId)
        {
            for (int i = 0; i < _Transition.Count; i++)
            {
                if (_Transition[i]._Id == pId)
                {
                    return _Transition[i];
                }
            }
            return null;
        }
    }
}