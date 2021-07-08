using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Binding.Field;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace DronDonDon.Core.UI.Dialog
{
    [UIController(DialogService.CONFIRM_DIALOG)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class ConfirmDialog : MonoBehaviour
    {
        public delegate void ConfirmDelegate(ConfirmResult result);

        [UITextBinding("TitleText")]
        private TextBinding _title;

        [UITextBinding("MessageText")]
        private TextBinding _message;

        [UITextBinding("YesButtonTitle")]
        private TextBinding _yesButtonTitle;

        [UITextBinding("NoButtonTitle")]
        private TextBinding _noButtonTitle;

        [CanBeNull]
        private ConfirmDelegate _callback;

        [Inject]
        private IoCProvider<DialogManager> _dialogProvider;

        [UICreated]
        private void Init(string title, string message, string yesButtonTitle, string noButtonTitle, ConfirmDelegate callback)
        {
            Title = title;
            Message = message;
            YesButtonTitle = yesButtonTitle;
            NoButtonTitle = noButtonTitle;
            _callback = callback;
        }

        [UIOnClick("YesButton")]
        private void OnYesClick()
        {
            _callback?.Invoke(ConfirmResult.YES);
            _dialogProvider.Require()
                           .Hide(this);
        }

        [UIOnClick("NoButton")]
        private void OnNoClick()
        {
            _callback?.Invoke(ConfirmResult.NO);
            _dialogProvider.Require()
                           .Hide(this);
        }

        private string Title
        {
            set { _title.Text = value; }
        }

        private string Message
        {
            set { _message.Text = value; }
        }

        private string YesButtonTitle
        {
            set { _yesButtonTitle.Text = value; }
        }

        private string NoButtonTitle
        {
            set { _noButtonTitle.Text = value; }
        }

        public enum ConfirmResult
        {
            YES,
            NO
        }
    }
}