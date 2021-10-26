using System;
using UnityEngine;


namespace CaribeAtomic
{

        public class CharacterMotor2D : MonoBehaviour
        {
            /// <summary>
            /// Cached CharacterMotor Movement Settings Class.
            /// </summary>
            [Tooltip("Settings Character Movement")]
            public SettingsMovement  m_movement = SettingsMovement.Default();

            /// <summary>
            /// Cached CharacterMotor Activation Settings Class.
            /// </summary>
             [Tooltip("Settings for On/Off Bool Fields.")]
            public SettingsBools m_activation = SettingsBools.Default();

        
            [Tooltip("Settings for Objects Structure Detection")]
            public GroundSensor m_detection;

            [SerializeField]
            public SettingsEffect m_effect;

            public Core m_core { get; private set; }


            private bool m_startFacingRight = true;

            // True if the character is crouching
            protected bool m_isCrouching = false;

            // True if the character is not on ground and going up
            protected bool m_isJumping = false;

            // True if the character is not on ground and going up
            protected bool m_isWalking = false;

            // True if the character is not on ground and going up
            protected bool m_isRunning = false;

            // True if the character is not on the ground and going down
            protected bool m_isFalling = false;

            // True if the character is wall surfing
            protected bool m_isWallSurfing = false;

            // When the Character is Landing from falling
            protected bool m_Landing = false;


     

            // True if character is on ground
            protected bool m_isGrounded = false;

            // Reference to velocity
            protected Vector3 m_velocity = Vector3.zero;

            // Last known position
            protected Vector3 m_lastPosition;

            // True if front wall is detected
            protected bool m_frontWallDetected = false;

            // Maximum time to end long jump while jumping 
            private float m_longJumpEndTime = 0.0f;

            // Check if the jump button was released
            private bool m_releaseJump = true;

            // Time when the jump button was released
            private float m_releaseJumpTime = 0.0f;

            // Number of double jump executed
            private int m_doubleJumpCount = 0;

            // Time when the jump started
            private float m_jumpStartTime = 0.0f;

            // Last time dust effect was spawned
            private float m_dustEndTime = 0.0f;

            // If the character can currently double jump
            private bool m_canDoubleJump = false;

            // If the character can currently wall jump
            private bool m_canWallJump = false;

            // Last valid wall jump direction
            private bool m_wallJumpRight = false;

            // Local rigid body of character to apply movement
            protected Rigidbody2D m_localRigidbody2D;

            // Stop movement until time
            private float m_movementDisableTime = 0f;

            // Slow horizon just after wall jumping
            private float m_wallJumpSlowDuration = 0.5f;

            // Slow horizontal until 
            private float m_wallJumpEndTime = 0f;

            // 
            private int m_lastFrameCount = 0;

            private bool m_canJump = false;

            // The last time the character can jump
            private float m_canJumpTime = 0f;

            // The last time the character can jump
            private float m_canWallJumpTime = 0f;

            private Animator animator;
           
            private bool m_isFacingRight;
           


         public bool IsFacingRight
        { get => m_activation.m_isFacingRight; 
          set => m_activation.m_isFacingRight = value; }

      
        public void Start()
            {
                // Get the local Rigibody 2d
                m_localRigidbody2D = GetComponent<Rigidbody2D>();
                if (m_localRigidbody2D)
                {
                    m_localRigidbody2D.sharedMaterial = new PhysicsMaterial2D();
                    m_localRigidbody2D.sharedMaterial.bounciness = 0.0f;
                    m_localRigidbody2D.sharedMaterial.friction = 0.0f;
                }

                // Get the animator if the array is not set
                if (animator == null )
                {
                    animator = GetComponent<Animator>();
              
                }

                
                 m_isFacingRight = m_activation.m_isFacingRight;

                 //m_core = GetComponent<Core>();

            // Last position to use with friction
            m_lastPosition = transform.position;

                if (!m_startFacingRight)
                {
                    Flip();
                }
            }

        private void OnDrawGizmos()
            {
                if (m_detection != null)
                {
                    m_detection.OnDrawGizmos();
                }
            }

