using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;

namespace Yu
{
    public class CharacterCatalogModel
    {
        private RowCfgCharacter _cacheRowCfgCharacter;
        private bool _isSameCharacter;
        private ItemVoiceNode _cacheItemVoiceNode;
        private readonly List<RowCfgCharacterVoice> _rowCfgVoiceList = new List<RowCfgCharacterVoice>();
        private readonly List<RowCfgCharacterArchive> _rowCfgArchiveList = new List<RowCfgCharacterArchive>();

        /// <summary>
        /// 每次打开时
        /// </summary>
        /// <param name="param"></param>
        public void OnOpen(params object[] param)
        {
            if (param.Length < 1)
            {
                Debug.LogError("没有传入角色名");
                return;
            }

            //角色名与缓存的一致就不用重复加载
            SetCacheCharacter(param[0].ToString());
            if (!_isSameCharacter)
            {
                LoadCharacterAllRowCfg();
            }
        }

        /// <summary>
        /// 是否是上一次打开时的的角色
        /// </summary>
        public bool IsSameCharacter()
        {
            return _isSameCharacter;
        }

        /// <summary>
        /// 获取RowCfgCharacter
        /// </summary>
        /// <returns></returns>
        public RowCfgCharacter GetRowCfgCharacter()
        {
            return _cacheRowCfgCharacter;
        }

        /// <summary>
        /// 设置当前ItemVoiceNode
        /// </summary>
        public void SetCurrentItemVoiceNode(ItemVoiceNode itemVoiceNode)
        {
            _cacheItemVoiceNode = itemVoiceNode;
        }

        /// <summary>
        /// 获取当前ItemVoiceNode
        /// </summary>
        /// <returns></returns>
        public ItemVoiceNode GetCurrentItemVoiceNode()
        {
            return _cacheItemVoiceNode;
        }

        /// <summary>
        /// 获取角色所有语音rowCfg
        /// </summary>
        /// <returns></returns>
        public List<RowCfgCharacterVoice> GetRowCfgVoiceList()
        {
            return _rowCfgVoiceList;
        }

        /// <summary>
        /// 获取角色所有档案rowCfg
        /// </summary>
        /// <returns></returns>
        public List<RowCfgCharacterArchive> GetRowCfgArchiveList()
        {
            return _rowCfgArchiveList;
        }

        /// <summary>
        /// 加载角色所有的语音和档案数据
        /// </summary>
        private void LoadCharacterAllRowCfg()
        {
            if (_cacheRowCfgCharacter==null)
            {
                return;
            }
            
            _rowCfgVoiceList.Clear();
            _rowCfgArchiveList.Clear();
            var configManager = ConfigManager.Instance;
            var rowCfgCharacter = configManager.cfgCharacter[_cacheRowCfgCharacter.key];
            foreach (var voiceKey in rowCfgCharacter.catalogVoiceList)
            {
                _rowCfgVoiceList.Add(configManager.cfgCharacterVoice[voiceKey]);
            }

            foreach (var archiveKey in rowCfgCharacter.catalogArchiveList)
            {
                _rowCfgArchiveList.Add(configManager.cfgCharacterArchive[archiveKey]);
            }
        }

        /// <summary>
        /// 检测是否是上一个角色
        /// </summary>
        private void SetCacheCharacter(string characterName)
        {
            _isSameCharacter=_cacheRowCfgCharacter != null && characterName.Equals(_cacheRowCfgCharacter.key);
            _cacheRowCfgCharacter = ConfigManager.Instance.cfgCharacter[characterName];
        }
    }
}