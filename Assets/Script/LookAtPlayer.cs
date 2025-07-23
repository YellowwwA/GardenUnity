using System.Collections;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;

    void OnEnable()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("❌ Player 태그가 붙은 오브젝트를 찾지 못했습니다.");
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            //Debug.Log($"LookAt 실행 중: {gameObject.name}");
            Vector3 targetPos = player.position;
            targetPos.y = transform.position.y;  // y축 고정
            transform.LookAt(targetPos);
        }
    }
}
