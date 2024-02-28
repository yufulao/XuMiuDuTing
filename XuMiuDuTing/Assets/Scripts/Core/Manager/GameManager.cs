using System;
using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;

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
        public void ReturnToTitle(float bgmFadeOutTime=0.5f)
        {
            StartCoroutine(ReturnToTitleIEnumerator(bgmFadeOutTime));
        }
        
        /// <summary>
        /// 返回Title
        /// </summary>
        /// <param name="bgmFadeOutTime"></param>
        /// <returns></returns>
        private IEnumerator ReturnToTitleIEnumerator(float bgmFadeOutTime)
        {
            UIManager.Instance.CloseAllWindows();
            UIManager.Instance.OpenWindow("LoadingView");
            yield return BGMManager.Instance.PlayBgmFadeDelay("主界面-章节选择界面", bgmFadeOutTime, 0f, 0.5f);
            SetTimeScale(1f);
            CameraManager.Instance.ResetObjCamera();
            GC.Collect();
            yield return SceneManager.Instance.ChangeSceneAsync(ConfigManager.Instance.cfgScene["Tittle"].scenePath);
            UIManager.Instance.OpenWindow("HomeView");
            UIManager.Instance.CloseWindow("LoadingView");
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