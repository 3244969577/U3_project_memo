using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        // 依赖
        private Transform crosshair; // 十字准星
        private Transform player;      // 玩家目标
        private Transform target;    // 跟随目标


        public float lerpSpeed = 1.0f;

        private Vector3 offset;

        private Vector3 targetPos;

        private void Awake()
        {
            // Debug.Log("CameraFollow Awake" + GameManager.instance);
            
        }

        private void Start()
        {
            Debug.Assert(GameManager.instance != null, "GameManager is null");
            Debug.Assert(GameManager.instance.player != null, "Player is null");

            player = GameManager.instance.player.transform;
            target = player;

            if (target == null) return;

            crosshair = GameManager.instance.crosshair.transform;
            // offset = transform.position - target.position;

            // 初始化位置，z=-10
            targetPos = target.position;
            targetPos.z = transform.position.z;

            transform.position = targetPos;
            
            
        }

        private void Update()
        {


            if (target == null) return;

            targetPos = target.position;
            targetPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);

        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            Debug.Log("SetTarget " + target.name);
        }
    }
}
