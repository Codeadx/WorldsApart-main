using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMainMenu : MonoBehaviour
{
    CharacterState State;
    [SerializeField] GameObject animationPanel;
    [SerializeField] GameObject drawRopePanel;
    [SerializeField] GameObject grapplingDebugPanel;
    [SerializeField] GameObject level01Panel;
    [SerializeField] GameObject shipBuilderPanel;
    [SerializeField] GameObject inventoryGridPanel;
    [SerializeField] GameObject grapplingGun;
    [SerializeField] GameObject spawnShipFrame;
    [SerializeField] Button Animation;
    [SerializeField] Button DrawRope;
    [SerializeField] Button GrapplingDebug;
    [SerializeField] Button Level01;
    [SerializeField] Button ShipBuilder;
    [SerializeField] Button SpawnShip;
    [SerializeField] MeshCombiner meshCombiner;
    [SerializeField] public bool LockCursor = true;

    void Start() {
        Animation.onClick.AddListener(LoadAnimation);
        DrawRope.onClick.AddListener(LoadDrawRope);
        GrapplingDebug.onClick.AddListener(LoadGrapplingDebug);
        Level01.onClick.AddListener(LoadLevel01);
        ShipBuilder.onClick.AddListener(LoadShipBuilder);
        SpawnShip.onClick.AddListener(SpawnShipFrame);
    }

    public void ReceiveCharacterState(CharacterState CurrentCharacterState) {
        State = CurrentCharacterState;
        TransitionToState(State);
    }

    public void TransitionToState(CharacterState newState) {
        CharacterState tmpInitialState = State;
        State = newState;
    }

    void Update() {
        switch (State) {
            case CharacterState.Default:
                if (LockCursor == true) {
                    if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            break;

            case CharacterState.Interacting:
                LockCursor = false;
                Cursor.lockState = CursorLockMode.None;
            break;
        }
                

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            inventoryGridPanel.SetActive(!inventoryGridPanel.activeSelf);
        }
    }

    private void LoadAnimation() {
        ScenesManager.Instance.LoadScene(ScenesManager.Scene.Animation);
    }
    private void LoadDrawRope() {
        ScenesManager.Instance.LoadScene(ScenesManager.Scene.DrawRope);
    }
    private void LoadGrapplingDebug() {
        ScenesManager.Instance.LoadScene(ScenesManager.Scene.GrapplingDebug);
    }
    private void LoadLevel01() {
        ScenesManager.Instance.LoadScene(ScenesManager.Scene.Level01);
    }
    private void LoadShipBuilder() {
        ScenesManager.Instance.LoadScene(ScenesManager.Scene.ShipBuilder);
    }

    private void SpawnShipFrame() {
        meshCombiner.StartCoroutine(meshCombiner.CombineMeshes(1f));
    }
}