        /// <summary>
        /// Walls Checker: Is this character in front an Wall?
        /// </summary>
        private void CheckWalls()
            {
                Collider2D[] colliders;

                // if the front wall check object is set
                if (m_detection.frontWallCheckObject)
                {
                    m_frontWallDetected = false;

                    // Get everything in the detect radius that is ground
                    colliders = Physics2D.OverlapCircleAll(m_detection.frontWallCheckObject.position, m_detection.detectRadius, m_detection.whatIsGround);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        // unless it is the gameObject
                        if (colliders[i].gameObject != gameObject && !colliders[i].isTrigger)
                        {
                            // front wall is detected
                            m_frontWallDetected = true;
                            m_detection.OnFrontWallDetected();

                            // stop here
                            break;
                        }
                    }
                }
            }


        //private CharacterState m_motorStates; To be used in the future.

        /// <summary>
        /// Grounded Checker: Is this character standing on VALID 'ground'?
        /// </summary>
        private void CheckGround()
            {
                // No ground check object
                if (!m_detection.groundCheckObject)
                {
                    // Always on ground
                    m_isGrounded = true;
                    return;
                }

                // If going up
                if (!m_isGrounded && m_localRigidbody2D.velocity.y > 0.05f)
                {
                    // Dont check ground
                    m_isGrounded = false;
                    return;
                }

                bool wasGrounded = m_isGrounded;
                m_isGrounded = false;

                // detects colliders below character
                Collider2D[] colliders = Physics2D.OverlapCircleAll(m_detection.groundCheckObject.position, m_detection.detectRadius, m_detection.whatIsGround);


                for (int i = 0; i < colliders.Length; i++)
                {
                    // If collider is not gameObject
                    if (colliders[i].gameObject != gameObject && !colliders[i].isTrigger)
                    {
                        m_isGrounded = true;
                        m_detection.OnGroundDetected();

                        // If was grounded and objects are set
                        if (m_effect.dustLandingEmitter && m_detection.stepCheckObject && !wasGrounded)
                        {
                            // spawn dust landing effect
                            Instantiate(m_effect.dustLandingEmitter, m_detection.stepCheckObject.position, m_detection.stepCheckObject.rotation);
                        }
                        break;
                    }
                }

                // Set the Jump variable 
                if (m_isGrounded)
                {
                    m_canJump = true;
                }
            }

        /// <summary>
        /// Create sprites for facing right and facing left and switch between them as needed
        /// <summary>
        public void Flip()
                {
                    // Switch object facing
                    m_isFacingRight = !m_isFacingRight;
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                    // Check wall a 2nd time because we
                    CheckWalls();
                }
   
        public void MovementDisable(float duration)
            {
                m_movementDisableTime = Time.time + duration;
            }
                    
        public void MovementEnable()
            {
                m_movementDisableTime = 0f;
            }


        #region MOVE METHOD
        /// <summary>
        /// Is the main Method for Character Motor and make the Character Move.
        /// Move the character toward a direction or make it crouch / jump / walk
        /// <param name="direction">move horizotal on X coordenate</param>
        /// <param name="crouch"> true: make the character crouch, false: make the character stand</param>
        /// <param name="jump">true: character jump </param>
        /// <param name="walkToggle"> make character switch between walk / run. </param>
        /// <summary>
        public void Move(float direction, bool crouch, bool jump, bool walkToggle)
            {

           
            // Skip if already called for this frame
            if (m_lastFrameCount == Time.frameCount)
                {
                    return;
                }
                m_lastFrameCount = Time.frameCount;
                Vector3 currentPosition = transform.position;

                if (m_movementDisableTime >= Time.time)
                {
                    direction = 0f;
                    crouch = false;
                    jump = false;
                    walkToggle = false;
                }

                bool running = false;
                bool walking = false;
                bool crouching = false;

          
                if (!m_activation.enableAlwaysRun && walkToggle || m_activation.enableAlwaysRun && !walkToggle)
                {
                    running = true;
                    walking = false;
                }
                else
                {
                    running = false;
                    walking = true;
                }
                // Dont enable jump
                if (jump && !m_activation.enableJump)
                {
                    jump = false;
                }

                // We can't crouch and jump at the same time
                if (crouch && jump)
                {
                    crouch = false;
                    crouching = false;
                }

                // Check the ground
                CheckGround();

                // Check the walls
                CheckWalls();

                // Check when the jump button is released
                if (m_activation.enableJump && !jump && !m_releaseJump && m_releaseJumpTime < Time.time + 0.1f)
                {
                    m_releaseJump = true;
                }

                // Set the speed
                float speed = 0;
                if (running)
                {
                    speed = direction * m_movement.runSpeed * Time.deltaTime * 100;
                }
                else
                {
                    speed = direction * m_movement.walkSpeed * Time.deltaTime * 100;
                }

                // Check if character can stand
                if (m_detection.ceilingCheckObject && !crouch && m_isGrounded)
                {
                    // Keep character crouching
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(m_detection.ceilingCheckObject.position, m_detection.detectRadius, m_detection.whatIsGround);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        // unless it is the gameObject
                        if (colliders[i].gameObject != gameObject && !colliders[i].isTrigger)
                        {
                            crouch = true;
                            crouching = true;
                            m_detection.OnCeilingDetected();

                            // Also cannot jump when crouched
                            jump = false;
                        }
                    }
                }

