using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGun : MonoBehaviour
{
    public float range = 100f;
    public GameObject bulletClone;
    public Transform gunTip;
    public RaycastHit shootHit;
    public float timeToShoot = 2f;
    BulletScript bulletScript;
    // Start is called before the first frame update
    void Start() {
        bulletScript = bulletClone.GetComponent<BulletScript>();
    }

    // Update is called once per frame
    void Update() {
        timeToShoot -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1")) {
            if (timeToShoot < 0) {
                Shoot();
                timeToShoot = 2f;
            }
        }
    }

    void Shoot() {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out shootHit, range)) {
            bulletScript.hitPoint = shootHit.point;
            Instantiate(bulletClone, gunTip.position, Quaternion.identity);
        }
    }
}
