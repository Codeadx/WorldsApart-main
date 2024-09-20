using KinematicCharacterController;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

    public enum CharacterState {
        Default,
        Tethered,
        Gliding,
        Interacting,
    }

    public enum GrapplingState {
        Attaching,
        Tethered,
        Detatched
    }
    
    public struct PlayerCharacterInputs {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool JumpHeld;
        public bool JumpUp;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool CrouchHeld;
        public bool MouseDown;
        public bool MouseUp;
        public bool Sprint;
        public bool Interaction;
    }

    public class MyCharacterController : MonoBehaviour, ICharacterController
    {
        Animator Animator;
        AnimationManager AnimationManager;
        Camera Camera;
        GrapplingGun GrapplingGun;
        public KinematicCharacterMotor Motor;
        public MyPlayer Player;
        public UIMainMenu Menu;

        [Header("Air Movement")]  
        private float MaxAirMoveSpeed = 9.81f;
        private float AirAccelerationSpeed = 9.81f;
        private float Drag = 0.07f;
        private bool _gliding = false;

        [Header("Gliding")]
        [System.NonSerialized] public float facingVel;
        [System.NonSerialized] public float forwardVel;
        [System.NonSerialized] public float rightVel;
        [System.NonSerialized] public float elapsedTime = 0;
        Vector3 screenPosition;
        Vector3 lookAtPosition;   


        [Header("Grappling")]
        public float dotProduct;
        float centripetalForceMagnitude;
        public float _cumulativeRotationAngle = 0f;
        public bool _hasCompletedFullRotation = false;
        private bool _needsFullRotation = false;      // Flag to check if we need to complete a 360° rotation
        public float _remainingRotation = 0f;        // Remaining rotation to reach 360°
        public float _signedAngle;
        public float AlignmentDuration;
        public float RotationDuration;
        public float RotationStrength;
        public float TetheringDuration;
        float angle;
        Vector3 axis;
        float distance;
        float tetherLength;
        float reelingSpeed = 10f;  // Speed at which the player reels in or out
        public float maxTetherLength = 50f;
        public float minTetherLength = 9.81f;
        float reelingForce = 0f;


        [Header("Jumping")]
        private bool AllowJumpingWhenSliding = false;
        private float JumpSpeed = 13.08f;
        private float JumpScalableForwardSpeed = 0f;
        private float JumpPreGroundingGraceTime = 0f;
        private float JumpPostGroundingGraceTime = 0f;

        [Header("Misc")]
        private List<Collider> IgnoredColliders = new List<Collider>();
        public Vector3 Gravity = new Vector3(0, -19.62f, 0);
        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        [System.NonSerialized] public Vector3 _moveInputVector;
        [System.NonSerialized] public Vector3 _lookInputVector;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private bool _jumpRequested = false;
        private float _timeSinceLastAbleToJump = 0f;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private Vector3 _internalVelocityAdd = Vector3.zero;

        [Header("Stable Movement")]
        Vector3 targetMovementVelocity;
        [System.NonSerialized] public Vector3 smoothedLookInputDirection;
        [System.NonSerialized] public Vector3 smoothedMoveInputDirection;
        [System.NonSerialized] public float MaxStableMoveSpeed = 3.27f;
        [System.NonSerialized] public float MaxStableRunSpeed = 9.81f;
        [System.NonSerialized] public float CurrentMaxVelocity;
        [System.NonSerialized] public float StableMovementSharpness = 10f;
        [System.NonSerialized] public float OrientationSharpness = 10f;
        public bool Sprint = false;

        [Header("Animation")]
        Vector3 directionToGrapple;
        Vector3 previousPosition;
        Vector3 previousVelocity;
        Vector3 velocityAcceleration;
        public bool isGrounded;

        [Header("UI")]
        public TextMeshProUGUI currentVelocityTxt;
        public TextMeshProUGUI angVelTxt;
        public CharacterState CurrentCharacterState { get; set; }

        //Tethering vars
        private GrapplingState _internalGrapplingState;
        private GrapplingState _grapplingState
        {
            get
            {
                return _internalGrapplingState;
            }
            set
            {
                _internalGrapplingState = value;
                _attachingTimer = 0f;
            }
        }
        public float _attachingTimer = 0f;
        private float _angle;
        private Vector3 _axis;
        private float _angleToTarget;
        public Vector3 _axisToTarget;
        private Vector3 _forwardDirection;
        private Quaternion _combinedRotations;
        private Quaternion _deltaRotation;
        private Quaternion _previousRotation;
        Quaternion _upRotation;


        private void Awake() {   
            Camera = Camera.main;
            Motor.CharacterController = this;
            Animator = GetComponentInChildren<Animator>();
            AnimationManager = Motor.GetComponentInChildren<AnimationManager>();
            GrapplingGun = Motor.GetComponent<GrapplingGun>();
            Motor = GetComponent<KinematicCharacterMotor>();
            TransitionToState(CharacterState.Default);
        }

        public void SendCharacterState() {
            Player.ReceiveCharacterState(CurrentCharacterState);
            Menu.ReceiveCharacterState(CurrentCharacterState);
        }

        public void TransitionToState(CharacterState newState) {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {                
                case CharacterState.Default:
                    {
                        break;
                    }
                
                case CharacterState.Tethered:
                    {
                        _grapplingState = GrapplingState.Attaching;
                        Motor.SetGroundSolvingActivation(false);
                        break;
                    }

                case CharacterState.Interacting:
                    {
                        GrapplingGun.enabled = false;
                        break;
                    }
            }
        }

        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                break;
            }
        }
        
        void Update() {
            if(Motor.GroundingStatus.IsStableOnGround) {
                CurrentMaxVelocity = Sprint ? MaxStableRunSpeed : MaxStableMoveSpeed;
                if (Sprint == true) {
                    Animator.SetBool("_isSprinting", true);
                } else {
                    Animator.SetBool("_isSprinting", false);
                }
            }
        }

        public void SetInputs(ref PlayerCharacterInputs inputs) 
        {

            //Handle Grappling transitions
            if (inputs.MouseDown) 
            {
                if(GrapplingGun.hookHitPoint != null) 
                {
                    //Check if we are Grounded
                    CheckIfGrounded(transform.position, transform.rotation, Gravity.normalized, 0.5f);
                    if (!isGrounded) 
                    {
                        if(CurrentCharacterState == CharacterState.Default) 
                        {
                            TransitionToState(CharacterState.Tethered);
                        } 
                    }
                }
            } 

            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);
            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;

            if (cameraPlanarDirection.sqrMagnitude == 0f) {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);
            
            // Move and look inputs
            _moveInputVector = cameraPlanarRotation * moveInputVector;
            _lookInputVector = cameraPlanarDirection;

            switch (CurrentCharacterState) 
            {
                case CharacterState.Default:
                    // Jumping input
                    if (inputs.JumpDown) {
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested = true;
                    }

                    //Sprint input
                    if (Motor.GroundingStatus.IsStableOnGround && inputs.Sprint == true) {
                        Sprint = true;
                    } else { Sprint = false; }  

                    //Gliding input
                    if(!Motor.GroundingStatus.IsStableOnGround && inputs.JumpHeld) {
                        if(facingVel > 0f) {
                            elapsedTime += Time.deltaTime;
                            if (elapsedTime > 0.6f) {
                                TransitionToState(CharacterState.Gliding);
                            }
                        }
                    }
                break;

                case CharacterState.Gliding:
                    //Exit gliding
                    CheckIfGrounded(transform.position, transform.rotation, Gravity.normalized, 0.5f);
                    if(isGrounded || inputs.JumpUp || facingVel < 0f) {
                        TransitionToState(CharacterState.Default);
                        Animator.SetBool("_isGliding", false);
                    }
                break;
            }

            if(Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround) 
            {
                TransitionToState(CharacterState.Default);
            }

            if(Input.GetKeyDown(KeyCode.Q)) 
            {
                Time.timeScale = 0.5f;
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                Time.timeScale = 1f;
            }
            
            // if(Input.GetKeyDown(KeyCode.E)) {
            //     Ray ray = new Ray (Camera.transform.position, Camera.transform.forward);
            //     RaycastHit hit;
            //     if(Physics.Raycast(ray, out hit)) {
            //         if(hit.collider.CompareTag("Interactable")) {
            //             TransitionToState(CharacterState.Interacting);
            //             SendCharacterState();
            //         }
            //     }
            // }
        }

        public void BeforeCharacterUpdate(float deltaTime) {
            
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) 
        {  
            smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1f - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
            smoothedMoveInputDirection = Vector3.Slerp(Motor.CharacterForward, _moveInputVector, 1f - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            Vector3 currentVelocity = (transform.position - previousPosition) / deltaTime;
            angVelTxt.text = "Rotation:" + currentRotation.ToString();

            Quaternion worldRotation = transform.parent.rotation * transform.localRotation;

            switch (CurrentCharacterState)
            {
                // Default Rotation
                case CharacterState.Default:
                    {
                        if (Motor.GroundingStatus.IsStableOnGround) 
                        {
                            if(_moveInputVector.sqrMagnitude == 0) 
                            {
                                CheckIfGrounded(transform.position, transform.rotation, Gravity.normalized, 0.5f);

                                if (isGrounded == true) 
                                {
                                    Quaternion cleanRotation = Quaternion.LookRotation(smoothedLookInputDirection);
                                    currentRotation = Quaternion.Slerp(transform.rotation, new Quaternion(0, cleanRotation.y, 0, cleanRotation.w), 1f - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
                                }
                            } 
                            else if (_moveInputVector.sqrMagnitude > 0) 
                            {
                                Vector3 axis = Vector3.Cross(_moveInputVector, Vector3.up);
                                currentRotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(-velocityAcceleration.magnitude * 4f, axis) * Quaternion.LookRotation(currentVelocity), 1f - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
                            } 
                            // Reset additiveVelocity variables to 0 when grounded
                            angle = 0f;
                        }
                        break;
                    }

                
                // Grappling Rotation
                case CharacterState.Tethered:
                    {
                        Vector3 _upDirection = Vector3.Normalize(GrapplingGun.hookHitPoint - transform.position);
                        float _currentDistance;

                        Quaternion _forwardRotation = Quaternion.identity;

                        switch (_grapplingState)
                        {
                            case GrapplingState.Attaching:
                                    Quaternion _rotationDifference = Quaternion.Inverse(currentRotation) * _combinedRotations;
                                    _rotationDifference.ToAngleAxis(out _angleToTarget, out _axisToTarget);

                                    _signedAngle = Vector3.Dot(Motor.CharacterUp, _upDirection) > 0 ? _angleToTarget : -_angleToTarget;

                                    _remainingRotation = _signedAngle;  // Calculate remaining angle

                                    if (_needsFullRotation && !_hasCompletedFullRotation)
                                    {
                                        // Rotate by the remaining rotation first
                                        float rotationStep = _remainingRotation * (_attachingTimer / RotationDuration);  // Rotate smoothly by the remaining amount

                                        Quaternion additionalRotation = Quaternion.AngleAxis(rotationStep, _axisToTarget);

                                        // Apply the additional rotation
                                        currentRotation *= additionalRotation;

                                        // Subtract the applied rotation
                                        _remainingRotation -= rotationStep;

                                        // If remaining rotation is small enough, complete it
                                        if (_remainingRotation <= 5f)
                                        {
                                            _remainingRotation = 0f;
                                            _needsFullRotation = false;  // Reset flag once the 360 is complete
                                            _hasCompletedFullRotation = true;
                                        }

                                    }
                                    else if (_hasCompletedFullRotation)
                                    {
                                        // Rotate by the remaining rotation first
                                        float rotationStep = _remainingRotation * (_attachingTimer / RotationDuration);  // Rotate smoothly by the remaining amount

                                        Quaternion additionalRotation = Quaternion.AngleAxis(rotationStep, _axisToTarget);

                                        // Apply the additional rotation
                                        currentRotation *= Quaternion.Inverse(additionalRotation);

                                        // Subtract the applied rotation
                                        _remainingRotation -= rotationStep;

                                        // If remaining rotation is small enough, complete it
                                        if (_remainingRotation <= 5f)
                                        {
                                            _remainingRotation = 0f;
                                            _needsFullRotation = false;  // Reset flag once the 360 is complete
                                            _hasCompletedFullRotation = false;
                                        }

                                    }
                                break;
                            
                            case GrapplingState.Tethered:
                            dotProduct = Vector3.Dot(Motor.CharacterForward, currentVelocity.normalized);
                                _currentDistance = _upDirection.magnitude;

                                // Rotate character's Up vector
                                _upRotation = Quaternion.FromToRotation(Motor.CharacterUp, _upDirection);
                                // Rotate Forward vector
                                _forwardRotation = Quaternion.FromToRotation(Motor.CharacterForward, _lookInputVector);

                                // Combined rotations
                                _combinedRotations = _upRotation * _forwardRotation;

                                float t = Mathf.Clamp01(_attachingTimer / AlignmentDuration);
                                float easedT = 1 - (1 - t) * (1 - t);  // Quadratic ease-out

                                // Apply rotation
                                currentRotation = Quaternion.Slerp(currentRotation, _upRotation * currentRotation, easedT);

                                // Calculate delta
                                _deltaRotation = Quaternion.Inverse(_previousRotation) *  currentRotation;
                                _deltaRotation.ToAngleAxis(out _angle, out _axis);
                                break;
                                
                            case GrapplingState.Detatched:
                                Vector3 _angularDisplacement = _axis.normalized * _angle * Mathf.Deg2Rad;
                                Vector3 _angularSpeed = _deltaRotation * (_angularDisplacement / deltaTime);

                                // Respresent delta as a Quaternion
                                Quaternion _addedRotation = Quaternion.Euler(_angularSpeed * RotationStrength) * currentRotation;

                                // Apply delta to current
                                currentRotation = _addedRotation;
                                // Set flag to complete rotation when attaching
                                _needsFullRotation = true; 
                                break;
                        }
                        break;
                    }

                //Gliding Rotation
                case CharacterState.Gliding:
                {
                    if (!GrapplingGun._tethered) {
                        screenPosition = Input.mousePosition;
                        screenPosition.z = Camera.main.farClipPlane;
                        lookAtPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                        currentRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAtPosition - transform.position), 1f - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
                    }
                    break;
                }


                //Interacting Rotation
                case CharacterState.Interacting:
                {
                    RaycastHit closestHit1;
                    if (Motor.CharacterGroundSweep(transform.position, transform.rotation, -Motor.CharacterUp, 2f, out closestHit1)) {
                        if (closestHit1.transform != null) {
                            currentRotation = Quaternion.LookRotation(Motor.CharacterForward, closestHit1.normal);
                        }
                    }
                    break;
                }
            }
            _previousRotation = currentRotation;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
            facingVel = Vector3.Dot(transform.forward, currentVelocity);   
            currentVelocityTxt.text = "Velocity:" + currentVelocity.magnitude.ToString();

                                    if(Input.GetKey(KeyCode.X))
                        {
                            currentVelocity = Vector3.zero;
                            Gravity = Vector3.zero;
                        }

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        targetMovementVelocity = Vector3.zero;
                        if (Motor.GroundingStatus.IsStableOnGround && currentVelocity != Vector3.zero) 
                        {
                            Animator.SetBool("_isMoving", true);
                            // Reorient velocity on slope
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
                            
                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            targetMovementVelocity = reorientedInput * CurrentMaxVelocity;

                            // Smooth movement Velocity
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                            velocityAcceleration = targetMovementVelocity - currentVelocity;
                        } 
                        else 
                        {   
                            // Add move input
                            if (_moveInputVector.sqrMagnitude > 0f && Motor.GroundingStatus.IsStableOnGround) 
                            {
                                targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                                // Prevent climbing on un-stable slopes with air movement
                                if (Motor.GroundingStatus.FoundAnyGround) 
                                {
                                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                                }

                                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                                currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                            }

                            // Gravity
                            currentVelocity += Gravity * deltaTime;

                            // Drag
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }

                        //Handle Jumping
                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;

                        if (_jumpRequested) 
                        {
                            // See if we actually are allowed to jump
                            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)) 
                            {
                                // Calculate jump direction before ungrounding
                                Vector3 jumpDirection = Motor.CharacterUp;
                                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround) 
                                {
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                                }

                                Motor.ForceUnground(0.1f);

                                // Add to the return velocity and reset jump state
                                currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                            }    
                        }

                        // Take into account additive velocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f) 
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }
                    
                case CharacterState.Tethered:
                    {
                        if (GrapplingGun._tethered == true) 
                        {
                            tetherLength = GrapplingGun.tetherLength;
                            directionToGrapple = Vector3.Normalize(GrapplingGun.hookHitPoint - transform.position);
                            distance = Vector3.Distance(GrapplingGun.hookHitPoint, transform.position);
                            float speedTowardsGrapplePoint = Mathf.Round(Vector3.Dot(currentVelocity, directionToGrapple) * 100) / 100;
                            
                            // Calculate the centripetal force
                            centripetalForceMagnitude = (Motor.SimulatedCharacterMass * currentVelocity.sqrMagnitude) / distance;
                            Vector3 centripetalForce = directionToGrapple * centripetalForceMagnitude;

                            // Handle reeling input
                            if (Input.GetKey(KeyCode.LeftShift) && tetherLength > minTetherLength) 
                            {
                                currentVelocity += Vector3.Lerp(Motor.CharacterUp, directionToGrapple, 80f * Time.deltaTime);
                            } 
                            else if (Input.GetKey(KeyCode.LeftControl) && tetherLength < maxTetherLength) 
                            {
                                // Reel out: Increase tether length
                                tetherLength += reelingSpeed * Time.deltaTime;
                                reelingForce = -reelingSpeed * Motor.SimulatedCharacterMass; // Negative force
                            }

                            // Apply centripetal force if the distance exceeds the tether length
                            if (distance > tetherLength) {
                                currentVelocity += centripetalForce * Time.deltaTime / Motor.SimulatedCharacterMass;
                                // Prevent moving away from grapple point if velocity points away
                                if (speedTowardsGrapplePoint < 0) {
                                    currentVelocity -= speedTowardsGrapplePoint * directionToGrapple;
                                }
                            }

                            //Update animator parameters
                            Animator.SetBool("_isGrappling", true);
                            Animator.SetBool("_isGliding", false);
                            Animator.SetBool("_isFalling", false);
                            Animator.SetFloat("facingVel", facingVel);

                        } 

                        else if (GrapplingGun._tethered == false) 
                        {
                            Animator.SetBool("_isGrappling", false);
                        }

                        if(Input.GetKey(KeyCode.Space)) 
                        {
                            AddVelocity(directionToGrapple + Vector3.up * 1f);
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));

                        if (!Motor.GroundingStatus.IsStableOnGround) 
                        {
                            float facingGravity = Vector3.Dot(currentVelocity, Vector3.up);
                            Animator.SetFloat("facingGravity", facingGravity);
                        }

                        // Take into account additive velocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f) 
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }

                case CharacterState.Gliding: 
                    {
                        if (facingVel > 0f) 
                        {
                            Animator.SetBool("_isGliding", true);
                            forwardVel = transform.InverseTransformDirection(currentVelocity).z;
                            float liftFactor = 0.07f;
                            currentVelocity = Vector3.Lerp(currentVelocity, transform.forward * forwardVel, liftFactor * facingVel * forwardVel * deltaTime);
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        break;
                    }


                case CharacterState.Interacting:
                    currentVelocity = Vector3.zero;
                break;
            }

            previousVelocity = currentVelocity;
            previousPosition = transform.position;
        }

        public void AfterCharacterUpdate(float deltaTime) {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Handle jump-related values
                        {
                            // Handle jumping pre-ground grace period
                            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime) {
                                _jumpRequested = false;
                            }
                            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) {
                                // If we're on a ground surface, reset jumping values
                                if (!_jumpedThisFrame) {
                                    _jumpConsumed = false;
                                }
                                _timeSinceLastAbleToJump = 0f;
                            } else {
                                // Keep track of time since we were last able to jump (for grace period)
                                _timeSinceLastAbleToJump += deltaTime;
                            }
                        }
                        break;
                    }

                case CharacterState.Tethered:
                    {
                        switch (_grapplingState)
                        {

                            case GrapplingState.Attaching:
                                if(_attachingTimer >= TetheringDuration || _remainingRotation <= 5)
                                {
                                    _grapplingState = GrapplingState.Tethered;
                                }
                                 // Keep track of time since we started anchoring
                                _attachingTimer += deltaTime;
                                break;
                            
                            case GrapplingState.Tethered:
                                if(GrapplingGun._tethered == false)
                                {
                                    if(CurrentCharacterState == CharacterState.Tethered)
                                    {
                                        _grapplingState = GrapplingState.Detatched;

                                    }
                                }
                                break;

                            case GrapplingState.Detatched:
                                if(GrapplingGun._tethered == true)
                                {
                                    if(CurrentCharacterState == CharacterState.Tethered)
                                    {
                                        _grapplingState = GrapplingState.Attaching;
                                    }
                                }
                                break;
                        }
                        break;
                    }
            }
        }

        public bool CheckIfGrounded(Vector3 position, Quaternion rotation, Vector3 normal, float distance) {
            RaycastHit closestHit;
            if (Motor.CharacterGroundSweep(position, rotation, normal, distance, out closestHit)) {
                if (closestHit.transform != null) {
                    return isGrounded = true;
                }
            }
            return isGrounded = false;
        }

        void OnDrawGizmos() {   
            Gizmos.color = Color.yellow;
            GizmosExtensions.DrawWireArc(transform.position + new Vector3(0, 0.5f, 0), Vector3.forward, 360f - _signedAngle, 3f, 20f);
        }

        public bool IsColliderValidForCollisions(Collider coll) {
            if (IgnoredColliders.Contains(coll)) {
                return false;
            }
            return true;
        }

        public void OnLookAt(Collider hitCollider) {

        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {

        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
            // if (hitCollider != null && hitCollider.CompareTag("Interactable") && Input.GetKeyDown(KeyCode.E)) {
            //     TransitionToState(CharacterState.Interacting);
            // }
        }
        
        public void AddVelocity(Vector3 velocity) {
            _internalVelocityAdd += velocity;
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) {
        }

        public void PostGroundingUpdate(float deltaTime) {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider) {
        }
}