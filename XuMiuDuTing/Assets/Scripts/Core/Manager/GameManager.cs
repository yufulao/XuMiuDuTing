using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Rabi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Yu
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private readonly List<IMonoManager> _managerList = new List<IMonoManager>();
        public bool test; //测试模式
        public bool crack; //破解版

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            //Application.targetFrameRate = 120;

            _managerList.Add(EventManager.Instance);
            _managerList.Add(AssetManager.Instance);
            _managerList.Add(InputManager.Instance);
            _managerList.Add(FsmManager.Instance);
            _managerList.Add(BGMManager.Instance);
            _managerList.Add(SFXManager.Instance);
            _managerList.Add(SceneManager.Instance);
            _managerList.Add(UIManager.Instance);
            _managerList.Add(CameraManager.Instance);
            _managerList.Add(ProcedureManager.Instance);

            foreach (var manager in _managerList)
            {
                manager.OnInit();
            }
        }

        private void Start()
        {
            if (test)
            {
                IsTest();
            }

            VersionControl();//版本检查

            BGMManager.Instance.ReloadVolume();
            SFXManager.Instance.ReloadVolume();
            ReturnToTitle(0f);
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        public void ReturnToTitle(float bgmFadeOutTime = 0.5f, UnityAction callback = null)
        {
            StartCoroutine(ReturnToTitleIEnumerator(bgmFadeOutTime, callback));
        }

        /// <summary>
        /// 进入下一个关卡步骤
        /// </summary>
        public void EnterNextStageProcedure()
        {
            ProcedureManager.Instance.EnterNextStageProcedure();
        }

        /// <summary>
        /// 通关当前关卡
        /// </summary>
        public void PassStage()
        {
            var stageName = SaveManager.GetString("StageName", "1-1");
            PassStage(stageName);
        }

        /// <summary>
        /// 解锁关卡
        /// </summary>
        public void UnlockStage(string stageName)
        {
            var stageData = SaveManager.GetT("StageData", new StageData());
            if (stageData.allStage.ContainsKey(stageName))
            {
                if (stageData.allStage[stageName].isUnlock)
                {
                    return;
                }

                stageData.allStage[stageName].isUnlock = true;
                SaveManager.SetT("StageData", stageData);
                return;
            }

            stageData.allStage.Add(stageName, new StageDataEntry()
            {
                stageName = stageName,
                isPass = false,
                isUnlock = true
            });
            SaveManager.SetT("StageData", stageData);
        }

        /// <summary>
        /// 通关指定关卡
        /// </summary>
        public void PassStage(string stageName)
        {
            var stageData = SaveManager.GetT("StageData", new StageData());
            var stageDataEntry = stageData.allStage[stageName];
            var rowCfgStage = ConfigManager.Instance.cfgStage[stageName];
            if (stageDataEntry.isPass)
            {
                return;
            }

            stageDataEntry.isPass = true;
            SaveManager.SetT("StageData", stageData);

            var unlockStageList = rowCfgStage.unlockStageList;
            if (unlockStageList == null)
            {
                return;
            }

            foreach (var unlockStageName in unlockStageList)
            {
                UnlockStage(unlockStageName);
            }

            //其他通关特定关执行卡特定事件，下面写================================================================================================================
        }

        /// <summary>
        /// 返回Title
        /// </summary>
        /// <param name="bgmFadeOutTime"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator ReturnToTitleIEnumerator(float bgmFadeOutTime, UnityAction callback)
        {
            DialogueManager.instance.StopAllConversations();
            UIManager.Instance.GetCtrlWithCreate<BattleMainCtrl>("BattleMainView")?.ClearAllEntityHud(); //关闭所有hud，因为HUD独立
            UIManager.Instance.CloseAllLayerWindows("NormalLayer");
            UIManager.Instance.OpenWindow("LoadingView");
            yield return BGMManager.Instance.PlayBgmFadeDelay("主界面-章节选择界面", bgmFadeOutTime, 0f, 0.5f, 1f);
            SetTimeScale(1f);
            CameraManager.Instance.ResetObjCamera();
            GC.Collect();
            yield return SceneManager.Instance.ChangeSceneAsync(ConfigManager.Instance.cfgScene["Tittle"].scenePath);
            UIManager.Instance.DestroyWindow("BattleMainView");
            UIManager.Instance.OpenWindow("HomeView");
            UIManager.Instance.CloseWindow("LoadingView");
            ProcedureManager.Instance.SetNullState();
            callback?.Invoke();
        }

        /// <summary>
        /// 进入关卡
        /// </summary>
        public void EnterStage()
        {
            StartCoroutine(IEnterStage());
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitApplication()
        {
            Application.Quit();
        }

        /// <summary>
        /// 设置时间速率
        /// </summary>
        /// <param name="timeScale"></param>
        public void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        /// <summary>
        /// 测试模式
        /// </summary>
        private void IsTest()
        {
            Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Instance.cfgUI["IngameDebugView"].uiPath)
                , UIManager.Instance.GetUIRoot().Find("NormalLayer")).GetComponent<Canvas>().worldCamera = CameraManager.Instance.GetUICamera();
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void OnUpdateCheckPause()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                var pauseCtrl = UIManager.Instance.GetCtrl<PauseCtrl>("PauseView");
                if (pauseCtrl.GetIsOnPause()) //当前是暂停状态
                {
                    UIManager.Instance.CloseWindow("PauseView");
                    return;
                }

                UIManager.Instance.OpenWindow("PauseView");
            }
        }

        /// <summary>
        /// 进入游戏关卡
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnterStage()
        {
            UIManager.Instance.OpenWindow("LoadingView"); //加载界面
            yield return BGMManager.Instance.StopBgmFadeDelay(0f, 0.4f); //关闭bgm
            yield return new WaitForSeconds(0.5f);
            //yield return SceneManager.Instance.ChangeSceneAsync(rowCfgStage.scenePath); //切换场景
            GC.Collect(); //清gc
            UIManager.Instance.CloseWindow("LoadingView"); //关闭加载界面
        }

        /// <summary>
        /// 游戏版本控制
        /// </summary>
        private void VersionControl()
        {
            var lastVersion = SaveManager.GetString("Version", "0.0.1");
            var nowVersion = Application.version;
            SaveManager.SetString("Version", nowVersion);
            if (lastVersion.Equals(nowVersion))
            {
                return;
            }

            SaveManager.DeleteKey("StageName");
            SaveManager.DeleteKey("PlotNameInMainPlot");
            SaveManager.DeleteKey("StageData");
            SaveManager.DeleteKey("TeamData");
            SaveManager.DeleteKey("SkillData");
        }

        private void Update()
        {
            foreach (var manager in _managerList)
            {
                manager.Update();
            }
        }

        private void FixedUpdate()
        {
            foreach (var manager in _managerList)
            {
                manager.FixedUpdate();
            }
        }

        private void LateUpdate()
        {
            foreach (var manager in _managerList)
            {
                manager.LateUpdate();
            }
        }

        private void OnDestroy()
        {
            for (var i = _managerList.Count - 1; i >= 0; i--)
            {
                _managerList[i].OnClear();
            }
        }
    }
}