using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaribeAtomic
{
    public class CharacterStateMachine 
    {
        public CharacterState CurrentState { get; private set; }


        public void Initialize(CharacterState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(CharacterState newState)
        {

            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}