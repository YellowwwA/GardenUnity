using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwitch : MonoBehaviour
{
    public GameObject playerController;      // 캐릭터 이동을 담당하는 오브젝트
    public Camera topDownCamera;             // 탑다운 카메라
    public Camera playerCamera;              // 기존 캐릭터 카메라
    public GameObject placementUI;           // 배치 모드용 UI (있다면)

    private bool isInPlacementMode = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInPlacementMode = !isInPlacementMode;
            SwitchMode(isInPlacementMode);
        }
    }

    void SwitchMode(bool placementMode)
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //playerController.SetActive(!placementMode);      // 이동 비활성화
        playerController.GetComponent<PlayerMove>().enabled = !placementMode;
        playerCamera.enabled = !placementMode;           // 캐릭터 시점 OFF
        topDownCamera.enabled = placementMode;           // 탑다운 시점 ON

        if (placementUI != null)
        {
            playerController.GetComponent<PlayerMove>().enabled = placementMode;
            placementUI.SetActive(placementMode);        // 배치 UI on/off
        }
    }
}
