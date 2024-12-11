using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Csce552
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody rbdy;

        public SphereCollider coll;

        public LayerMask groundMask;

        public GameObject mesh;

        public GameObject anchor;

        public AudioSource audioSource;

        public AudioClip jumpLandSfx;
        
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

        public float rotateDecayParam = 1f;
        
        [Header("Runtime")]
        public PlayerInput playerInput;
        public PlayerState playerState = PlayerState.Ground;

        public JumpStats jumpStats;
        
        public float jumpBuffer;

        public float coyoteTimer;

        public float jumpHoldTimer;
        public float jumpHoldForce;

        public bool canBound;
        
        public event Action OnJump;
        public event Action OnBounce;
        public event Action OnStartBounce;

        public int lane = 0;

        private void Update()
        {
            playerInput.Update
            (
                Input.GetKey(KeyCode.Z),
                Input.GetKey(KeyCode.X),
                Input.GetKey(KeyCode.LeftArrow),
                Input.GetKey(KeyCode.RightArrow)
            );
        }

        public void SetState(PlayerState newState)
        {
            if ((playerState == PlayerState.Ground && newState == PlayerState.Air)
                || (playerState == PlayerState.Air && newState == PlayerState.Ground))
            {
                Debug.Log("played");
                audioSource.PlayOneShot(jumpLandSfx);
            }
            
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
            transform.localPosition = transform.localPosition.WithX(0f);
            PlayerInput input = playerInput.Take();
            
            MoveForward();

            HandleLaneSwitch(input);

            switch (playerState)
            {
                case PlayerState.Ground:
                    GroundUpdate(input);
                    break;
                case PlayerState.Air:
                    AirUpdate(input);
                    break;
            }
            transform.localPosition = transform.localPosition.WithX(0f);
        }

        void HandleLaneSwitch(PlayerInput input)
        {
            if (input.left.JustPressed && lane > -1)
            {
                lane -= 1;
            }

            if (input.right.JustPressed && lane < 1)
            {
                lane += 1;
            }

            Quaternion rotation = anchor.transform.rotation;

            float curRot = rotation.eulerAngles.z;
            float target = lane * 45f;
            float newRot = Mathf.LerpAngle(curRot, target, 1 - Mathf.Exp(-rotateDecayParam * Time.deltaTime));

            rotation.eulerAngles = rotation.eulerAngles.WithZ(newRot);
            anchor.transform.rotation = rotation;
        }

        // TODO: Replace with spline pathing if necessary?
        void MoveForward()
        {
            float forwardSpeed = Vector3.Dot(transform.forward, rbdy.velocity);
            float newSpeed = Mathf.MoveTowards(forwardSpeed, targetSpeed, acceleration * Time.deltaTime);

            var newRotation = mesh.transform.rotation;
            newRotation *= Quaternion.Euler(90,0,0);
            mesh.transform.rotation = Quaternion.Lerp(mesh.transform.rotation, newRotation, Time.deltaTime * 8f);

            if (Input.GetKey(KeyCode.Space))
            {
                newSpeed = 0;
            }
            rbdy.velocity += transform.forward * (newSpeed - forwardSpeed);
        }

        void GroundUpdate(PlayerInput input)
        {
            if (!Physics.SphereCast(rbdy.position, coll.radius, -transform.up, out var ground, 0.5 groundMask))
            {
                SetState(PlayerState.Air);
            }
            
            // Jump
            if (input.jump.JustPressed)
            {
                jumpBuffer = 0f;
                var calculatedStats = JumpStats.FromMinMaxHeight(minJumpHeight, maxJumpHeight, Physics.gravity.magnitude);
                
                rbdy.velocity += calculatedStats.InitialVelocity * transform.up;

                jumpHoldForce = calculatedStats.HoldForce;
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

            Vector3 down = -transform.up;
            float downSpeed = Vector3.Dot(rbdy.velocity, down);
            
            if (downSpeed > -maxAirFallSpeed)
            {
                downSpeed = Mathf.MoveTowards(downSpeed, maxAirFallSpeed, Physics.gravity.magnitude * Time.deltaTime);
                // rbdy.velocity += transform.up * (-Physics.gravity.magnitude * Time.deltaTime);
                // rbdy.velocity = rbdy.velocity.MaxInDirection(down, maxAirFallSpeed);
                rbdy.velocity = rbdy.velocity.InDirection(down, downSpeed);
                // rbdy.velocity = rbdy.velocity.WithY(
                //     Mathf.MoveTowards(rbdy.velocity.y, -maxAirFallSpeed, Physics.gravity.magnitude * Time.deltaTime));
            }
            else // vel <= maxBoundFallSpeed
            {
                downSpeed = Mathf.MoveTowards(downSpeed, maxAirFallSpeed, airFallDeceleration * Time.deltaTime);
                rbdy.velocity = rbdy.velocity.InDirection(down, downSpeed);
                // rbdy.velocity = rbdy.velocity.WithY(
                //     Mathf.MoveTowards(rbdy.velocity.y, -maxAirFallSpeed,
                //         airFallDeceleration * Time.deltaTime)
                // );

            }
            
            if (
                (input.jump.Pressed || input.bound.Pressed) && 
                jumpHoldTimer > 0 &&
                Vector3.Dot(rbdy.velocity.normalized, transform.up) >= 0)
            {
                rbdy.velocity += jumpHoldForce * Time.deltaTime * transform.up;
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
                
                    rbdy.velocity += calculatedStats.InitialVelocity * transform.up;

                    jumpHoldForce = calculatedStats.HoldForce;
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

        private void HandleCollision(Collision collision)
        {
            if (playerState == PlayerState.Ground)
            {
                rbdy.velocity -= collision.impulse;
                return;
            }
            
            var hit = collision.contacts[0];
            if (collision.contacts[0].normal.y > 0f)
            {
                // Debug.Log("impulse: " + collision.impulse);
                
                Vector3 newDir = FloatExtensions.ChangeToSurface(rbdy.velocity.normalized, hit.normal);
                if (playerInput.bound.Pressed && rbdy.velocity.magnitude >= minBounceSpeed)
                {
                    rbdy.velocity -= collision.contacts[0].impulse;
                    rbdy.velocity = Vector3.Reflect(rbdy.velocity, hit.normal) * bounceReflectMultiplier;
                    Debug.Log(rbdy.velocity);
                    JumpStats bounceJumpStats = JumpStats.FromInitialVelAndMultiplier(rbdy.velocity.magnitude, Physics.gravity.magnitude, boundHeightMultiplier);
                    jumpHoldForce = bounceJumpStats.HoldForce;
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
            transform.localPosition = transform.localPosition.WithX(0f);
        }

        private void OnCollisionStay(Collision other)
        {
            HandleCollision(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision);
        }
    }
}
