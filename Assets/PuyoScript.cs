using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoScript : MonoBehaviour
{
    private int dropSet = 1;
    private int preDropSet = 0;
    private bool Falling = true;
    private bool Rotating = true;


    void Start() {
    }

    void Update() {
        transform.Translate(0, -0.02f, 0);

        if (Input.GetKeyDown(KeyCode.X)) {
            if (Rotating == true) {
                transform.Rotate (0, 0, 90);
            }
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            if (transform.position.x < -9.5) {
                if (Falling == true) {
                    transform.Translate(-1, 0, 0);
                } 
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (transform.position.x > -14.5); {
                if (Falling == true) {
                    transform.Translate(1, 0, 0);
                }
            }
        }

        void OnTriggerEnter2D (Collider2D collider) {
            Falling = false;
            Rotating = false;
        }
    }
}
