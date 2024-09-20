using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonBall : MonoBehaviour
{
    public GameObject cannonBallClone;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }
    
    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player") {
            GameManager.gameManager._playerHealth.DmgUnit(20);
            Destroy(cannonBallClone);
        } else {
            Destroy(cannonBallClone);   
        }
    }
}
