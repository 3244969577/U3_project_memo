using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float lerpSpeed = 1.0f;

        private Vector3 offset;

        private Vector3 targetPos;

        private void Start()
        {
            if (target == null) return;
            offset = transform.position - target.position;
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                target = GameObject.FindGameObjectWithTag("Crosshair").transform;
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                // 获取玩家目标
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }

            if (target == null) return;

            targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);

        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
