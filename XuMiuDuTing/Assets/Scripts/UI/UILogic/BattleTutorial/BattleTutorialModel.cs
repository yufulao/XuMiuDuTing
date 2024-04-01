using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Yu
{
    public class BattleTutorialModel
    {
        private int _allPageCount;
        private int _currentPageIndex;
        
        
        /// <summary>
        /// 每次打开时
        /// </summary>
        /// <param name="allPageCount"></param>
        public void OnOpen(int allPageCount)
        {
            _allPageCount = allPageCount;
            _currentPageIndex = 0;
        }
        
        /// <summary>
        /// 下一页
        /// </summary>
        public void NextPage()
        {
            _currentPageIndex++;
            if (_currentPageIndex>=_allPageCount)
            {
                _currentPageIndex = _allPageCount - 1;
            }
        }

        /// <summary>
        /// 上一页
        /// </summary>
        public void PrePage()
        {
            _currentPageIndex--;
            if (_currentPageIndex<0)
            {
                _currentPageIndex = 0;
            }
        }

        /// <summary>
        /// 获取当前页index
        /// </summary>
        /// <returns></returns>
        public int GetCurrentPageIndex()
        {
            return _currentPageIndex;
        }

        /// <summary>
        /// 是否是第一页
        /// </summary>
        /// <returns></returns>
        public bool IsFirstPage()
        {
            return _currentPageIndex <= 0;
        }
        
        /// <summary>
        /// 是否是最后一页
        /// </summary>
        /// <returns></returns>
        public bool IsLastPage()
        {
            return _currentPageIndex >= _allPageCount-1;
        }
    }
}
