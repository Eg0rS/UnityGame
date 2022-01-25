using System.Collections.Generic;
using AgkCommons.Extension;
using UnityEngine;
using DG.Tweening;

namespace Components
{
    public class ChipLine : MonoBehaviour
    {
        private List<GameObject> _chipList;

        [SerializeField]
        private float _timeSpin;

        private void Start()
        {
            _chipList = gameObject.GetChildren();
            int i = 0;
            foreach (GameObject chipObject in _chipList) {
                Vector3 rotation = transform.rotation.eulerAngles;
                chipObject.transform.DORotate(new Vector3(rotation.x, 360, rotation.z), _timeSpin, RotateMode.FastBeyond360).SetRelative(true)
                          .SetLoops(-1)
                          .SetEase(Ease.Flash)
                          .SetDelay(0.3f * i);
                i++;
            }
        }
    }
}