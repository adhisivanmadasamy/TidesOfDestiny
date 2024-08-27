using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    public State<T> CurrentState {  get; private set; }

    T Owner;
    public StateMachine(T owner)
    {
        Owner = owner;
    }

    public void ChangeState(State<T> newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter(Owner);
    }

    public void Execute()
    {
        CurrentState?.Execute();
    }

}


