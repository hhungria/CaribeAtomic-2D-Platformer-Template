using System;
using UnityEngine;


namespace CaribeAtomic
{
    [RequireComponent(typeof (CharacterMotor2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private CharacterMotor2D m_Character;
        private bool m_Jump;
        private bool m_Crouch;
        private bool m_toogleWalkAndRun = true;

        private InputHandler m_InputHandler { get; set; }

        private void Awake()
        {
            m_Character = GetComponent<CharacterMotor2D>();
            m_InputHandler = GetComponent<InputHandler>();
        }

       
        public void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = m_InputHandler.JumpInput;
            }

         
                // Read the Crouch input in Update so button presses aren't missed.
                m_Crouch = m_InputHandler.CrouchInput;
            
           

           

        }


        public void FixedUpdate()
        {
           
            // Read the inputs.
            float h = m_InputHandler.NormInputX;
            bool c = m_Crouch, v = m_Jump, w = m_toogleWalkAndRun = true;
           
            /// <summary>
            /// Call Move Method from the Character Motor. 
            /// <param name="h"> Allow movement along Horizontal Axis coordenate</param>
            /// <param name="c"> m_Crouch = true: make the character crouch, false: make the character stand</param>
            /// <param name="v"> m_Jump = true : Allow movement along Vertical Axis and disable gravity force. </param>
            /// <param name="w"> m_toogleWalkAndRun = true : Make character switch between walk / run. </param>
            /// <summary>
            m_Character.Move(h, c, v, w);
            m_Jump = false;
        }

        //public void LogicUpdate() { }  For Future Implementations


        //public void LogicFixedUpdate()
        //{

        //}
    }
}
