using System.Collections;
using System.Collections.Generic;
using Rabi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class CharacterInfoItem : MonoBehaviour
{
    [SerializeField] private GameObject objSelectedBg;
    [SerializeField] private Image imagePortrait;
    [SerializeField] private GameObject objReadyTip;
    [SerializeField] private Slider sliderHp;
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private Slider sliderMp;
    [SerializeField] private TextMeshProUGUI textMp;
    [SerializeField] private Image imageBp;
    [SerializeField] private Transform buffsContainer;

    [SerializeField] private Animator selectedBgAnimator;
    // public List<BuffObject> buffs;
    // public List<BuffObject> debuffs;

    private string _characterName;
    private List<Sprite> _bpSpriteList;

    public void Init(List<Sprite> bpSpriteList)
    {
        _bpSpriteList = bpSpriteList;
    }

    /// <summary>
    /// 第一次刷新
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="bp"></param>
    /// <param name="maxHp"></param>
    /// <param name="mp"></param>
    public void RefreshOnStart(string characterName,int bp,int maxHp,int mp)
    {
        _characterName = characterName;
        imagePortrait.sprite = AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgCharacter[_characterName].portraitBattleMenuPath);
        RefreshBp(bp);
        RefreshMp(mp);
        RefreshHp(0, maxHp);
        SetActiveObjReadyTip(false);
    }

    /// <summary>
    /// 刷新Bp
    /// </summary>
    /// <param name="bp"></param>
    public void RefreshBp(int bp)
    {
        if (_bpSpriteList==null||bp+4>=_bpSpriteList.Count)
        {
            Debug.LogError("mp越界,bp="+bp);
            return;
        }
        imageBp.sprite = _bpSpriteList[bp + 4];
    }
    
    /// <summary>
    /// 激活启用readyTip
    /// </summary>
    public void SetActiveObjReadyTip(bool active)
    {
        objReadyTip.SetActive(active);
    }

    /// <summary>
    /// 刷新生命值显示
    /// </summary>
    public void RefreshMp(int mp)
    {
        sliderMp.value = mp;
        textMp.text = mp.ToString();
    }
    
    /// <summary>
    /// 刷新生命值显示
    /// </summary>
    public void RefreshHp(int hp,int maxHp)
    {
        sliderHp.maxValue = maxHp;
        sliderHp.value = hp;
        textHp.text = hp.ToString();
    }
    
    /// <summary>
    /// 被选择时
    /// </summary>
    public void EnterSelect()
    {
        selectedBgAnimator.SetTrigger("selectedBgOpen");
    }

    /// <summary>
    /// 取消选择时
    /// </summary>
    public void QuitSelect()
    {
        selectedBgAnimator.SetTrigger("selectedBgClose");
    }

    /// <summary>
    /// 没有被选择时
    /// </summary>
    public void NoSelected()
    {
        selectedBgAnimator.Play("idle");
    }
}
}