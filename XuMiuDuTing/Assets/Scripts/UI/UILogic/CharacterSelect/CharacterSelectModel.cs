using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;
using UnityEngine.Events;

namespace Yu
{
    public class CharacterSelectModel
    {
        private int _teammateIndex;
        private string _originalCharacterName;
        private string[] _teamArray;
        public UnityAction<int,string> onCharacterChange;//int是在team中的index，string是替换的characterName
        public UnityAction<int> onCharacterRemove;

        public void OnOpen(IReadOnlyList<object> param)
        {
            if (param.Count < 3)
            {
                Debug.LogError("传入CharacterSelect的参数不够");
                return;
            }

            _teamArray = param[0] as string[];
            _teammateIndex = int.Parse(param[1].ToString());
            _originalCharacterName = param[2]?.ToString();

            if (param.Count >= 5) //有change和remove事件
            {
                onCharacterChange = (UnityAction<int,string>) param[3];
                onCharacterRemove = (UnityAction<int>) param[4];
            }
        }

        /// <summary>
        /// 获取当前选择的角色名
        /// </summary>
        /// <returns></returns>
        public string GetOriginalCharacterName()
        {
            return _originalCharacterName;
        }

        /// <summary>
        /// 获取当前选择的队员编号
        /// </summary>
        /// <returns></returns>
        public int GetOriginalTeammateIndex()
        {
            return _teammateIndex;
        }

        /// <summary>
        /// 获取存档中的TeamData
        /// </summary>
        /// <returns></returns>
        public string[] GetTeamArray()
        {
            return _teamArray;
        }

        /// <summary>
        /// 获取所有角色名
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetCharacterNameList()
        {
            var characterList = ConfigManager.Instance.cfgCharacter.AllConfigs;
            var characterNameList = new List<string>();
            characterNameList.AddRange(characterList.Select(rowCfgCharacter => rowCfgCharacter.key));
            return characterNameList;
        }

        /// <summary>
        /// 判断这个character是不是在队伍里
        /// </summary>
        /// <param name="teamArray"></param>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public static bool IsCharacterInTeam(IEnumerable<string> teamArray, string characterName)
        {
            if (teamArray==null)
            {
                return false;
            }
            return teamArray.Any(characterNameInTeam => !string.IsNullOrEmpty(characterNameInTeam) && characterNameInTeam.Equals(characterName));
        }
    }
}