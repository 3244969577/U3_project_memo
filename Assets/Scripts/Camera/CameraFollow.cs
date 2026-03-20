using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        // 依赖
        public Transform crosshair; // 十字准星
        public Transform player;      // 玩家目标


        private Transform target;    // 跟随目标
        public float lerpSpeed = 1.0f;

        private Vector3 offset;

        private Vector3 targetPos;

        private void Start()
        {
            target = player;
            if (target == null) return;
            // offset = transform.position - target.position;
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                target = crosshair;
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                target = player;
            }

            if (target == null) return;

            targetPos = target.position;
            targetPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);

        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
