using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;

namespace Yu
{
    public partial class BattleManager : MonoSingleton<BattleManager>
    {
        public Transform entityObjContainer;
        public List<Transform> characterSpawnPointList = new List<Transform>();
        public List<Transform> enemySpawnPointList = new List<Transform>();

        private BattleMainCtrl _uiCtrl;
        private BattleFsm _fsm;
        private BattleModel _model;

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="teamArray"></param>
        public void Init(string[] teamArray)
        {
            _model = new BattleModel();
            _model.Init(teamArray);
            InitFsm();
            InitUI();
            EventManager.Instance.AddListener(EventName.OnRoundEnd, OnRoundEnd);
            // StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay("战斗a", 0f, 0.5f, 0f, 1f,
            //     () => { StartCoroutine(BGMManager.Instance.PlayBgmFadeDelay("战斗b", 0f, 0.5f, 0f)); }));
            SpawnEntity();
            InitCommand();
            _uiCtrl.view.UpdateAllEntityUIInfo(_model.allCharacterEntities, _model.allEnemyEntities);
            RefreshMenuInfo();
            StartCoroutine(AllEntityPlayEnterAnimation());
        }
        
        /// <summary>
        /// 以当前角色更新菜单面板
        /// </summary>
        public void RefreshMenuInfo()
        {
            var characterEntity = _model.allCharacterEntities[currentBattleId];
            _uiCtrl.view.RefreshMenuInfo(characterEntity.GetCharacterName(), characterEntity.GetBp(), characterEntity.GetBpPreview(), characterEntity.GetMp());
        }

