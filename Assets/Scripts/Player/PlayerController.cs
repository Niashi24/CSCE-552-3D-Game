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

        public AudioClip jumpSfx;
        public AudioClip landSfx;
        public AudioClip rollSfx;
        
        [Header("Parameters")]
        public float targetSpeed = 10f;

        public float acceleration = 50f;

        public float speedUp = 0.1f;

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

        public float groundHeight = 3f;
        
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

        public float yVelocity;
        public float forwardVelocity;

        public int score;
        public float scoreDisplacement;
        public float scoreMultiplier = 0.5f;

        private void Update()
        {
            playerInput.Update
            (
                Input.GetKey(KeyCode.Space),
                Input.GetKey(KeyCode.X),
                Input.GetKey(KeyCode.LeftArrow),
                Input.GetKey(KeyCode.RightArrow)
            );
        }

        public void SetState(PlayerState newState)
        {
            if (playerState == PlayerState.Ground && newState == PlayerState.Air)
            {
                audioSource.PlayOneShot(jumpSfx);
            } 
            else if (playerState == PlayerState.Air && newState == PlayerState.Ground)
            {
                // Debug.Log("played");
                audioSource.PlayOneShot(landSfx);
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
            rbdy.velocity = Vector3.zero;
            transform.localPosition = transform.localPosition.WithX(0f);
            PlayerInput input = playerInput.Take();
            targetSpeed += speedUp * Time.deltaTime;
            
            MoveForward();

            HandleLaneSwitch(input);

            float before = transform.localPosition.z;

            switch (playerState)
            {
                case PlayerState.Ground:
                    GroundUpdate(input);
                    break;
                case PlayerState.Air:
                    AirUpdate(input);
                    break;
            }
            
            float after = transform.localPosition.z;
            
            scoreDisplacement += (before - after) * scoreMultiplier;
            if ((int)scoreDisplacement > score)
            {
                EventManager.AddScore((int)scoreDisplacement - score);
                score = (int)scoreDisplacement;
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
            float newSpeed = Mathf.MoveTowards(forwardVelocity, targetSpeed, acceleration * Time.deltaTime);

            var newRotation = mesh.transform.rotation;
            newRotation *= Quaternion.Euler(90,0,0);
            mesh.transform.rotation = Quaternion.Lerp(mesh.transform.rotation, newRotation, Time.deltaTime * 8f);

            forwardVelocity = newSpeed;
        }

        void GroundUpdate(PlayerInput input)
        {
            // if (!Physics.SphereCast(rbdy.position, coll.radius, -transform.up, out var ground, 0.5f, groundMask))
            // {
            //     SetState(PlayerState.Air);
            // }
            //
            // Jump
            if (input.jump.JustPressed)
            {
                jumpBuffer = 0f;
                var calculatedStats = JumpStats.FromMinMaxHeight(minJumpHeight, maxJumpHeight, Physics.gravity.magnitude);

                yVelocity += calculatedStats.InitialVelocity;

                jumpHoldForce = calculatedStats.HoldForce;
                jumpHoldTimer = calculatedStats.Time;
                
                OnJump?.Invoke();
                
                SetState(PlayerState.Air);
            }
            
            transform.localPosition += new Vector3(0f, yVelocity, -forwardVelocity) * Time.deltaTime;
            transform.localPosition = transform.localPosition.WithY(groundHeight);
        }

        void AirUpdate(PlayerInput input)
        {

            Vector3 down = -transform.up;
            if (yVelocity > -maxAirFallSpeed)
            {
                yVelocity = Mathf.MoveTowards(yVelocity, -maxAirFallSpeed, Physics.gravity.magnitude * Time.deltaTime);
            }
            else // vel <= maxBoundFallSpeed
            {
                yVelocity = Mathf.MoveTowards(yVelocity, -maxAirFallSpeed, airFallDeceleration * Time.deltaTime);

            }
            
            if (
                (input.jump.Pressed || input.bound.Pressed) && 
                jumpHoldTimer > 0 &&
                yVelocity >= 0)
            {
                yVelocity += jumpHoldForce * Time.deltaTime;
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

                    yVelocity += calculatedStats.InitialVelocity;

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
                if (yVelocity > -maxBoundFallSpeed)
                {
                    yVelocity = Mathf.MoveTowards(yVelocity, -maxBoundFallSpeed, boundForce * Time.deltaTime);
                }
                else // vel <= maxBoundFallSpeed
                {
                    yVelocity = Mathf.MoveTowards(yVelocity, -maxBoundFallSpeed,
                        boundFallDeceleration * Time.deltaTime);
                }
            }
            
            

            transform.localPosition += new Vector3(0f, yVelocity, -forwardVelocity) * Time.deltaTime;

            if (transform.localPosition.y < groundHeight)
            {
                yVelocity = 0;
                transform.localPosition = transform.localPosition.WithY(groundHeight);
                SetState(PlayerState.Ground);
            }
        }

        private void HandleCollision(Collision collision)
        {
            // if (playerState == PlayerState.Ground)
            // {
            //     rbdy.velocity -= collision.impulse;
            //     return;
            // }
            
            var hit = collision.contacts[0];
            if (playerState == PlayerState.Air && collision.contacts[0].normal.y > 0f)
            {
                
                SetState(PlayerState.Ground);
                // Debug.Log("impulse: " + collision.impulse);
                
                // Vector3 newDir = FloatExtensions.ChangeToSurface(rbdy.velocity.normalized, hit.normal);
                // if (playerInput.bound.Pressed && rbdy.velocity.magnitude >= minBounceSpeed)
                // {
                //     rbdy.velocity -= collision.contacts[0].impulse;
                //     rbdy.velocity = Vector3.Reflect(rbdy.velocity, hit.normal) * bounceReflectMultiplier;
                //     Debug.Log(rbdy.velocity);
                //     JumpStats bounceJumpStats = JumpStats.FromInitialVelAndMultiplier(rbdy.velocity.magnitude, Physics.gravity.magnitude, boundHeightMultiplier);
                //     jumpHoldForce = bounceJumpStats.HoldForce;
                //     jumpHoldTimer = bounceJumpStats.Time;
                //     canBound = false;
                //     // send bounce event
                //     OnBounce?.Invoke();
                // }
                // else
                // {
                //     var oldVelocity = rbdy.velocity;
                //     rbdy.velocity = rbdy.velocity.WithDirection(newDir.normalized);
                //
                //     // if ground is smooth enough, keep all speed
                //     // otherwise lose speed
                //     // if (newDir.magnitude + 0.02f < cosMinAngle)
                //     // {
                //     //     velocity *= newDir.magnitude;
                //     //     Debug.Log($"Lost momentum (hard hit): {velocity.magnitude - oldVelocity.magnitude} from change: {newDir.magnitude}");
                //     // }
                // }
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
