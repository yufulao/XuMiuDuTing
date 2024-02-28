using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TeamItem : MonoBehaviour
{
    [SerializeField] private Button btnEdit;
    [SerializeField] private Image portrait;

    private string _characterName;
    private int _teammateIndex;
    private UnityAction<string,int> _btnOnClickEdit;

    private void Start()
    {
        btnEdit.onClick.AddListener(BtnOnClickEdit);
    }

    private void OnDestroy()
    {
        btnEdit.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 刷新item
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="teammateIndex"></param>
    public void Refresh(string characterName,int teammateIndex)
    {
        _characterName = characterName;
        _teammateIndex = teammateIndex;
        var portraitPath = string.IsNullOrEmpty(characterName)
            ? ConfigManager.Instance.cfgGlobal["NullPortraitTeamEditPath"].value
            : ConfigManager.Instance.cfgCharacter[_characterName].portraitTeamEditPath;
        portrait.sprite = AssetManager.Instance.LoadAsset<Sprite>(portraitPath);
    }

    /// <summary>
    /// 设置btnEdit点击回调
    /// </summary>
    /// <param name="func"></param>
    public void SetBtnOnClickEdit(UnityAction<string,int> func)
    {
        _btnOnClickEdit = func;
    }

    /// <summary>
    /// btnEdit点击时
    /// </summary>
    private void BtnOnClickEdit()
    {
        _btnOnClickEdit?.Invoke(_characterName,_teammateIndex);
    }
}