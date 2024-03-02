// ******************************************************************
//       /\ /|       @file       ConfigManager.cs
//       \ V/        @brief      配置表管理器(由python自动生成)
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |
//      /  \\        @Modified   2022-04-23 22:40:15
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

namespace Rabi
{
    public sealed class ConfigManager : BaseSingleTon<ConfigManager>
    {
        public readonly CfgBGM cfgBGM = new CfgBGM();
        public readonly CfgCamera cfgCamera = new CfgCamera();
        public readonly CfgChapter cfgChapter = new CfgChapter();
        public readonly CfgChapterType cfgChapterType = new CfgChapterType();
        public readonly CfgCharacter cfgCharacter = new CfgCharacter();
        public readonly CfgEnemy cfgEnemy = new CfgEnemy();
        public readonly CfgGlobal cfgGlobal = new CfgGlobal();
        public readonly CfgMainPlot cfgMainPlot = new CfgMainPlot();
        public readonly CfgPrefab cfgPrefab = new CfgPrefab();
        public readonly CfgScene cfgScene = new CfgScene();
        public readonly CfgSFX cfgSFX = new CfgSFX();
        public readonly CfgSFXType cfgSFXType = new CfgSFXType();
        public readonly CfgStage cfgStage = new CfgStage();
        public readonly CfgSubPlot cfgSubPlot = new CfgSubPlot();
        public readonly CfgUI cfgUI = new CfgUI();
        public readonly CfgUICommon cfgUICommon = new CfgUICommon();

        public ConfigManager()
        {
            //初始场景有Text的情况 查找翻译文本需要加载资源 因为同为Awake回调 加载顺序可能优于AssetManager 故补充加载
            AssetManager.Instance.OnInit();
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            cfgBGM.Load();
            cfgCamera.Load();
            cfgChapter.Load();
            cfgChapterType.Load();
            cfgCharacter.Load();
            cfgEnemy.Load();
            cfgGlobal.Load();
            cfgMainPlot.Load();
            cfgPrefab.Load();
            cfgScene.Load();
            cfgSFX.Load();
            cfgSFXType.Load();
            cfgStage.Load();
            cfgSubPlot.Load();
            cfgUI.Load();
            cfgUICommon.Load();
        }
    }}