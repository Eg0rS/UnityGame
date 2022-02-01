using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace AgkUI.Element.Buttons
{
    [PublicAPI]
    public class ToggleButton : UIBehaviour, IPointerClickHandler
    {
        public ButtonClickedEvent OnClick { get; set; } = new ButtonClickedEvent();
        [SerializeField]
        [CanBeNull]
        private Image _onImage;
        [SerializeField]
        [CanBeNull]
        private Image _offImage;

        [SerializeField]
        private float _slideTime = 0.25f;
        [SerializeField]
        private Color _disabledColor = new Color(0.78f, 0.78f, 0.78f);
        [SerializeField]
        private bool _isOn;

        [CanBeNull]
        private Image _image;
        private Color _enabledColor;
        private bool _interactable = true;

        protected override void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            if (_image != null) {
                _enabledColor = _image.color;
            }
        }

        public void PlaySwitchAnimation(bool immediately = false)
        {
            PlaySwitchAnimation(_onImage, 1, 0, immediately);
            PlaySwitchAnimation(_offImage, 0, 1, immediately);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Interactable) {
                return;
            }

            _isOn = !_isOn;
            PlaySwitchAnimation(true);
            OnClick.Invoke();
        }

        private void PlaySwitchAnimation([CanBeNull] Image image, float onOpacity, float offOpacity, bool immediately)
        {
            if (image == null) {
                return;
            }

            DOTween.Kill(image);
            if (immediately) {
                image.color = new Color(image.color.r, image.color.g, image.color.b, _isOn ? onOpacity : offOpacity);
            } else {
                image.material.DOFade(_isOn ? onOpacity : offOpacity, _slideTime);
            }
        }

        public bool Interactable
        {
            get { return _interactable; }
            set
            {
                _interactable = value;
                if (_image != null) {
                    _image.color = _interactable ? _enabledColor : _disabledColor;
                }
            }
        }

        [PublicAPI]
        public bool IsOn
        {
            get { return _isOn; }
            set
            {
                if (_isOn == value) {
                    return;
                }
                _isOn = value;
                PlaySwitchAnimation(true);
            }
        }
    }
}