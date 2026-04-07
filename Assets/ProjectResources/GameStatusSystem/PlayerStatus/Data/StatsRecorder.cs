using System.Collections.Generic;
using UnityEngine;

namespace GameStatusSystem.PlayerStatus
{
    public class StatsRecorder
    {
        private LinkedList<float> _hitTimestamps;       // 命中时间戳链表

        private LinkedList<float> _shotTimestamps;       // 射击时间戳链表

        public StatsRecorder()
        {
            _hitTimestamps = new LinkedList<float>();
            _shotTimestamps = new LinkedList<float>();
        }

        // 记录命中
        public void RecordHit()
        {
            float currentTime = Time.time;
            _hitTimestamps.AddLast(currentTime);
            _shotTimestamps.AddLast(currentTime);
            CleanupOldTimestamps(currentTime);
        }

        // // 记录未命中
        // public void RecordMiss()
        // {
        //     float currentTime = Time.time;
        //     _shotTimestamps.AddLast(currentTime);
        //     CleanupOldTimestamps(currentTime);
        // }

        // 记录射击
        public void RecordShot()
        {
            float currentTime = Time.time;
            _shotTimestamps.AddLast(currentTime);
            CleanupOldTimestamps(currentTime);
        }

        // 获取指定时间范围内的命中率
        public float GetHitRate(float timeWindow = 5f)
        {
            float currentTime = Time.time;
            CleanupOldTimestamps(currentTime, timeWindow);

            int shotCount = GetShotCount(timeWindow);
            if (shotCount == 0) return 0f;

            int hitCount = GetHitCount(timeWindow);
            return (float)hitCount / shotCount;
        }

        // 获取指定时间范围内的射击次数
        public int GetShotCount(float timeWindow = 5f)
        {
            float currentTime = Time.time;
            CleanupOldTimestamps(currentTime, timeWindow);
            return _shotTimestamps.Count;
        }

        // 获取指定时间范围内的命中次数
        public int GetHitCount(float timeWindow = 5f)
        {
            float currentTime = Time.time;
            CleanupOldTimestamps(currentTime, timeWindow);
            return _hitTimestamps.Count;
        }

        // 清理过期的时间戳
        private void CleanupOldTimestamps(float currentTime, float timeWindow = 5f)
        {
            // 清理命中时间戳
            while (_hitTimestamps.Count > 0 && currentTime - _hitTimestamps.First.Value > timeWindow)
            {
                _hitTimestamps.RemoveFirst();
            }

            // 清理射击时间戳
            while (_shotTimestamps.Count > 0 && currentTime - _shotTimestamps.First.Value > timeWindow)
            {
                _shotTimestamps.RemoveFirst();
            }
        }

        // 定时清空（比如每5秒难度评估后调用）
        public void ClearWindow()
        {
            _hitTimestamps.Clear();
            _shotTimestamps.Clear();
        }

        

    }
}
