using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperHit : MonoBehaviour
{

    public float AttackForce { get; set; } = 5000;
    public float Speed = 0.05f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var model = collision.gameObject.GetComponent<Model>();
        if (model != null)
        {
            model.setDamage(AttackForce);
        }
    }
    private void Update()
    {
        gameObject.transform.localScale += new Vector3(Speed, Speed, 0);
    }
}
