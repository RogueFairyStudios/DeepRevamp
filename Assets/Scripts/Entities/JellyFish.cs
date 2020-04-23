﻿using UnityEngine;
using System.Collections;

namespace DEEP.Entities
{
    public class JellyFish : EntityBase
    {

        private Vector3 initialPosition;
        private Vector3 targetPosition;

        [SerializeField] private float minMovementDelay = 5.0f;
        [SerializeField] private float maxMovementDelay = 10.0f;

        [SerializeField] private Vector3 movementBoxSize = new Vector3();

        [SerializeField] private float movementVelocity = 2.0f;
        [SerializeField] private float rotationVelocity = 90.0f;

        Animator _animator;

        protected void Start()
        {

            base.Start();

            // Initializes variables.
            initialPosition = transform.position;
            targetPosition = initialPosition;
            
            // Gets the aniamtor component.
            _animator = GetComponentInChildren<Animator>();

            // Starts the movement cycle.
            StartCoroutine(WaitForDelay());

        }

        private IEnumerator WaitForDelay()
        {

            // Waits for a random time inside the allowed range.
            float delay = Random.Range(minMovementDelay, maxMovementDelay);
            float time = 0.0f;

            while(time < delay)
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Start movement.
            StartCoroutine(MoveTo());

        }

        private IEnumerator MoveTo()
        {

            // Gets a random position.
            targetPosition = initialPosition + new Vector3(Random.Range(-movementBoxSize.x, movementBoxSize.x) / 2.0f,
                                                              Random.Range(-movementBoxSize.y, movementBoxSize.y) / 2.0f,
                                                              Random.Range(-movementBoxSize.z, movementBoxSize.z) / 2.0f);

            // Rotates the body.
            Quaternion rotate = Quaternion.LookRotation((targetPosition - transform.position).normalized);
            while(Quaternion.Angle(transform.rotation, rotate) > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.fixedDeltaTime * Mathf.Deg2Rad * rotationVelocity);
                yield return new WaitForFixedUpdate();
            }

            // Clamps the rotation.
            transform.LookAt(targetPosition);

            // Swims.
            _animator.SetBool("Swim", true);

            while(Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                // Moves
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, // The code inside Mathf.Clamp is used to smooth the movement at the end.
                                                         movementVelocity * Time.fixedDeltaTime * Mathf.Clamp(Vector3.Distance(transform.position, targetPosition), 0.05f, 1.0f)); 
                yield return new WaitForFixedUpdate();
            }

            // Stops swimming.
            _animator.SetBool("Swim", false);
            
            // Restarts the cycle.
            StartCoroutine(WaitForDelay());  

        }

        private void OnDrawGizmosSelected()
        {
            // Draws the random movement box.
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(initialPosition, movementBoxSize);

            // Draws a line to the destination.
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, targetPosition);
        }

    }

}