                // only control the character if grounded
                if (m_isGrounded)
                {
                    // If crouching
                    if (m_activation.enableCrouch && crouch)
                    {
                        crouching = true;
                        walking = true;
                        running = false;
                        speed = direction * m_movement.walkSpeed * m_movement.crouchWalkSpeed * Time.deltaTime * 100;

                        // Change colliders when crouching
                        if (m_detection.crouchCollider != null)
                        {
                            m_detection.crouchCollider.isTrigger = false;
                        }
                        if (m_detection.standCollider != null)
                        {
                            m_detection.standCollider.isTrigger = true;
                        }
                    }
                    else
                    {
                        crouching = false;

                        // Change colliders when crouching
                        if (m_detection.standCollider != null)
                        {
                            m_detection.standCollider.isTrigger = false;
                        }
                        if (m_detection.crouchCollider != null)
                        {
                            m_detection.crouchCollider.isTrigger = true;
                        }
                    }

                    if (speed != 0)
                    {
                        // Move the character by finding the target velocity
                        Vector2 targetVelocity = new Vector2(speed * Time.deltaTime, m_localRigidbody2D.velocity.y);

                        // And then smoothing it out and applying it to the character
                        m_localRigidbody2D.velocity = Vector3.SmoothDamp(m_localRigidbody2D.velocity, targetVelocity, ref m_velocity, m_movement.groundMovementSmoothing);
                    }

                    // Simulated horizontal friction
                    if (m_localRigidbody2D.velocity.x != 0)
                    {
                        Vector2 friction = Vector2.zero;
                        friction.x = m_localRigidbody2D.velocity.x - m_localRigidbody2D.velocity.x * 10.0f * Time.deltaTime * m_movement.groundFriction;
                        friction.y = m_localRigidbody2D.velocity.y;

                        // Do not reverse velocity because of friction
                        if (m_localRigidbody2D.velocity.x > 0 && friction.x < 0 || m_localRigidbody2D.velocity.x > 0 && friction.x < 0)
                        {
                            friction.x = 0;
                        }
                        m_localRigidbody2D.velocity = friction;
                    }

                    // Set gravity
                    m_localRigidbody2D.gravityScale = 1.0f;

                    // If the input is moving the character right and the character is facing left...
                    if (speed > 0 && !m_isFacingRight)
                    {
                        // Flip the character
                        Flip();
                    }
                    // Otherwise if the input is moving the character left and the character is facing right...
                    else if (speed < 0 && m_isFacingRight)
                    {
                        // Flip the character
                        Flip();
                    }
                }

