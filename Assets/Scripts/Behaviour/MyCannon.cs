using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MyCannon : MonoBehaviour
{
    public GameObject Player;
    [SerializeField] Transform Enemy;

    [Header("Cannon Firing")]
    public GameObject cannonBall;
    Rigidbody cannonballRb;
    public Transform shotPos;
    public float firePower;
    public int powerMulti;
    public float timeToShootInitial = 2.5f;
    public float timeToShoot;

    [Header("Misc")]
    public Vector3 CannonBottom;
    public Transform CannonReleasePoint;
    
    public Vector3 BottomAnchorPoint {
        get {
            return transform.position + transform.TransformVector(CannonBottom);
        }
    }
    // Start is called before the first frame update
    void Start() {
        firePower *= powerMulti;
    }

    // Update is called once per frame
    void Update() {
        Enemy.LookAt(new Vector3(Player.transform.position.x, 1, Player.transform.position.z));
        
        timeToShoot -= Time.deltaTime;

        if (timeToShoot < 0)
        {
            FireCannon();
            timeToShoot = timeToShootInitial;
        }
    }

    public void FireCannon() {
        shotPos.rotation = transform.rotation; 
        GameObject cannonBallClone = Instantiate(cannonBall, shotPos.position, shotPos.rotation) as GameObject;
        cannonballRb = cannonBallClone.GetComponent<Rigidbody>();
        cannonballRb.AddForce(transform.forward * firePower);
    }
}
