using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaribeAtomic
{
    public class CharacterState
    {
    
        protected Core m_core;
        protected CharacterStateMachine stateMachine;
        protected Platformer2DUserControl control;
        protected CharacterMotor2D motor;

        protected bool isAnimationFinished;
        protected bool isExitingState;

        protected float startTime;

        private string animBoolName;
        private bool animBoolValue;



        public CharacterState(Core core, string animBoolName, bool animBoolValue)
        {
            this.m_core = core;
            this.animBoolName = animBoolName;
            this.animBoolValue = animBoolValue;
        }

    
        public virtual void Enter()
        {
           
            DoChecksPhysics();
            m_core.CharacterAnimator.SetBool(animBoolName, animBoolValue );
            startTime = Time.time;
            isAnimationFinished = false;
            isExitingState = false;
        }

        public virtual void Exit()
        {
            //Debug.Log(animBoolValue+" "+ animBoolName);
            m_core.CharacterAnimator.SetBool(animBoolName, !animBoolValue);
            isExitingState = true;
        }

        public virtual void LogicUpdate()
        {

        }
        public virtual void PhysicsUpdate()
        {
            DoChecksPhysics();
        }

        public virtual void DoChecksPhysics() { }

        public virtual void AnimationTrigger() { }

        public virtual void AnimationFinishTrigger() => isAnimationFinished = true;


    }

    public struct CurrentState
    {
        public string m_state;
        public bool m_stateValue;

        public static CurrentState StateValue(string state, bool b)
        {
            CurrentState settings;

            settings.m_state = state;
            settings.m_stateValue = b;

            return settings;

        }


    }




}
