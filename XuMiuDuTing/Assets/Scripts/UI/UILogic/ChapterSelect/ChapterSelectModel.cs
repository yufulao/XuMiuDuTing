using System.Collections.Generic;
using Rabi;
using UnityEngine;

namespace Yu
{
    public class ChapterSelectModel
    {
        private string _currentChapterType;

        public void OnInit()
        {
        }

        /// <summary>
        /// 获取当前选择的章节类别
        /// </summary>
        /// <returns></returns>
        public string GetCurrentChapterType()
        {
            return _currentChapterType;
        }
        
        /// <summary>
        /// 设置当前选择的章节类别
        /// </summary>
        /// <param name="chapterType"></param>
        public void SetCurrentChapterType(string chapterType)
        {
            _currentChapterType = chapterType;
        }
    }
}