        /// <summary>
        /// 生成角色和敌人并初始化
        /// </summary>
        private void SpawnEntity()
        {
            //清空敌人和角色entity列表
            _model.characterNumber = 0;
            _model.characterCount = 0;
            _model.enemyNumber = 0;
            _model.allEntities.Clear();
            for (var i = 0; i < entityObjContainer.transform.childCount; i++)
            {
                Destroy(entityObjContainer.transform.GetChild(i).gameObject);
            }

            _uiCtrl.ClearAllEntityHud();

            var characterTeam = _model.GetCharacterTeamArray();
            //设置角色
            if (characterSpawnPointList.Count < characterTeam.Length)
            {
                Debug.LogError("队伍角色数量大于可用战斗位置数量");
            }

            for (var i = 0; i < characterTeam.Length; i++)
            {
                var characterName = characterTeam[i];
                if (string.IsNullOrEmpty(characterName))
                {
                    _uiCtrl.view.characterInfoItemList[i].gameObject.SetActive(false);
                    continue;
                }

                _uiCtrl.view.characterInfoItemList[i].gameObject.SetActive(true);
                // buffManager.ClearBuff(characterInfoUiList[i]);
                var rowCfgCharacter = ConfigManager.Instance.cfgCharacter[characterName];
                //生成角色
                var characterObj = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgCharacter.entityObjPath), entityObjContainer.transform);
                //设置角色站位
                characterObj.transform.position = characterSpawnPointList[i].position;
                //设置entity属性
                var characterEntity = characterObj.GetComponent<CharacterEntityCtrl>();
                var infoItem = _uiCtrl.view.characterInfoItemList[i];
                var entityHud = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(
                    ConfigManager.Instance.cfgUICommon["EntityHUD"].path), _uiCtrl.view.entityHudContainer).GetComponent<EntityHUD>();
                characterEntity.Init(characterName, infoItem, entityHud);
                // characterEntity.skillVfxAnim = characterObj.transform.Find("SkillVfx").GetComponent<Animator>();
                // characterEntity.buffVfxAnim = characterObj.transform.Find("BuffVfx").GetComponent<Animator>();
                //设置HUD跟随组件
                var uiFollowObj = characterEntity.gameObject.AddComponent<UIFollowObj>();
                uiFollowObj.objFollowed = characterObj.transform.Find("HudFollowPoint");
                uiFollowObj.rectFollower = entityHud.GetComponent<RectTransform>();
                uiFollowObj.offset = new Vector2(0f, 15f);
                //一些初始设置
                characterEntity.SetEntitySpineOutlineActive(false);
                //加进entityList
                _model.characterNumber++;
                _model.characterCount++;
                _model.allEntities.Add(characterEntity);
                _model.allCharacterEntities.Add(characterEntity);
            }

            var enemyTeam = _model.GetEnemyTeam();
            //生成敌人
            if (enemyTeam.Count > enemySpawnPointList.Count)
            {
                Debug.LogError("敌人不够生成位置");
            }

            for (var i = 0; i < enemyTeam.Count; i++)
            {
                var enemyName = enemyTeam[i];
                if (string.IsNullOrEmpty(enemyName))
                {
                    _uiCtrl.view.enemyInfoItemList[i].gameObject.SetActive(false);
                    continue;
                }

                _uiCtrl.view.enemyInfoItemList[i].gameObject.SetActive(true);
                //buffManager.ClearBuff(enemyInfoUiList[i]);
                var rowCfgEnemy = ConfigManager.Instance.cfgEnemy[enemyTeam[i]];
                //生成敌人
                var enemyObj = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgEnemy.entityObjPath), entityObjContainer.transform);
                //设置敌人站位
                enemyObj.transform.position = enemySpawnPointList[i].position;
                //设置entity属性
                var enemyEntity = enemyObj.GetComponent<EnemyEntityCtrl>();
                var infoItem = _uiCtrl.view.enemyInfoItemList[i];
                var entityHud = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(
                    ConfigManager.Instance.cfgUICommon["EntityHUD"].path), _uiCtrl.view.entityHudContainer).GetComponent<EntityHUD>();
                enemyEntity.Init(enemyName, infoItem, entityHud);
                // enemyEntity.skillVfxAnim = enemyObj.transform.Find("SkillVfx").GetComponent<Animator>();
                // enemyEntity.buffVfxAnim = enemyObj.transform.Find("BuffVfx").GetComponent<Animator>();
                //设置HUD跟随组件
                var uiFollowObj = enemyEntity.gameObject.AddComponent<UIFollowObj>();
                uiFollowObj.objFollowed = enemyObj.transform.Find("HudFollowPoint");
                uiFollowObj.rectFollower = entityHud.GetComponent<RectTransform>();
                uiFollowObj.offset = new Vector2(0f, 15f);
                //一些初始设置
                enemyEntity.SetEntitySpineOutlineActive(false);
                //加进entityList
                _model.enemyNumber++;
                _model.allEntities.Add(enemyEntity);
                _model.allEnemyEntities.Add(enemyEntity);
            }

            var enemyInfoItemList = _uiCtrl.view.enemyInfoItemList;
            //设置左上角敌人列表
            for (var i = enemyTeam.Count; i < enemyInfoItemList.Count; i++)
            {
                enemyInfoItemList[i].gameObject.SetActive(false);
            }

            InitAimSelectToggleList(); // 初始化entity的复选框
        }

        /// <summary>
        /// 初始化entity的复选框
        /// </summary>
        private void InitAimSelectToggleList()
        {
            var aimSelectPanel = _uiCtrl.view.aimSelectPanel;
            aimSelectPanel.gameObject.SetActive(false);
            _model.activedToggleList.Clear();
            for (var i = 0; i < _model.allEntities.Count; i++)
            {
                var index = i;
                _model.allEntities[index].GetEntityHud().SetToggleSelectOnValueChange((isOn) => SelectEntity(isOn, _model.allEntities[index]));
            }

            ResetSelectEntity();
        }

        /// <summary>
        /// 给selectAttackButton调用
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="entity"></param>
        private void SelectEntity(bool isOn, BattleEntityCtrl entity)
        {
            var selectedEntityList = _model.selectedEntityList;
            var activedToggleList = _model.activedToggleList;
            if (isOn)
            {
                selectedEntityList.Add(entity);
                _model.canSelectCount--;
                if (_model.canSelectCount <= 0 && activedToggleList[0].group == null) //activeToggleList[0].group != null来判断是不是只允许选择单体,因为选择单体要设定group
                {
                    activedToggleList
                        .Where(activeToggle => !activeToggle.isOn)
                        .ToList()
                        .ForEach(activeToggle => activeToggle.gameObject.SetActive(false));
                }

                return;
            }

            selectedEntityList.Remove(entity);
            if (_model.canSelectCount == 0)
            {
                foreach (var activeToggle in activedToggleList)
                {
                    activeToggle.gameObject.SetActive(true);
                }
            }

            _model.canSelectCount++;
        }

        /// <summary>
        /// 重置选择目标
        /// </summary>
        private void ResetSelectEntity()
        {
            foreach (var entityCtrl in _model.allEntities)
            {
                var toggleSelect = entityCtrl.GetEntityHud().toggleSelect;
                toggleSelect.isOn = false;
                toggleSelect.gameObject.SetActive(false);
            }

            _model.selectedEntityList.Clear();
            _model.canSelectCount = -1;
        }

        /// <summary>
        /// 回合结束事件
        /// </summary>
        private void OnRoundEnd()
        {
            //Buff生效
            //重置防御
        }

        /// <summary>
        /// 所有entity入场动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator AllEntityPlayEnterAnimation()
        {
            //todo 入场动画
            var allEntities = _model.allEntities;
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < allEntities.Count; i++)
            {
                try
                {
                    //StartCoroutine(Utils.PlayAnimation(allEntities[i].animator, "start"));
                }
                catch (System.Exception)
                {
                }
            }

            //yield return new WaitForSeconds(Utils.GetAnimatorLength(allEntities[0].animator, "start"));
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitUI()
        {
            UIManager.Instance.OpenWindow("BattleMainView");
            _uiCtrl = UIManager.Instance.GetCtrl<BattleMainCtrl>("BattleMainView");
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void InitFsm()
        {
            _fsm = FsmManager.Instance.GetFsm<BattleFsm>();
            _fsm.SetFsm(new Dictionary<Type, IFsmState>()
            {
                {typeof(CharacterCommandInputState), new CharacterCommandInputState()},
                {typeof(EnemyCommandInputState), new EnemyCommandInputState()},
                {typeof(ExecutingState), new ExecutingState()}
            });
        }
    }
}