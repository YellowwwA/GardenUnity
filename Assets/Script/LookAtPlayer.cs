using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;  // Inspector에서 직접 할당하거나 태그로 찾아도 됨

    void Start()
    {
        // 플레이어를 태그로 자동 할당하고 싶다면:
//        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            Debug.Log(playerObj); // null이 아닌지 확인
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 targetPos = player.position;
            targetPos.y = transform.position.y;  // 높이(y축)는 무시

            transform.LookAt(targetPos);
        }
    }
}