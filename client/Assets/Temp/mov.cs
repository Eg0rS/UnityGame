using UnityEngine;
using DG.Tweening;

public class mov : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        Time.timeScale = 2f;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.DOLocalPath(new[] {new Vector3(1, 0, 0), new Vector3(-1, 0, 0)}, 3f).SetLoops(-1).SetEase(Ease.Linear);
        // _sequence.Append(_rigidbody.DOLocalPath(new[] {new Vector3(0, 1, 0)}, 1f).SetEase(Ease.Linear))
        //           .Append(_rigidbody.DOLocalPath(new[] {new Vector3(0, 1, 0)}, 1f).SetEase(Ease.Linear))
        //           .OnComplete(() => _sequence.Restart());
    }
}