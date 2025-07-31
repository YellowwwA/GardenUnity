using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwitch : MonoBehaviour
{
    public GameObject playerController;      // 캐릭터 이동 스크립트가 붙은 오브젝트
    public Camera topDownCamera;             // 배치 모드용 탑다운 카메라
    public Camera playerCamera;              // 플레이어 시점 카메라
    public GameObject placementUI;           // 배치 모드 UI

    private bool isInPlacementMode = false;

    void Start()
    {
        // 커서 항상 보이게 설정
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 버튼에서 호출할 함수
    public void OnModeSwitchButtonClicked()
    {
        isInPlacementMode = !isInPlacementMode;
        SwitchMode(isInPlacementMode);
    }

    private void SwitchMode(bool placementMode)
    {
        // 카메라 전환
        playerCamera.enabled = !placementMode;
        topDownCamera.enabled = placementMode;

        // 캐릭터 이동 스크립트 활성/비활성
        var moveScript = playerController.GetComponent<PlayerMove>();
        if (moveScript != null)
            moveScript.enabled = !placementMode;

        // 배치 UI 활성화
        if (placementUI != null)
            placementUI.SetActive(placementMode);
    }
}
