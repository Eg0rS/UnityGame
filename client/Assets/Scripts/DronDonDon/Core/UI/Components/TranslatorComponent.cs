using AgkCommons.L10n.Service;
using IoC.Attribute;
using TMPro;
using UnityEngine;

namespace DronDonDon.Core.UI.Components
{
    public class TranslatorComponent : MonoBehaviour
    {
        [Inject]
        private L10nService _l10NService;

        [SerializeField]
        private TextMeshProUGUI _text;

        private void Start()
        {
            _text.text = _l10NService.Message(_text.text);
        }
    }
}