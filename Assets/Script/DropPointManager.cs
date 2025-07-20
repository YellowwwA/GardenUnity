using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPointManager : MonoBehaviour
{
    public static DropPointManager Instance;

    public Transform[] dropPoints;
    public float validDropRadius = 1.5f;

    private HashSet<Transform> usedPoints = new HashSet<Transform>();

    private void Awake()
    {
        Instance = this;
    }

    public bool IsValidDropPosition(Vector3 dropPos, out Vector3 nearestPoint)
    {
        foreach (Transform point in dropPoints)
        {
            if (usedPoints.Contains(point))
                continue;

            if (Vector3.Distance(dropPos, point.position) <= validDropRadius)
            {
                nearestPoint = point.position;
                usedPoints.Add(point); // 💡 해당 포인트는 이제 사용된 것으로 간주
                return true;
            }
        }

        nearestPoint = Vector3.zero;
        return false;
    }

    // 선택 사항: 나중에 포인트를 비울 수 있도록 기능 추가
    public void ReleaseDropPoint(Vector3 position)
    {
        foreach (Transform point in dropPoints)
        {
            if (point.position == position)
            {
                usedPoints.Remove(point);
                break;
            }
        }
    }

    public void ReserveDropPoint(int index)
    {
        if (index >= 0 && index < dropPoints.Length)
        {
            usedPoints.Add(dropPoints[index]);
        }
    }

    public int GetDropPointIndex(Vector3 position)
    {
        for (int i = 0; i < dropPoints.Length; i++)
        {
            if (dropPoints[i].position == position)
                return i;
        }
        return -1;
    }
}
