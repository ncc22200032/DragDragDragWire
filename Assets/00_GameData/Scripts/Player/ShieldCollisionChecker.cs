using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollisionChecker : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IEnemyDamageable>().Die();
            gameObject.SetActive(false);
        }
    }
}
