using UnityEngine;

public class Blinking : MonoBehaviour
{
    private float _time;
    public bool _on =false;
    private bool _active = false;
    void Update()
    {
        _time += Time.deltaTime;
        if (_time>=0.5f)
        {
            _time -= 0.5f;
            if (_on)
            {
                _active = !_active;
                gameObject.SetActive(_active);
            }
        }
    }
}