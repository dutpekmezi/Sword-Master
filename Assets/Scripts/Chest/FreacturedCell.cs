using dutpekmezi;
using UnityEngine;

public class FreacturedCell : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            enemy.OnTakeDamageHandler(enemy, 1f);
        }
    }
}
