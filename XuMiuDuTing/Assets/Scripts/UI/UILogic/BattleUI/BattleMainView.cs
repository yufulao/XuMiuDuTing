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
        public Transform entityHudContainer;
        public SkillSelectPanel skillSelectPanel;
        public AimSelectPanel aimSelectPanel;
        public DescribeItem describeItemBuff;
        public DescribeItem describeItemSkill;
        public GameObject objMask;

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
        }
    }
}