                // Only controls character if it is in the air
                if (!m_isGrounded)
                {
                    if (m_detection.crouchCollider)
                    {
                        m_detection.crouchCollider.isTrigger = true;
                    }
                    if (m_detection.standCollider)
                    {
                        m_detection.standCollider.isTrigger = false;
                    }

                    // only control the character if airControl is turned on
                    if (m_activation.enableAirControl)
                    {
                        if (speed != 0)
                        {
                            // slow air control just after a wall jump
                            if (Time.time < m_wallJumpEndTime)
                            {
                                float factor = (1 - ((m_wallJumpEndTime - Time.time) / m_wallJumpSlowDuration));
                                speed *= factor;
                            }

                            // Move the character
                            Vector3 targetVelocity = new Vector2(speed * Time.deltaTime * m_movement.airMovement, m_localRigidbody2D.velocity.y);

                            // And then smoothing it out and applying it to the character
                            m_localRigidbody2D.velocity = Vector3.SmoothDamp(m_localRigidbody2D.velocity, targetVelocity, ref m_velocity, m_movement.airMovementSmoothing);
                        }
                    }

                    // Simulated air friction
                    if (m_localRigidbody2D.velocity.x != 0)
                    {
                        Vector2 friction = Vector2.zero;
                        friction.x = m_localRigidbody2D.velocity.x - m_localRigidbody2D.velocity.x * 5.0f * Time.deltaTime * m_movement.airFriction;
                        friction.y = m_localRigidbody2D.velocity.y;

                        // Do not reverse velocity because of friction
                        if (m_localRigidbody2D.velocity.x > 0 && friction.x < 0 || m_localRigidbody2D.velocity.x > 0 && friction.x < 0)
                        {
                            friction.x = 0;
                        }
                        m_localRigidbody2D.velocity = friction;
                    }

                    // Simulated air push by other objects
                    if (m_lastPosition != null && m_activation.enableAirPush)
                    {
                        Vector2 targetVelocity = new Vector2(currentPosition.x - m_lastPosition.x, currentPosition.y - m_lastPosition.y);
                        m_localRigidbody2D.velocity = Vector3.SmoothDamp(m_localRigidbody2D.velocity, targetVelocity * 35f, ref m_velocity, 0.15f);
                    }

                    // Set falling gravity
                    if (m_localRigidbody2D.velocity.y < 0)
                    {
                        m_localRigidbody2D.gravityScale = m_movement.fallingGravity;
                    }
                    else
                    {
                        m_localRigidbody2D.gravityScale = m_movement.jumpingGravity;
                    }

                    // If the input is moving the character right and the character is facing left
                    if (speed > 0 && !m_isFacingRight && m_wallJumpEndTime < Time.time)
                    {
                        // Flip the character
                        Flip();
                    }
                    // Otherwise if the input is moving the character left and the character is facing right
                    else if (speed < 0 && m_isFacingRight && m_wallJumpEndTime < Time.time)
                    {
                        // Flip the character
                        Flip();
                    }
                }

                // Stops long jump if character is going down
                if (!m_isGrounded && Time.time > m_longJumpEndTime && m_localRigidbody2D.velocity.y <= 0)
                {
                    m_longJumpEndTime = Time.time;
                }


                // Prevents multiple jumps if character is close to ground in multiple frames
                bool justJumped = false;
                if (Time.time - m_jumpStartTime < 0.2f)
                {
                    justJumped = true;
                }

                // Just in time jump
                if (m_activation.enableJump && m_releaseJump && m_isGrounded && !justJumped)
                {
                    m_canJumpTime = Time.time + m_movement.justInTimeJump;
                }

                // Just in time wall jump
                if (m_activation.enableJump && !m_isGrounded && m_activation.enableWallJump && m_canWallJump && !justJumped)
                {
                    m_canWallJumpTime = Time.time + m_movement.justInTimeJump;
                }

