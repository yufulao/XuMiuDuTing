using System.Collections;
using System.Collections.Generic;

namespace Yu
{
    public class PauseModel
    {
        private float _cacheTimeScale;
        private bool _isOnPause;

        public void OnInit()
        {
            _cacheTimeScale = 1f;
        }

        /// <summary>
        /// 设置是否是暂停状态
        /// </summary>
        /// <param name="isOnPause"></param>
        public void SetOnPause(bool isOnPause)
        {
            _isOnPause = isOnPause;
        }
        
        /// <summary>
        /// 获取是否是暂停状态
        /// </summary>
        public bool GetOnPause()
        {
            return _isOnPause;
        }

        /// <summary>
        /// 获取暂停之前的时间速率
        /// </summary>
        /// <returns></returns>
        public float GetTimeScale()
        {
            return _cacheTimeScale;
        }

        /// <summary>
        /// 设置暂停之前的时间速率
        /// </summary>
        /// <param name="timeScale"></param>
        public void SetTimeScale(float timeScale)
        {
            _cacheTimeScale = timeScale;
        }
    }
}
