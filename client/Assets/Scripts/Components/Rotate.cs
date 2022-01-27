using UnityEngine;
using DG.Tweening;

namespace Components
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField]
        private float _twistTime;

        private Tween _rotateTwin;

        private void Start()
        {
            _rotateTwin = transform.DORotate(new Vector3(0, 360, 0), _twistTime, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Flash);
        }

        private void OnDestroy()
        {
            _rotateTwin.Kill();
        }
    }
}