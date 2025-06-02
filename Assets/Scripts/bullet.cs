using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public bool isEnemy { get; set; } = false;
    public float AttackForce { get; set; } = 50;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var model = collision.gameObject.GetComponent<Model>();
        bool isEnemyCollsion = collision.gameObject.GetComponent<Unit>().IsEnemy;
        if (model != null && (isEnemy!=isEnemyCollsion))
        {
            model.setDamage(AttackForce);
            Destroy(gameObject);
        }
    }
}
