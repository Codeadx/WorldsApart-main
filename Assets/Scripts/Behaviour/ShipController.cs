using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour, IMoverController
{
    public PhysicsMover Mover;
    private Transform _transform;
    [SerializeField] float forwardSpeed = 9.81f, strafeSpeed = 9.81f;
    [SerializeField] float forwardAcceleration = 0.2f;
    [SerializeField] float strafeAcceleration = 0.2f;
    [SerializeField] float activeForwardSpeed, activeStrafeSpeed;
    [SerializeField] float turnSpeed = 100f;
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";
    float yaw;
    float pitch;
    float roll;

    void Start() {
        _transform = this.transform;
        Mover.MoverController = this;
    }

    void LateUpdate() {
        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, 1 * forwardSpeed, forwardAcceleration * Time.deltaTime);
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw(HorizontalInput) * strafeSpeed, strafeAcceleration * Time.deltaTime);
        yaw = turnSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        pitch = turnSpeed * Time.deltaTime * -Input.GetAxis("Vertical");
        roll = turnSpeed * Time.deltaTime * Input.GetAxis("Roll");
    }

    public void UpdateMovement(out Vector3 position, out Quaternion rotation, float deltaTime) {
        position = _transform.position + (transform.forward * activeForwardSpeed * Time.deltaTime);
        rotation = _transform.rotation * Quaternion.Euler(pitch, yaw, roll);
    }

    // void OnCollisionEnter(Collision collision) {
    //     MyCharacterController myCharacterController = collision.transform.GetComponent<MyCharacterController>();
    //     if (myCharacterController.CurrentCharacterState.Interacting) {
    //         Move();
    //     }
    // }
}
