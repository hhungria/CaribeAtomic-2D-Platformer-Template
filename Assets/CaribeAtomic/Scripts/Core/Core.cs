using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaribeAtomic
{
    public class Core : MonoBehaviour
    {
        private CharacterMotor2D _motor;
        private Platformer2DUserControl _2dController;
        private Animator _animator;

        private CharacterStateMachine StateMachine { get; set; }
        private CharacterState CharacterCurrentState;

        public CharacterMotor2D Motor
        {
            get => _motor;
            private set => _motor = value;
        }
    
        public Platformer2DUserControl Controller2D
        {
            get => _2dController;
            private set => _2dController = value;
        }

        public Animator CharacterAnimator
        {
            get => _animator;
            private set => _animator = value;
        }

        public  CharacterState GetCurrentState
        {
            get => CharacterCurrentState;
            set => CharacterCurrentState = value;
        }
      

        private void Awake()
        {
            StateMachine = new CharacterStateMachine();
            CharacterCurrentState = new CharacterState(this, "Idle", true);

            Motor = GetComponent<CharacterMotor2D>();
            //GroundDetector = GetComponent<GroundDetection>();
            Controller2D = GetComponent<Platformer2DUserControl>();
            CharacterAnimator = GetComponentInChildren<Animator>();

        }



        private void Start()
        {
            StateMachine.Initialize(CharacterCurrentState);
        }

        private void Update()
        {
            
            CharacterCurrentState = GetCurrentState;
            //Controller2D.LogicUpdate();
            StateMachine.ChangeState(CharacterCurrentState);

            // StateMachine.ChangeState();
        }

        public void FixedUpdate()
        {
            //Controller2D.LogicFixedUpdate();
        }




    }
}