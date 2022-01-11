using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] float atk = 10f;

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Impulse);
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            other.GetComponent<EnemyBehaviour>().Damage(atk, true);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }
        
    }
}
