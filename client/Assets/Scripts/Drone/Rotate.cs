using System;
using UnityEngine;
using DG.Tweening;

namespace Drone
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] public float speed = 0;

        private void Start()
        {
           // transform.do
        }

        void Update()
        {
            transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self); 
        }
    }
}