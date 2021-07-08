using System;
using AgkCommons.L10n.Service;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Binding.Field;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using IoC.Attribute;
using JetBrains.Annotations;
using UnityEngine;

namespace DronDonDon.Core.UI.Dialog
{
    [UIController(DialogService.CONNECT_ERROR_DIALOG)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class ConnectErrorDialog : MonoBehaviour
    {

        [Inject]
        private L10nService _l10NService;

        [Inject]
        private DialogManager _dialogManager;
        
        [UITextBinding("TitleText")]
        private TextBinding _titleText;

        [UITextBinding("MessageText")]
        private TextBinding _messageText;


        private Action _onRetry;
        private Action _onCancel;


        [UICreated]
        public void Init([CanBeNull] Action onRetry, [CanBeNull] Action onCancel)
        {
            _onRetry = onRetry;
            _onCancel = onCancel;
            _titleText.Text = _l10NService.Message("#connectionErrorTitle");
            _messageText.Text = _l10NService.Message("#connectionErrorMessage");
        }

        [UIOnClick("RetryButton")]
        private void OnRetryClick()
        {
            _onRetry?.Invoke();
            Close();
        }

        [UIOnClick("CancelButton")]
        private void OnCancelClick()
        {
            _onCancel?.Invoke();
            Close();
        }

        private void Close()
        {
            _dialogManager.Hide(this);
        }
    }
}