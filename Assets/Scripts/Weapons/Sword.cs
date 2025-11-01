using UnityEngine;

namespace dutpekmezi
{
    public class Sword : WeaponBase
    {
        [Header("Sword Settings")]
        public float moveSpeed = 5f; // Smooth transition speed to the opposite orbit side

        private bool isMoving = false;
        private float targetAngle;
        private Vector3 targetPosition;

        protected override void Ability()
        {
            if (Input.GetMouseButtonDown(0) && !isMoving)
            {
                // Determine the opposite point along the orbit
                targetAngle = currentAngle + 180f;
                if (targetAngle > 360f)
                    targetAngle -= 360f;

                targetPosition = player.position + new Vector3(
                    Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                    Mathf.Sin(targetAngle * Mathf.Deg2Rad),
                    0f
                ) * orbitRadius;

                isMoving = true;
            }

            if (isMoving)
            {
                MoveToOpposite();
            }
        }

        private void MoveToOpposite()
        {
            SetRotate(false);

            // Smoothly move toward the opposite side
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Make sure the front points toward the movement direction
            
            Vector3 dir = (targetPosition - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.up = dir;
            

            // Check if we've reached the destination
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                transform.position = targetPosition;
                currentAngle = targetAngle;
                clockwise = !clockwise;
                isMoving = false;

                SetRotate(true);
            }
        }
    }
}
