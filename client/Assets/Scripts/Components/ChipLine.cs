using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;
using NUnit.Framework.Internal;
using Random = System.Random;

namespace Components
{
    public class ChipLine : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _chipList;

        [SerializeField]
        private float _timeSpin;
        private Sequence _sequence;

        private void Awake()
        {
            _sequence = DOTween.Sequence();
        }

        private void Start()
        {
            int i = 0;
            foreach (GameObject chipObject in _chipList) {
                chipObject.transform.DORotate(new Vector3(90, 360, 0), _timeSpin, RotateMode.FastBeyond360)
                          .SetLoops(-1)
                          .SetEase(Ease.Flash)
                          .SetDelay(0.3f * i);
                i++;
            }
        }
    }
}