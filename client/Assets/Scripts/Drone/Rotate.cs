using System;
using UnityEngine;
using DG.Tweening;

namespace Drone
{
    public class Rotate : MonoBehaviour
    {

        private void Start()
        {
            transform.DORotate(new Vector3(0, 180, 0), 5, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart);
        }
        
    }
}