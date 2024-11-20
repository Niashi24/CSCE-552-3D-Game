using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody rbdy;

        public SphereCollider coll;

        public LayerMask groundMask;
        
        [Header("Parameters")]
        public float targetSpeed = 10f;

        public float acceleration = 50f;

        public float minJumpHeight;
        public float maxJumpHeight;

        public float coyoteTime;
        
        public float maxAirFallSpeed;
        public float airFallDeceleration;

        public float bounceReflectMultiplier = 0.75f;
        public float minBounceSpeed = 50f;
        
        public float boundForce = 16f;
        public float maxBoundFallSpeed;
        public float boundFallDeceleration;
        public float boundHeightMultiplier = 1.5f;
        
        [Header("Runtime")]
        public PlayerInput playerInput;
        public PlayerState playerState = PlayerState.Ground;

        public JumpStats jumpStats;
        
        public float jumpBuffer;

        public float coyoteTimer;

        public float jumpHoldTimer;
        public Vector3 jumpHoldForce;

        public bool canBound;
        
        public event Action OnJump;
        public event Action OnBounce;
        public event Action OnStartBounce;

        private void Update()
        {
            playerInput.Update
            (
                Input.GetKey(KeyCode.Z),
                Input.GetKey(KeyCode.X),
                Input.GetAxisRaw("Horizontal")
            );
        }

        public void SetState(PlayerState newState)
        {
            // On exit
            switch (playerState)
            {
                case PlayerState.Ground:
                    break;
                case PlayerState.Air:
                    break;
            }

            playerState = newState;
            
            // On enter
            switch (playerState)
            {
                case PlayerState.Ground:
                    break;
                case PlayerState.Air:
                    break;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            PlayerInput input = playerInput.Take();
            
            MoveForward();

            switch (playerState)
            {
                case PlayerState.Ground:
                    GroundUpdate(input);
                    break;
                case PlayerState.Air:
                    AirUpdate(input);
                    break;
            }
        }

        // TODO: Replace with spline pathing if necessary?
        void MoveForward()
        {
            float forwardSpeed = Vector3.Dot(transform.forward, rbdy.velocity);
            float newSpeed = Mathf.MoveTowards(forwardSpeed, targetSpeed, acceleration * Time.deltaTime);

            if (Input.GetKey(KeyCode.Space))
            {
                newSpeed = 0;
            }
            rbdy.velocity += transform.forward * (newSpeed - forwardSpeed);
        }

        void GroundUpdate(PlayerInput input)
        {
            // Jump
            if (input.jump.JustPressed)
            {
                jumpBuffer = 0f;
                var calculatedStats = JumpStats.FromMinMaxHeight(minJumpHeight, maxJumpHeight, Physics.gravity.magnitude);
                
                rbdy.velocity += calculatedStats.InitialVelocity * Vector3.up;

                jumpHoldForce = calculatedStats.HoldForce * Vector3.up;
                jumpHoldTimer = calculatedStats.Time;
                
                OnJump?.Invoke();
                
                SetState(PlayerState.Air);
            }
        }

        void AirUpdate(PlayerInput input)
        {
            // if (Physics.SphereCast(rbdy.position, coll.radius, -transform.up, out var ground, 0.01f, groundMask))
            // {
            //     if (ground.normal.y > 0)
            //     {
            //         Vector2 newDir = FloatExtensions.ChangeToSurface(velocity.normalized, hit.normal);
            //         if (input.bound.Pressed && newDir.magnitude < cosMinAngle - 0.01f && velocity.magnitude >= minBounceSpeed)
            //         {
            //             velocity = velocity.Reflect(hit.normal) * bounceReflectMultiplier;
            //             JumpStats bounceJumpStats = JumpStats.FromInitialVelAndMultiplier(velocity.magnitude, Physics2D.gravity.magnitude, boundHeightMultiplier);
            //             jumpHoldForce = velocity.normalized * bounceJumpStats.HoldForce;
            //             jumpHoldTimer = bounceJumpStats.Time.Log();
            //             canBound = false;
            //             // send bounce event
            //             OnBounce?.Invoke();
            //         }
            //         else
            //         {
            //             var oldVelocity = velocity;
            //             velocity = velocity.WithDirection(newDir.normalized);
            //     
            //             // if ground is smooth enough, keep all speed
            //             // otherwise lose speed
            //             if (newDir.magnitude + 0.02f < cosMinAngle)
            //             {
            //                 velocity *= newDir.magnitude;
            //                 Debug.Log($"Lost momentum (hard hit): {velocity.magnitude - oldVelocity.magnitude} from change: {newDir.magnitude}");
            //             }
            //             SetState(PlayerState.Ground);
            //         }
            //         
            //         break;
            //     }
            // }
            
            if (rbdy.velocity.y > -maxAirFallSpeed)
            {
                rbdy.velocity = rbdy.velocity.WithY(
                    Mathf.MoveTowards(rbdy.velocity.y, -maxAirFallSpeed, Physics.gravity.magnitude * Time.deltaTime));
            }
            else // vel <= maxBoundFallSpeed
            {
                rbdy.velocity = rbdy.velocity.WithY(
                    Mathf.MoveTowards(rbdy.velocity.y, -maxAirFallSpeed,
                        airFallDeceleration * Time.deltaTime)
                );
            }
            
            if (
                (input.jump.Pressed || input.bound.Pressed) && 
                jumpHoldTimer > 0 &&
                Vector3.Dot(rbdy.velocity.normalized, jumpHoldForce) >= 0)
            {
                rbdy.velocity += jumpHoldForce * Time.deltaTime;
                jumpHoldTimer = (jumpHoldTimer - Time.deltaTime).AtLeast(0f);
            }
            else
            {
                jumpHoldTimer = 0;
            }

            if (input.jump.JustPressed)
            {
                if (coyoteTimer > 0)
                {
                    var calculatedStats = JumpStats.FromMinMaxHeight(minJumpHeight, maxJumpHeight, Physics.gravity.magnitude);
                
                    rbdy.velocity += calculatedStats.InitialVelocity * Vector3.up;

                    jumpHoldForce = calculatedStats.HoldForce * Vector3.up;
                    jumpHoldTimer = calculatedStats.Time;
                }
                else
                {
                    jumpBuffer = 0.1f;
                }
            }
            else
            {
                jumpBuffer = (jumpBuffer - Time.deltaTime).AtLeast(0f);
            }
            
            if (input.bound.JustPressed)
            {
                OnStartBounce?.Invoke();
                canBound = true;
            }
            
            if (canBound && input.bound.Pressed)
            {
                if (rbdy.velocity.y > -maxBoundFallSpeed)
                {
                    rbdy.velocity = rbdy.velocity.WithY(Mathf.MoveTowards(rbdy.velocity.y, -maxBoundFallSpeed, boundForce * Time.deltaTime));
                }
                else // vel <= maxBoundFallSpeed
                {
                    rbdy.velocity = rbdy.velocity.WithY(Mathf.MoveTowards(rbdy.velocity.y, -maxBoundFallSpeed,
                        boundFallDeceleration * Time.deltaTime));
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var hit = collision.contacts[0];
            if (collision.contacts[0].normal.y > 0f)
            {
                Debug.Log("impulse: " + collision.impulse);
                
                Vector3 newDir = FloatExtensions.ChangeToSurface(rbdy.velocity.normalized, hit.normal);
                if (playerInput.bound.Pressed && rbdy.velocity.magnitude >= minBounceSpeed)
                {
                    rbdy.velocity -= collision.contacts[0].impulse;
                    rbdy.velocity = Vector3.Reflect(rbdy.velocity, hit.normal) * bounceReflectMultiplier;
                    Debug.Log(rbdy.velocity);
                    JumpStats bounceJumpStats = JumpStats.FromInitialVelAndMultiplier(rbdy.velocity.magnitude, Physics.gravity.magnitude, boundHeightMultiplier);
                    jumpHoldForce = rbdy.velocity.normalized * bounceJumpStats.HoldForce;
                    jumpHoldTimer = bounceJumpStats.Time;
                    canBound = false;
                    // send bounce event
                    OnBounce?.Invoke();
                }
                else
                {
                    var oldVelocity = rbdy.velocity;
                    rbdy.velocity = rbdy.velocity.WithDirection(newDir.normalized);
                
                    // if ground is smooth enough, keep all speed
                    // otherwise lose speed
                    // if (newDir.magnitude + 0.02f < cosMinAngle)
                    // {
                    //     velocity *= newDir.magnitude;
                    //     Debug.Log($"Lost momentum (hard hit): {velocity.magnitude - oldVelocity.magnitude} from change: {newDir.magnitude}");
                    // }
                    SetState(PlayerState.Ground);
                }
            }
        }
    }
}
