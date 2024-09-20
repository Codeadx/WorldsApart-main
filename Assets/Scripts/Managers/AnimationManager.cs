using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator Animator;
    private GrapplingGun grapplingGun;
    private MyCharacterController Character;
    private KinematicCharacterMotor Motor;
    public float animationSmoothing;

    [Header("Animation & IK")]
    Vector3 screenPosition;
    Vector3 lookAtPosition; 

    [Header("Gliding")]
    public bool _gliding;

    [Header("Grappling")]
    Vector3 tangentialVelocity;
    Vector3 direction;

    [Header("Runstage")]
    public Vector3 lastPosition;
    float angleCounter;
    [SerializeField] float radius;
    [SerializeField] float stepDistance;
    [SerializeField] Transform strideWheel;

    Quaternion currentRotation;

    public float elapsedTime = 0;
    public float raycastLength;

    // Start is called before the first frame update
    void Awake() {
        Animator = GetComponent<Animator>();
        grapplingGun = GetComponentInParent<GrapplingGun>();
        Character = GetComponentInParent<MyCharacterController>();
        Motor = GetComponentInParent<KinematicCharacterMotor>();
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    
    void Update() {
        //Update Head IK lookAtPosition
        screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.farClipPlane;
        lookAtPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        currentRotation = transform.rotation;
        HandleInputs();
        HandleRunStage();
    }

    #region HandleInputs
    void HandleInputs() {
        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround) {
            Animator.SetBool("_isGrounded", true);
            Animator.SetBool("_isFalling", false);
        } else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround) {
            Animator.SetBool("_isGrounded", false);
            Animator.SetBool("_isFalling", true);
        }
    }
    #endregion

    #region HandleRunStage
    void HandleRunStage() {
        lastPosition = new Vector3(lastPosition.x, 0F, lastPosition.z);
        Vector3 currPosition = new Vector3 (transform.position.x, 0F, transform.position.z);

        float dist = Vector3.Distance(lastPosition, currPosition);
        float turnAngle = (dist / (2 * Mathf.PI * radius)) * 360F;
 
        strideWheel.Rotate(new Vector3(0f, -turnAngle, 0f));
            
        angleCounter += turnAngle;

        if (angleCounter > stepDistance) {
            angleCounter = 0;
        }
    
        Animator.SetFloat("runstage", (angleCounter/stepDistance));
        if (Animator.GetFloat("runstage") > 1f) {
            Animator.SetFloat("runstage", 0);
        } 
        lastPosition = currPosition;
    }
    #endregion

    void OnAnimatorIK() {
        //Head IK
        Animator.SetLookAtWeight(0.5f);  
        Animator.SetLookAtPosition(lookAtPosition);

        if (grapplingGun._tethered == true) {
            Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            Animator.SetIKPosition(AvatarIKGoal.RightHand, grapplingGun.hookHitPoint);
            Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            Animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(grapplingGun.hookHitPoint));
        }

        #region Hand Sliding
        Vector3 leftArmSpacing = new Vector3(-0.25f, 0, 0);
        Vector3 rightArmSpacing = new Vector3(0.25f, 0, 0);

        Vector3 proximity = new Vector3(-0.05f, -0.05f, -0.05f);

        Ray ray = new Ray(Animator.GetBoneTransform(HumanBodyBones.RightShoulder).position + transform.TransformVector(rightArmSpacing), Camera.main.transform.forward * raycastLength);
        RaycastHit rightHit;
        if (Physics.Raycast(ray, out rightHit, raycastLength)) {
            //Debug.DrawLine(Animator.GetBoneTransform(HumanBodyBones.RightShoulder).position + transform.TransformVector(rightArmSpacing), rightHit.point, Color.red);
            //Set animator float
            Animator.SetFloat("RightHandWeight", 1, 0.1f, Time.deltaTime * animationSmoothing);

            Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, Animator.GetFloat("RightHandWeight"));
            Animator.SetIKPosition(AvatarIKGoal.RightHand, rightHit.point + transform.TransformVector(proximity));
            
            Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, Animator.GetFloat("RightHandWeight"));

            Animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(-rightHit.normal) * Quaternion.Euler(-90, 0, 0));
        } else if (!Physics.Raycast(ray, out rightHit, raycastLength)) {
            //else return to 0
            Animator.SetFloat("RightHandWeight", 0, 0.1f, Time.deltaTime * animationSmoothing);
        }
        
        Ray ray2 = new Ray(Animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position + transform.TransformVector(leftArmSpacing), Camera.main.transform.forward * raycastLength);
        RaycastHit leftHit;
        if (Physics.Raycast(ray2, out leftHit, raycastLength)) {
            //Debug.DrawLine(Animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position + transform.TransformVector(leftArmSpacing), leftHit.point, Color.green);
            //Set lookAt float
            Animator.SetFloat("LeftHandWeight", 1, 0.1f, Time.deltaTime * animationSmoothing);
           
            Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, Animator.GetFloat("LeftHandWeight"));
            Animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHit.point + transform.TransformVector(proximity));

            Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, Animator.GetFloat("LeftHandWeight"));

            Animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(-leftHit.normal) * Quaternion.Euler(-90, 0, 0));
        } else if (!Physics.Raycast(ray, out leftHit, raycastLength)) {
            //else return to 0
            Animator.SetFloat("LeftHandWeight", 0, 0.1f, Time.deltaTime * animationSmoothing);
        }

        // Ray ray3 = new Ray(Animator.GetBoneTransform(HumanBodyBones.RightShoulder).position + -armSpacing, Camera.main.transform.right * raycastLength * 2);
        // RaycastHit rightHit2;
        // if (Physics.Raycast(ray3, out rightHit2, raycastLength * 2)) {
        //     Animator.SetFloat("HandWeight", 1, 0.1f, Time.deltaTime * 0.3f);
        //     Debug.DrawLine(Animator.GetBoneTransform(HumanBodyBones.RightShoulder).position + -armSpacing, rightHit2.point, Color.green);
        //     Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, );
        //     Animator.SetIKPosition(AvatarIKGoal.RightHand, rightHit2.point * proximity);
        //     Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, );
        //     Animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(Vector3.Project(rightHit2.point, Vector3.up)));
        // }
        #endregion
    }
}
