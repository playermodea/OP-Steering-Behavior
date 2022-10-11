using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private GameObject m_pOwner;
    private State m_pCurrentState;

    public void CustomUpdate()
    {
        m_pCurrentState.Execute(m_pOwner);
    }

    public void ChangeState(State newState)
    {
        if (newState == null)
        {
            return;
        }

        m_pCurrentState.Exit(m_pOwner);
        m_pCurrentState = newState;
        m_pCurrentState.Enter(m_pOwner);
    }

    public void SetCurrentState(State state)
    {
        m_pCurrentState = state;
    }

    public void SetOwner(GameObject owner)
    {
        m_pOwner = owner;
    }

    public State GetCurrentState()
    {
        return m_pCurrentState;
    }
}
