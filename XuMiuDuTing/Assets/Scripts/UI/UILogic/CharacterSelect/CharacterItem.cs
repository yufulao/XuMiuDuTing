using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterItem : MonoBehaviour
{
    [SerializeField] private Button btnChange;
    [SerializeField] private Button btnRemove;
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private GameObject objInTeamMask;

    private string _characterName;
    private UnityAction<string> _btnOnClickChange;
    private UnityAction _btnOnClickRemove;
    

    private void Start()
    {
        btnChange.onClick.AddListener(BtnOnClickChange);
        btnRemove.onClick.AddListener(BtnOnClickRemove);
    }

    /// <summary>
    /// 刷新item
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="btnChangeActive"></param>
    /// <param name="btnRemoveActive"></param>
    /// <param name="objInTeamMaskActive"></param>
    public void Refresh(string characterName,bool btnChangeActive,bool btnRemoveActive,bool objInTeamMaskActive)
    {
        _characterName = characterName;
        imageCharacterPortrait.sprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgCharacter[_characterName].portraitCharacterSelectPath);
        btnChange.gameObject.SetActive(btnChangeActive);
        btnRemove.gameObject.SetActive(btnRemoveActive);
        objInTeamMask.gameObject.SetActive(objInTeamMaskActive);
    }

    /// <summary>
    /// 设置btnChange点击回调
    /// </summary>
    /// <param name="func"></param>
    public void SetBtnOnClickChange(UnityAction<string> func)
    {
        _btnOnClickChange = func;
    }
    
    /// <summary>
    /// 设置btnRemove点击回调
    /// </summary>
    /// <param name="func"></param>
    public void SetBtnOnClickRemove(UnityAction func)
    {
        _btnOnClickRemove = func;
    }

    /// <summary>
    /// btnChange点击时
    /// </summary>
    private void BtnOnClickChange()
    {
        _btnOnClickChange?.Invoke(_characterName);
    }
    
    /// <summary>
    /// btnRemove点击时
    /// </summary>
    private void BtnOnClickRemove()
    {
        _btnOnClickRemove?.Invoke();
    }
}
