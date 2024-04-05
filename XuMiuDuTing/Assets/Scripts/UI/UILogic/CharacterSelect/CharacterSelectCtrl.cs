using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Yu
{
    public class CharacterSelectCtrl : UICtrlBase
    {
        private CharacterSelectModel _model;
        private CharacterSelectView _view;

        
        public override void OnInit(params object[] param)
        {
            _model = new CharacterSelectModel();
            _view = GetComponent<CharacterSelectView>();
        }

        //param[0]是teamArray,param[1]是teammateIndex，param[2]是originalCharacterName
        //param[3]是UnityAction<string> OnChange，param[4]是UnityAction<string> OnRemove
        public override void OpenRoot(params object[] param)
        {
            _model.OnOpen(param);
            UpdateAllCharacterItem();
            _view.OpenWindow();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
            _view.btnBack.onClick.AddListener(BtnOnClickBack);
        }

        /// <summary>
        /// 刷新角色列表
        /// </summary>
        private void UpdateAllCharacterItem()
        {
            var teamArray = _model.GetTeamArray();
            var originalCharacterName = _model.GetOriginalCharacterName();
            var characterNameList = CharacterSelectModel.GetCharacterNameList();
            for (var i = 0; i < _view.characterItemContainer.childCount; i++)
            {
                //todo 对象池处理
                Destroy(_view.characterItemContainer.GetChild(i).gameObject);
            }

            foreach (var characterName in characterNameList)
            {
                var characterItem = Instantiate(AssetManager.Instance.LoadAsset<GameObject>(
                    ConfigManager.Instance.cfgUICommon["CharacterItem"].path), _view.characterItemContainer).GetComponent<CharacterItem>();
                var isOriginalCharacter = characterName.Equals(originalCharacterName);
                var isTeammate = CharacterSelectModel.IsCharacterInTeam(teamArray, characterName);
                var canChange = !isOriginalCharacter && !isTeammate;
                var objInTeamMaskActive = isTeammate && characterName != originalCharacterName;
                characterItem.Refresh(characterName, canChange, isOriginalCharacter, objInTeamMaskActive);
                characterItem.SetBtnOnClickChange(CharacterItemOnBtnClickChange);
                characterItem.SetBtnOnClickRemove(CharacterItemOnBtnClickRemove);
            }
        }

        /// <summary>
        /// 当characterItem点击BtnChange时
        /// </summary>
        /// <param name="characterName"></param>
        private void CharacterItemOnBtnClickChange(string characterName)
        {
            _model.onCharacterChange?.Invoke(_model.GetOriginalTeammateIndex(),characterName);
            CloseRoot();
        }

        /// <summary>
        /// 当characterItem点击BtnRemove时
        /// </summary>
        private void CharacterItemOnBtnClickRemove()
        {
            _model.onCharacterRemove?.Invoke(_model.GetOriginalTeammateIndex());
            CloseRoot();
        }

        /// <summary>
        /// 返回
        /// </summary>
        private void BtnOnClickBack()
        {
            CloseRoot();
        }
    }
}