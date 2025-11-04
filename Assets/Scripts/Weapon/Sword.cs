using UnityEngine;

namespace dutpekmezi
{
    public class Sword : WeaponBase
    {
        [Header("Sword Settings")]
        public float moveSpeed = 5f; // transition speed to the opposite orbit side

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

                targetPosition = CharacterSystem.Instance.GetCharacterTransform().position + new Vector3(
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
            float z = transform.position.z;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = transform.position + new Vector3(0f, 0f, z);

            // Make sure the front points toward the movement direction

            Vector2 dir = (targetPosition - transform.position).normalized;
            if (dir != Vector2.zero)
                transform.up = dir;
            

            // Check if we've reached the destination
            if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
            {
                transform.position = targetPosition;
                transform.position = transform.position + new Vector3(0f, 0f, z);

                currentAngle = targetAngle;
                isMoving = false;

                SetRotate(true);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.TakeDamage(weaponData.AttackDamage);
            }
        }
    }
}