                // Character jump
                if (m_activation.enableJump && m_releaseJump && m_isGrounded && !justJumped && jump
                    || m_activation.enableJump && m_releaseJump && !m_isGrounded && !justJumped && jump && m_canJumpTime > Time.time)
                {
                    // Add a vertical force to the character
                    m_isGrounded = false;
                    m_releaseJump = false;
                    m_canJump = false;
                    m_canDoubleJump = false;
                    m_releaseJumpTime = Time.time;
                    m_jumpStartTime = Time.time;
                    m_longJumpEndTime = Time.time + m_movement.longJumpTime;
                    m_doubleJumpCount = m_movement.doubleJumpMax;

                    m_localRigidbody2D.AddForce(new Vector2(0f, m_movement.jumpForce));

                    // add jump effect
                    if (m_effect.dustJumpingEmitter && m_detection.stepCheckObject)
                    {
                        Instantiate(m_effect.dustJumpingEmitter, m_detection.stepCheckObject.position, m_detection.stepCheckObject.rotation);
                    }
                }
                else if (m_activation.enableJump && !m_isGrounded && jump)
                {
                    // Check for wall jump
                    if (m_activation.enableWallJump && m_canWallJump && m_releaseJump && !justJumped
                        || m_activation.enableWallJump && !justJumped && m_releaseJump && m_canWallJumpTime > Time.time)
                    {
                        // Add wall jump
                        float horizontalForce = 0;
                        if (!m_wallJumpRight)
                        {
                            horizontalForce = m_movement.wallJumpPush;
                        }
                        else
                        {
                            horizontalForce = -m_movement.wallJumpPush;
                        }
                        m_localRigidbody2D.velocity = Vector2.zero;
                        m_localRigidbody2D.AddRelativeForce(new Vector2(horizontalForce, m_movement.jumpForce * m_movement.wallJump));

                        // add wall jump effect
                        if (m_effect.dustLandingEmitter && m_detection.stepCheckObject)
                        {
                            Instantiate(m_effect.dustLandingEmitter, m_detection.stepCheckObject.position, m_detection.stepCheckObject.rotation);
                        }

                        // Change direction if needed
                        if (m_wallJumpRight == m_isFacingRight)
                        {
                            Flip();
                        }

                        m_canWallJump = false;
                        m_releaseJump = false;
                        m_canDoubleJump = false;
                        m_releaseJumpTime = Time.time;
                        m_jumpStartTime = Time.time;
                        m_longJumpEndTime = Time.time + m_movement.longJumpTime;
                        m_wallJumpEndTime = Time.time + m_wallJumpSlowDuration;
                        m_doubleJumpCount = m_movement.doubleJumpMax;
                    }

                    // Check for double jump
                    else if (m_activation.enableDoubleJump && !justJumped && m_canDoubleJump)
                    {
                        // Double jump factor is from 25% to 100% after 1s
                        float doubleJumpFactor = Mathf.Max(0.25f, Mathf.Min(1.0f, Time.time - m_jumpStartTime));

                        // Add double jump
                        m_localRigidbody2D.AddRelativeForce(new Vector2(0f, m_movement.jumpForce * doubleJumpFactor));

                        // double jump effect
                        if (m_effect.dustDoubleJumpEmitter && m_detection.stepCheckObject)
                        {
                            Instantiate(m_effect.dustDoubleJumpEmitter, m_detection.stepCheckObject.position, m_detection.stepCheckObject.rotation);
                        }

                        m_releaseJump = false;
                        m_canDoubleJump = false;
                        m_releaseJumpTime = Time.time;
                        m_jumpStartTime = Time.time;
                        m_longJumpEndTime = Time.time + m_movement.longJumpTime;
                        m_doubleJumpCount--;
                    }
                    // Check long jump
                    else if (m_activation.enableLongJump && Time.time < m_longJumpEndTime)
                    {
                        // Add long jump
                        m_localRigidbody2D.AddRelativeForce(new Vector2(0f, Mathf.Lerp(0.0f, m_movement.longJumpForce, (m_longJumpEndTime - Time.time) / m_movement.longJumpTime) * Time.deltaTime));
                    }
                }

                if (!m_isGrounded && !jump)
                {
                    if (m_doubleJumpCount > 0)
                    {
                        m_canDoubleJump = true;
                    }
                }

                m_isWallSurfing = false;

                // Wall surfing
                if (!m_isGrounded && m_localRigidbody2D.velocity.y < 0 && m_frontWallDetected && Mathf.Abs(direction) > 0)
                {
                    if (m_activation.enableWallSurfing)
                    {
                        //m_localRigidbody2D.AddRelativeForce(new Vector2(0f, Mathf.Abs(m_localRigidbody2D.velocity.y) * 10));
                        if (m_movement.wallSurfingFriction == 1f)
                        {
                            m_localRigidbody2D.velocity = new Vector2(m_localRigidbody2D.velocity.x, 0);
                            m_localRigidbody2D.gravityScale = 0f;
                        }
                        else
                        {
                            m_localRigidbody2D.velocity = new Vector2(m_localRigidbody2D.velocity.x, m_localRigidbody2D.velocity.y * (1 - m_movement.wallSurfingFriction));
                        }

                        m_isWallSurfing = true;

                        if (m_effect.dustWallSurfingEmitter && m_detection.frontWallCheckObject && Time.time > m_dustEndTime)
                        {
                            Instantiate(m_effect.dustWallSurfingEmitter, m_detection.frontWallCheckObject.position, m_detection.frontWallCheckObject.rotation);
                            m_dustEndTime = Time.time + m_effect.dustTime;
                        }
                    }
                }

