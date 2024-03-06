using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Events;

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

            BGMManager.Instance.ReloadVolume();
            SFXManager.Instance.ReloadVolume();
            //测试
            //StartCoroutine(BgmManager.Instance.PlayBgmFadeDelay("TestBgm",0f, 0f, 0f));
            //StartCoroutine(SfxManager.Instance.PlaySfx("TestSfx",1f));
            //EventManager.Instance.AddListener(EventName.Click,()=>{Debug.Log("Click");});
            //Debug.Log(ConfigManager.Instance.cfgBgm["TestBgm"].key);
            // StartCoroutine(SceneManager.Instance.ChangeSceneAsync("StageTest",(sceneInstance)=>
            // {
            //     BattleManager.Instance.OnInit();
            //     BattleManager.Instance.EnterStageScene("StageTest");
            // }));
            //GameObject obj = CommandManager.Instance.CreatWaitingObj();
            //SaveManager.SetFloat("TestFloat",0.5f);
            //Debug.Log(SaveManager.GetFloat("TestFloat", 0.1f));
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
            UIManager.Instance.CloseAllLayerWindows("NormalLayer");
            UIManager.Instance.OpenWindow("LoadingView");
            yield return BGMManager.Instance.PlayBgmFadeDelay("主界面-章节选择界面", bgmFadeOutTime, 0f, 0.5f);
            SetTimeScale(1f);
            CameraManager.Instance.ResetObjCamera();
            GC.Collect();
            yield return SceneManager.Instance.ChangeSceneAsync(ConfigManager.Instance.cfgScene["Tittle"].scenePath);
            UIManager.Instance.DestroyWindow("BattleMainView");
            UIManager.Instance.OpenWindow("HomeView");
            UIManager.Instance.CloseWindow("LoadingView");
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
                , UIManager.Instance.GetUIRoot().Find("NormalLayer"));
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