using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Yu
{
    public class ChapterSelectCtrl : UICtrlBase
    {
        private ChapterSelectModel _model;
        private ChapterSelectView _view;


        public override void OnInit(params object[] param)
        {
            _model = new ChapterSelectModel();
            _view = GetComponent<ChapterSelectView>();
            _model.OnInit();
            _view.OnInit();
        }

        public override void OpenRoot(params object[] param)
        {
            _view.OpenWindow();
            ResetDefaultChapterType();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
            _view.btnEnter.onClick.AddListener(BtnOnClickEnter);
            _view.btnMainPlot.onClick.AddListener(BtnOnClickMainPlot);
            _view.btnSubPlot.onClick.AddListener(BtnOnClickSubPlot);
        }

        /// <summary>
        /// 初始设置存档中的章节选择类别
        /// </summary>
        private void ResetDefaultChapterType()
        {
            var chapterType = SaveManager.GetString("ChapterType", DefChapterType.DMainPlot);
            if (chapterType == DefChapterType.DMainPlot)
            {
                BtnOnClickMainPlot();
            }

            if (chapterType == DefChapterType.DSubPlot)
            {
                BtnOnClickSubPlot();
            }

            _model.SetCurrentChapterType(chapterType);
        }

        /// <summary>
        /// 返回
        /// </summary>
        private void BtnOnClickBack()
        {
            CloseRoot();
        }

        /// <summary>
        /// 进入章节
        /// </summary>
        private void BtnOnClickEnter()
        {
            SaveManager.SetString("ChapterType", _model.GetCurrentChapterType());
            var chapterType = _model.GetCurrentChapterType();
            if (chapterType.Equals(DefChapterType.DMainPlot))
            {
                UIManager.Instance.OpenWindow("MainPlotSelectView");
            }
            if (chapterType.Equals(DefChapterType.DSubPlot))
            {
                //todo 支线剧情没做
                //UIManager.Instance.OpenWindow("SubPlotSelectView");
            }
        }

        /// <summary>
        /// 点击主线
        /// </summary>
        private void BtnOnClickMainPlot()
        {
            _model.SetCurrentChapterType(DefChapterType.DMainPlot);
            _view.btnMainPlot.transform.SetAsLastSibling();
            _view.btnMainPlot.interactable = false;
            _view.btnSubPlot.interactable = true;
            _view.RefreshInfoImage(AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgChapter[DefChapterType.DMainPlot].infoSpritePath));
        }

        /// <summary>
        /// 点击支线
        /// </summary>
        private void BtnOnClickSubPlot()
        {
            _model.SetCurrentChapterType(DefChapterType.DSubPlot);
            _view.btnSubPlot.transform.SetAsLastSibling();
            _view.btnSubPlot.interactable = false;
            _view.btnMainPlot.interactable = true;
            _view.RefreshInfoImage(AssetManager.Instance.LoadAsset<Sprite>(ConfigManager.Instance.cfgChapter[DefChapterType.DSubPlot].infoSpritePath));
        }
    }
}