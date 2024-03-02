using UnityEngine;
using System.Collections;
using Yu;

public class UIFollowObj : MonoBehaviour
{
    public Transform objFollowed;//3D物体（人物）
    public RectTransform rectFollower;//UI元素（如：血条等）
    public Vector2 offset;//偏移量

    private void Update()
    {
        if (!objFollowed)
        {
            return;
        }
        Vector2 screenPos = CameraManager.Instance.GetObjCamera().WorldToScreenPoint(objFollowed.transform.position);
        rectFollower.position = screenPos + offset;
    }
}
