using DronDonDon.Core.UI.Overlay;
using UnityEngine;

namespace DronDonDon.Core
{
    public class OverlayManager : MonoBehaviour
    {
        private int _lockCount;
        private PreloaderOverlay _preloaderOverlay;
        
        private void Awake()
        {
            _preloaderOverlay = FindObjectOfType<PreloaderOverlay>();
        }

        public void HideLoadingOverlay(bool removePreloaderAfterComplete = false)
        {
            // ReSharper disable once UseNullPropagation
            if (!ReferenceEquals(_preloaderOverlay, null)) {
                _preloaderOverlay.Complete(removePreloaderAfterComplete);
            }
        }

        public void ShowPreloader()
        {
            PreloaderOverlay.Show();
        }

        public void HidePreloader()
        {
            PreloaderOverlay.Hide();
        }

        private PreloaderOverlay PreloaderOverlay
        {
            get
            {
                if (_preloaderOverlay != null) {
                    return _preloaderOverlay;
                }
                GameObject prefab = Resources.Load("Embeded/Preloader/pfPreloader", typeof(GameObject)) as GameObject;
                GameObject go = Instantiate(prefab, transform);
                _preloaderOverlay = go.GetComponent<PreloaderOverlay>();
                return _preloaderOverlay;
            }
        }
    }
}