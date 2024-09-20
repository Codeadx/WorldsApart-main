using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using System.Linq;

    public class MyPlayer : MonoBehaviour
    {
        CharacterState State;
        CharacterCamera Camera;
        public Transform CameraFollowPoint;
        public Transform ShipBuildingFollowPoint;
        public MyCharacterController Character;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Awake() {
            Camera = FindObjectOfType<CharacterCamera>();
            Camera.SetFollowTransform(CameraFollowPoint);
            //Ignore the character's collider(s) for camera obstruction checks
            Camera.IgnoredColliders.Clear();
            Camera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        public void ReceiveCharacterState(CharacterState CurrentCharacterState) {
            State = CurrentCharacterState;
            TransitionToState(State);
        }

        public void TransitionToState(CharacterState newState) {
            CharacterState tmpInitialState = State;
            OnStateExit(tmpInitialState, newState);
            State = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        public void OnStateEnter(CharacterState state, CharacterState fromState) {
            switch (state)
            {
                case CharacterState.Interacting:
                    Camera = FindObjectOfType<CharacterCamera>();
                    Camera.SetFollowTransform(ShipBuildingFollowPoint);
                    //Ignore the character's collider(s) for camera obstruction checks
                    Camera.IgnoredColliders.Clear();
                    Camera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
                    Cursor.lockState = CursorLockMode.None;
                break;
            }
        }

        public void OnStateExit(CharacterState state, CharacterState toState) {
            switch (state)
            {
                case CharacterState.Default:
                break;
            }
        }

        private void Update() {
            HandleCharacterInput();
        }

        private void LateUpdate() {
            HandleCameraInput();
        }

        private void HandleCameraInput() {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            Camera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
        }

        private void HandleCharacterInput() {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = Camera.Transform.rotation;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.JumpUp = Input.GetKeyUp(KeyCode.Space);
            characterInputs.JumpHeld = Input.GetKey(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.LeftControl);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.LeftControl);
            characterInputs.CrouchHeld = Input.GetKey(KeyCode.LeftControl);
            characterInputs.MouseDown = Input.GetMouseButton(0);
            characterInputs.MouseUp = Input.GetMouseButtonUp(0);
            characterInputs.Sprint = Input.GetKey(KeyCode.LeftShift);
            characterInputs.Interaction = Input.GetKeyDown(KeyCode.E);
            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
    }