                // Enable wall jump
                if (m_activation.enableWallJump && !m_isGrounded && m_frontWallDetected)
                {
                    m_canWallJump = true;
                    m_wallJumpRight = m_isFacingRight;
                }
                else
                {
                    m_canWallJump = false;
                }

                // Walking dust
                if (m_effect.dustWalkingEmitter && m_isGrounded && Time.time > m_dustEndTime && m_detection.stepCheckObject != null && Mathf.Abs(m_localRigidbody2D.velocity.x) > 0.75f)
                {
                    Instantiate(m_effect.dustWalkingEmitter, m_detection.stepCheckObject.position, m_detection.stepCheckObject.rotation);
                    m_dustEndTime = Time.time + m_effect.dustTime;
                }

                // Set the variables for animator
                if (!m_isGrounded)
                {
                    m_isWalking = false;
                    m_isRunning = false;
                    m_isCrouching = false;
                    m_Landing = false;

                    if (m_localRigidbody2D.velocity.y >= 0)
                    {
                        m_isJumping = true;
                        m_isFalling = false;
                    }
                    else
                    {
                        m_isJumping = false;
                        m_isFalling = true;
                    }
                }
                else
                {
                    m_isJumping = false;
                    m_isFalling = false;
                    m_isCrouching = crouching;

                    if (Mathf.Abs(m_localRigidbody2D.velocity.x) >= 0.05f)
                    {
                        m_isWalking = walking;
                        m_isRunning = running;
                    }
                    else
                    {
                        m_isWalking = false;
                        m_isRunning = false;
                    }
                }

                if (Mathf.Abs(direction) == 0)
                {
                    m_isRunning = false;
                    m_isWalking = false;
                }

                // Send all fields variables to the animator
                AnimatorFields();

                m_lastPosition = transform.position;
            }

        #endregion MOVE METHOD

        #region ANIMATOR VARIABLES
        // Set All Fields to be played on the animator 
        //The strings are stored into a struct ex: k_Ground = "On_Grounded"
        private void AnimatorFields()
            {
                    if (animator== null )
                    {
                      print(string.Format("{0}, Cannot be Empty", animator));
                        return;
                    }  else {

                    animator.SetBool(Const.k_Ground, m_isGrounded);   //To be implemneted in the future (m_core.GetCurrentState = new CharacterState(m_core, "OnGrounded", m_isGrounded);

                    animator.SetBool(Const.k_Falling, m_isFalling);
                    if (m_activation.enableJump)
                    {
                        animator.SetBool(Const.k_Jump, m_isJumping);
                    }
                    if (m_activation.enableCrouch)
                    {
                        animator.SetBool(Const.k_Crouch, m_isCrouching);
                    }
                    if (m_activation.enableWalk)
                    {
                    animator.SetBool(Const.k_Walking, m_isWalking);  //To be implemneted in the future  m_core.GetCurrentState = new CharacterState(m_core, string.Format("{0}", Const.k_Walking), m_isWalking);

                    }


                    animator.SetBool(Const.k_Running, m_isRunning);
                    animator.SetBool(Const.k_Wall, m_isWallSurfing);
                    animator.SetBool("OnFacingRight", m_isFacingRight);
                    animator.SetFloat("hSpeed", Mathf.Abs(m_localRigidbody2D.velocity.x));
                    animator.SetFloat("vSpeed", Mathf.Abs(m_localRigidbody2D.velocity.y));
                }
            }

        #endregion ANIMATOR VARIABLES

        public bool GetIsFacingRight() => m_isFacingRight;

        public bool GetIsGrounded() => m_isGrounded;

        public bool GetIsFalling() => m_isFalling;
       
  
        public bool GetIsJumping() => m_isJumping;

        public bool GetIsRunning() => m_isRunning;

  
        public bool GetIsWalking() => m_isWalking;


        public bool GetIsCrouching() => m_isCrouching;
  
        }
    
}
