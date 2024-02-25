using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public static class Utils
{
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animationName"></param>
    /// <returns></returns>
    public static IEnumerator PlayAnimation(Animator animator, string animationName)
    {
        if (animator != null && animationName != null && animationName != "")
        {
            animator.Play(animationName, 0, 0f);
            yield return new WaitForSeconds(GetAnimatorLength(animator, animationName));
        }
    }

    /// <summary>
    /// 添加eventTrigger的事件
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="eventID"></param>
    /// <param name="callback"></param>
    public static void AddTriggersListener(EventTrigger trigger, EventTriggerType eventID, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        if (trigger.triggers.Count == 0)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventID;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 强制更新ui的layout
    /// </summary>
    /// <param name="rootTransform"></param>
    public static void ForceUpdateContentSizeFilter(Transform rootTransform)
    {
        foreach (ContentSizeFitter child in rootTransform.GetComponentsInChildren<ContentSizeFitter>(true))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(child.GetComponent<RectTransform>());
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootTransform.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 获取动画时长
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static float GetAnimatorLength(Animator animator, string name)
    {
        float length = 0;

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(name))
            {
                length = clip.length;
                break;
            }
        }

        return length;
    }

    /// <summary>
    /// 飘字效果
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="originalPosition"></param>
    public static Sequence TextFly(Graphic graphic, Vector3 originalPosition)
    {
        Transform transform = graphic.transform;
        Color originalColor = graphic.color;

        transform.position = originalPosition;
        //Debug.Log(rectTransform.localPosition);
        originalColor.a = 0;
        graphic.color = originalColor;

        Sequence textSequence = DOTween.Sequence().SetAutoKill(true);
        textSequence.Append(transform.DOMoveY(transform.position.y + 50, 0.5f));
        textSequence.Join(graphic.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 1), 0.5f));
        textSequence.AppendInterval(0.5f);
        textSequence.Append(transform.DOMoveY(transform.position.y + 50, 0.5f));
        textSequence.Join(graphic.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0), 0.5f));
        return textSequence;
    }

    /// <summary>
    /// 在场景中生成文字
    /// </summary>
    /// <param name="text">文字内容</param>
    /// <param name="parent">文字父物体</param>
    /// <param name="localPosition">文字相对父物体的偏移</param>
    /// <param name="localScale">文字缩放</param>
    /// <param name="fontSize">文字大小</param>
    /// <param name="color">文字颜色</param>
    /// <param name="textAnchor">文字锚点</param>
    /// <param name="textAlignment">文字对齐方式</param>
    /// <param name="sortingOrder">文字显示图层</param>
    /// <returns></returns>
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), Vector3 localScale = default, int fontSize = 40, Color? color = null,
        TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 0)
    {
        if (color == null) color = Color.white;
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = (Color) color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }


    /// <summary>
    /// 根据名称找到第一个组件（可以找到未激活的物体）
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static T FindTByName<T>(string str) where T : Component
    {
        var all = Resources.FindObjectsOfTypeAll<T>();
        foreach (T item in all)
        {
            if (item.gameObject.name == str)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// 反转字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Reverse(string str)
    {
      char[] arr = str.ToCharArray();
      Array.Reverse(arr);
      return new string(arr);
    }
}