using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CaribeAtomic
{
    public static class Const

    {
        public const string
            k_Ground = "OnGrounded",
            k_Jump = "OnJumping",
            k_Idle = "OnIdle",
            k_Crouch = "OnCrouching",
            k_Falling = "OnFalling",
            k_Running = "OnRunning",
            k_Walking = "OnWalking",
            k_Wall = "OnWall",
            k_Landing = "OnLanding";
    }


    [Serializable]
    public struct SettingsBools
        {

            [Tooltip("Enable jump")]
            public bool enableJump;

            [SerializeField]
            [Tooltip("Enable walk")]
            public bool enableWalk;

            [SerializeField]
            [Tooltip("Enable always run")]
            public bool enableAlwaysRun;

            [SerializeField]
            [Tooltip("Enable long jump. character can jump higher by keep pressing the jump button")]
            public bool enableLongJump;

            [SerializeField]
            [Tooltip("Enable character double jump")]
            public bool enableDoubleJump;

            [SerializeField]
            [Tooltip("Enable wall surfing. character can slow down fall wall by going forward a wall")]
            public bool enableWallSurfing;

            [SerializeField]
            [Tooltip("Enable wall jump. character can jump again if against a wall")]
            public bool enableWallJump;

            [SerializeField]
            [Tooltip("Enable character crouch")]
            public bool enableCrouch;

            [SerializeField]
            [Tooltip("Enable changing direction during jump")]
            public bool enableAirControl;

            [SerializeField]
            [Tooltip("Enable to keep momentum if pushed in the air by other objects")]
            public bool enableAirPush;

            // For determining which way the character is currently facing. Use this from the animator to check for
            // the character direction
             public bool m_isFacingRight;

        public static SettingsBools Default()
            {

                SettingsBools settings;

                settings.enableJump = true;

                settings.enableWalk = true;

                settings.enableAlwaysRun = true;

                settings.enableLongJump = true;

                settings.enableDoubleJump = true;

                settings.enableWallSurfing = true;

                settings.enableWallJump = true;

                settings.enableCrouch = true;

                settings.enableAirControl = true;

                settings.enableAirPush = true;

                settings.m_isFacingRight = true;

                return settings;

            }


        }

    // Movements
    [Serializable]
    public struct SettingsMovement
    {
        [Tooltip("Run speed")]
        public float runSpeed;

        [Tooltip("Walk speed")]
        public float walkSpeed;

        [Tooltip("Force up for the initial jump")]
        public float jumpForce;

        [Tooltip("Force up for the long jump")]
        public float longJumpForce;

        [Tooltip("Time for the long jump in seconds")]
        public float longJumpTime;

        [Tooltip("Force away from wall to apply to character when wall jumping")]
        public float wallJumpPush;

        [Range(0, 1.0f)]
        [Tooltip("Max jump factor relative to jump force when wall jumping. 1 = 100%")]
        public float wallJump;

        [Range(0, 1.0f)]
        [Tooltip("The character have an extra time to jump when falling ledge or wall jumping")]
        public float justInTimeJump;

        [Tooltip("Maximum number of double jump")]
        public int doubleJumpMax;

        [Range(0, 1)]
        [Tooltip("Max speed factor applied to crouching movement relative to walking speed. 1 = 100%")]
        public float crouchWalkSpeed;

        [Range(0, 1.0f)]
        [Tooltip("Smooth of the character movement when on the ground")]
        public float groundMovementSmoothing;

        [Range(0, 1.0f)]
        [Tooltip("Smooth of the character movement when in the air")]
        public float airMovementSmoothing;

        [Range(0, 1.0f)]
        [Tooltip("Friction on the wall when wall surfing")]
        public float wallSurfingFriction;

        [Range(0, 2.0f)]
        [Tooltip("Air direction control when changing direction")]
        public float airMovement;

        [Range(0, 4.0f)]
        [Tooltip("Change falling gravity when falling. Normal gravity is 1")]
        public float jumpingGravity;

        [Range(0, 4.0f)]
        [Tooltip("Change falling gravity when falling. Normal gravity is 1")]
        public float fallingGravity;

        [Range(0, 2.0f)]
        [Tooltip("Friction factor of the character on the ground")]
        public float groundFriction;

        [Range(0, 2.0f)]
        [Tooltip("Friction factor of the character in the air")]
        public float airFriction;



        [SerializeField]
        public static SettingsMovement Default()
        {
            SettingsMovement settings;

            settings.runSpeed = 600.0f;
            settings.walkSpeed = 300.0f;
            settings.jumpForce = 500.0f;

            settings.longJumpForce = 1200.0f;

            settings.longJumpTime = 0.6f;

            settings.wallJumpPush = 400f;

            settings.wallJump = 0.8f;

            settings.justInTimeJump = 0.2f;

            settings.doubleJumpMax = 1;

            settings.crouchWalkSpeed = 0.5f;

            settings.groundMovementSmoothing = 0.15f;

            settings.airMovementSmoothing = 0.15f;

            settings.wallSurfingFriction = 0.5f;

            settings.airMovement = 0.5f;

            settings.jumpingGravity = 2.0f;

            settings.fallingGravity = 2.75f;

            settings.groundFriction = 1.0f;

            settings.airFriction = 0.2f;   

            return settings;


        }

    }


    // Settings VFX and SFX  
    [Serializable]
    public class SettingsEffect
    {
        [SerializeField]
        [Tooltip("Emitted effect in stepCheckObject position when walking. optional")]
        public GameObject dustWalkingEmitter;

        [SerializeField]
        [Tooltip("Emitted effect in stepCheckObject position when jumping. optional")]
        public GameObject dustJumpingEmitter;

        [Tooltip("Emitted effect in stepCheckObject when landing on ground. optional")]
        public GameObject dustLandingEmitter;

        [Tooltip("Emitted effect in frontWallCheckObject position when landing on ground. optional")]
        public GameObject dustWallSurfingEmitter;

        [Tooltip("Emitted effect in stepCheckObject position when double jumping. optional")]
        public GameObject dustDoubleJumpEmitter;

        [Tooltip("Time between effects when walking in seconds")]
        public float dustTime = 0.2f;
    }




    // Configuration Detection structure
    [Serializable]
    public class GroundSensor
    {
        [SerializeField]
        [Tooltip("Which layer is ground for ground and wall detection. optional")]
        public LayerMask whatIsGround;

        [SerializeField]
        [Tooltip("Object to detect ground to be placed at the bottom of the object sprite. optional")]
        public Transform groundCheckObject;

        [SerializeField]
        [Tooltip("Object to detect ceiling to be placed at the top of the object sprite. optional")]
        public Transform ceilingCheckObject;

        [SerializeField]
        [Tooltip("Object to spawn step effects and landing effects. optional")]
        public Transform stepCheckObject;

        [SerializeField]
        [Tooltip("Object to detect wall to be placed in front of character sprite. optional")]
        public Transform frontWallCheckObject;

        [SerializeField]
        [Tooltip("Collider used when crouching. optional")]
        public Collider2D crouchCollider;

        [SerializeField]
        [Tooltip("Collider used when standing. optional")]
        public Collider2D standCollider;

        [SerializeField]
        [Tooltip("Detection radius for ground and wall detection")]
        public float detectRadius = 0.1f;

        private float groundDetectionTime = -10f;
        private float ceilingDetectionTime = -10f;
        private float fronWallDetectionTime = -10f;
        private float detectionDuration = 0.15f;

        public void OnGroundDetected()
        {
            groundDetectionTime = Time.time;
        }

        public void OnCeilingDetected()
        {
            ceilingDetectionTime = Time.time;
        }

        public void OnFrontWallDetected()
        {
            fronWallDetectionTime = Time.time;
        }

        // Drawing sphere Gizmos for the detection
        public void OnDrawGizmos()
        {

            // Draw ground detection
            if (groundCheckObject)
            {
                DrawGizmo(groundCheckObject.position, detectRadius, groundDetectionTime);
            }

            // Draw ceiling detection
            if (ceilingCheckObject)
            {
                DrawGizmo(ceilingCheckObject.position, detectRadius, ceilingDetectionTime);
            }

            // Draw wall detection
            if (frontWallCheckObject)
            {
                DrawGizmo(frontWallCheckObject.position, detectRadius, fronWallDetectionTime);
            }
        }

        private void DrawGizmo(Vector3 position, float radius, float detectionTime)
        {
            if (Time.time - detectionTime < detectionDuration)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
            Gizmos.DrawWireSphere(position, detectRadius);

            Gizmos.color = new Color(0f, 0.1f, 1f, 0.25f);
            Gizmos.DrawSphere(position, detectRadius);

            if (Time.time - detectionTime < detectionDuration)
            {
                float factor = 1 - ((Time.time - detectionTime) / detectionDuration);
                Gizmos.color = new Color(1f, 0f, 0f, factor);
            }
            Gizmos.DrawSphere(position, detectRadius);
        }

    }







}