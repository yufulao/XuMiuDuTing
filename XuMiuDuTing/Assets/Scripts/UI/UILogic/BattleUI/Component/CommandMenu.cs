using System.Collections;
using System.Collections.Generic;
using Rabi;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Yu
{
    public class CommandMenu : MonoBehaviour
{
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private Image imageCharacterName;
    [SerializeField] private Image imageBpCurrent;
    [SerializeField] private Image imageBpPreview;
    [SerializeField] private TextMeshProUGUI textMp;
    [SerializeField] private Slider sliderMp;
    public CommandMenuButton btnAttack;
    public CommandMenuButton btnBrave;
    public CommandMenuButton btnSkill;
    public CommandMenuButton btnUniqueSkill;
    public CommandMenuButton btnDefault;
    [SerializeField] private GameObject objMenuMask;
    
    private List<Sprite> _bpSpriteList;
    private string _characterName;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(List<Sprite> bpSpriteList)
    {
        _bpSpriteList = bpSpriteList;
    }

    /// <summary>
    /// 刷新Character信息
    /// </summary>
    /// <param name="characterName"></param>
    public void RefreshCharacter(string characterName)
    {
        _characterName = characterName;
        var rowCfgCharacter = ConfigManager.Instance.cfgCharacter[_characterName];
        var assetManager = AssetManager.Instance;
        imageCharacterPortrait.sprite = assetManager.LoadAsset<Sprite>(rowCfgCharacter.portraitBattleMenuPath);
        imageCharacterName.sprite = assetManager.LoadAsset<Sprite>(rowCfgCharacter.nameBattleMenuPath);
    }
    
    /// <summary>
    /// 更新bp值
    /// </summary>
    /// <param name="bpCurrent"></param>
    /// <param name="bpPreview"></param>
    public void RefreshBp(int bpCurrent,int bpPreview)
    {
        if (_bpSpriteList==null||bpCurrent+4>=_bpSpriteList.Count||bpPreview+4>=_bpSpriteList.Count)
        {
            Debug.LogError("mp越界,bpCurrent="+bpCurrent+",bpPreview="+bpPreview);
            return;
        }
        imageBpCurrent.sprite = _bpSpriteList[bpCurrent + 4];
        imageBpPreview.sprite = _bpSpriteList[bpPreview + 4];
    }

    /// <summary>
    /// 刷新mp值
    /// </summary>
    /// <param name="mp"></param>
    public void RefreshMp(int mp)
    {
        textMp.text = mp.ToString();
        sliderMp.value = mp;
    }
    
    /// <summary>
    /// 手动绑定事件
    /// </summary>
    /// <param name="btnAttackFunc"></param>
    /// <param name="btnBraveFunc"></param>
    /// <param name="btnSkillFunc"></param>
    /// <param name="btnUniqueSkillFunc"></param>
    /// <param name="btnDefaultFunc"></param>
    public void BindEvent(UnityAction btnAttackFunc,UnityAction btnBraveFunc,UnityAction btnSkillFunc,UnityAction btnUniqueSkillFunc,UnityAction btnDefaultFunc)
    {
        btnAttack.onClick.AddListener(btnAttackFunc);
        btnBrave.onClick.AddListener(btnBraveFunc);
        btnSkill.onClick.AddListener(btnSkillFunc);
        btnUniqueSkill.onClick.AddListener(btnUniqueSkillFunc);
        btnDefault.onClick.AddListener(btnDefaultFunc);
    }

    /// <summary>
    /// 设置面板是否可以交互
    /// </summary>
    public void SetMenuInteractable(bool interactable)
    {
        objMenuMask.SetActive(interactable);
    }
}
}