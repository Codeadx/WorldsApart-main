using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField]GameObject bulletClone;
    public Vector3 hitPoint;
    Rigidbody bulletRb;
    // Start is called before the first frame update
    void Start() {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.AddForce((hitPoint - transform.position).normalized, ForceMode.Impulse);
    }

    void Update() {

    }

    void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Enemy") {
            GameManager.gameManager._enemyHealth.DmgUnit(6);
            Destroy(bulletClone);
        } else {
            Destroy(bulletClone);
        }
    }
}
