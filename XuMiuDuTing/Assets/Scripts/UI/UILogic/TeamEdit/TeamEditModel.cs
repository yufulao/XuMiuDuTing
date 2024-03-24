using System.Collections.Generic;
using System.Linq;
using Rabi;
using UnityEngine;

namespace Yu
{
    public class TeamEditModel
    {
        private string _currentChapterType;
        private TeamData _teamData;
        private bool _isFixed;

        public void OnOpen()
        {
            LoadTeamData();
            _isFixed = false;
        }

        /// <summary>
        /// 设置固定编队
        /// </summary>
        /// <param name="isFixed"></param>
        /// <param name="teamArray"></param>
        public void SetFixedTeamArray(bool isFixed, string[] teamArray)
        {
            _isFixed = isFixed;
            if (isFixed)
            {
                _teamData.teamArray = teamArray;
            }
        }

        /// <summary>
        /// 交换队员
        /// </summary>
        /// <param name="teammateIndex"></param>
        /// <param name="replaceCharacterName"></param>
        public void ChangeTeammate(int teammateIndex, string replaceCharacterName)
        {
            _teamData.teamArray[teammateIndex] = replaceCharacterName;
        }

        /// <summary>
        /// 移除队员
        /// </summary>
        /// <param name="teammateIndex"></param>
        public void RemoveTeammate(int teammateIndex)
        {
            _teamData.teamArray[teammateIndex] = null;
        }

        /// <summary>
        /// 获取是否是固定编队
        /// </summary>
        /// <returns></returns>
        public bool GetIsFixed()
        {
            return _isFixed;
        }

        /// <summary>
        /// 获取编队列表
        /// </summary>
        /// <returns></returns>
        public string[] GetTeamArray()
        {
            return _teamData.teamArray;
        }

        /// <summary>
        /// 检测是否一个都没上场
        /// </summary>
        /// <returns></returns>
        public bool IsTeamArrayEmpty()
        {
            if (_teamData?.teamArray == null)
            {
                return true;
            }

            foreach (var characterName in _teamData.teamArray)
            {
                if (string.IsNullOrEmpty(characterName))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 保存当前队伍数组到存档
        /// </summary>
        public void SaveTeamData()
        {
            if (_isFixed)
            {
                return;
            }

            SaveManager.SetT("TeamData", _teamData);
        }

        /// <summary>
        /// 加载存档中的队伍数组
        /// </summary>
        private void LoadTeamData()
        {
            _teamData = SaveManager.GetT("TeamData", new TeamData());
        }
    }
}