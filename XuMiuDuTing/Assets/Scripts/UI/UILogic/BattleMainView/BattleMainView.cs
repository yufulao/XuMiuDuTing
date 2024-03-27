using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class BattleMainView : MonoBehaviour
    {
        public List<CharacterInfoItem> characterInfoItemList = new List<CharacterInfoItem>();
        public List<EnemyInfoItem> enemyInfoItemList = new List<EnemyInfoItem>();
        public List<CommandMenu> commandMenuList = new List<CommandMenu>();
        public Button btnUndoCommand;
        public Button btnGoBattle;
        public Animator animatorBtnGoBattle;
        [HideInInspector]public Transform entityHudContainer;
        public SkillSelectPanel skillSelectPanel;
        public AimSelectPanel aimSelectPanel;
        public DescribeItem describeItemBuff;
        public DescribeItem describeItemSkill;
        public GameObject objMask;
        public ToggleGroup selectToggleGroup;

        public List<Sprite> bpSpriteList = new List<Sprite>();
        public Animator animator;

        public void OnInit()
        {
            InitAllUI();
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow()
        {
            objMask.SetActive(false);
            gameObject.SetActive(true);
            animator.Play("Show", 0, 0f);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            StartCoroutine(CloseWindowIEnumerator());
        }

        /// <summary>
        /// 关闭窗口的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseWindowIEnumerator()
        {
            objMask.SetActive(true);
            yield return Utils.PlayAnimation(animator, "Hide");
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 初始化所有ui
        /// </summary>
        private void InitAllUI()
        {
            foreach (var characterInfoItem in characterInfoItemList)
            {
                characterInfoItem.Init(bpSpriteList);
            }

            foreach (var commandMenu in commandMenuList)
            {
                commandMenu.Init(bpSpriteList);
            }

            skillSelectPanel.Init();

            for (var i = 1; i < commandMenuList.Count; i++) //关闭多余的界面
            {
                commandMenuList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 0f);
                commandMenuList[i].SetMenuInteractable(true);
                commandMenuList[i].gameObject.SetActive(false);
            }
            
            describeItemBuff.ForceClose();
            describeItemSkill.ForceClose();
            entityHudContainer = UIManager.Instance.GetUIRoot().Find("NormalLayer").Find("EntityHudContainer");
        }

        /// <summary>
        /// 更新角色指令面板信息
        /// </summary>
        public void RefreshMenuInfo(string characterName, int bpCurrent, int bpPreview, int mp)
        {
            foreach (var commandMenu in commandMenuList)
            {
                commandMenu.RefreshCharacter(characterName);
                commandMenu.RefreshBp(bpCurrent, bpPreview);
                commandMenu.RefreshMp(mp);
            }
        }

        /// <summary>
        /// 更新下方角色信息列表
        /// </summary>
        public void UpdateAllEntityInfoItem(IEnumerable<CharacterEntityCtrl> allCharacterEntities, IEnumerable<EnemyEntityCtrl> allEnemyEntities)
        {
            foreach (var characterEntity in allCharacterEntities)
            {
                var infoItem = characterEntity.GetInfoItem();
                if (characterEntity.IsDie())
                {
                    infoItem.RefreshOnDie();
                    characterEntity.RefreshEntityHud();
                    continue;
                }

                infoItem.RefreshOnNotDie(characterEntity.GetHp(), characterEntity.GetMp(), characterEntity.GetBp());
                characterEntity.RefreshEntityHud();
            }

            foreach (var enemyEntity in allEnemyEntities)
            {
                var infoItem = enemyEntity.GetEnemyInfoItem();
                if (enemyEntity.IsDie())
                {
                    infoItem.RefreshOnDie();
                    enemyEntity.RefreshEntityHud();
                    continue;
                }

                infoItem.RefreshOnNotDie(enemyEntity.GetHp(),0,0);
                enemyEntity.RefreshEntityHud();
            }
        }
    }
}