using System;
using AgkUI.Dialog.Service;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable UnusedMember.Local

namespace DronDonDon.Core.UI.Dialog
{
    public class DialogService : MonoBehaviour
    {
        public const string ALERT_DIALOG = "UI/Dialog/Standart/pfAlertDialog@embeded";
        public const string CONFIRM_DIALOG = "UI/Dialog/Standart/pfConfirmDialog@embeded";
        public const string ERROR_DIALOG = "UI/Dialog/Standart/pfErrorDialog@embeded";
        public const string CONNECT_ERROR_DIALOG = "UI/Dialog/Standart/pfConnectErrorDialog@embeded";

        private const string DEFAULT_ERROR_TITLE = "#lblError";
        private const string DEFAULT_ERROR_MESSAGE = "#warnMoreErrorsFound";
        private const string YES_BUTTON_TITLE = "#btnYes";
        private const string NO_BUTTON_TITLE = "#btnNo";

        [Inject]
        private IoCProvider<DialogManager> _dialogManagerProvider;

        public void ShowAlert(string title, string message, [CanBeNull] UnityAction callback = null)
        {
            DialogManager dialogManager = _dialogManagerProvider.Get();
            if (dialogManager == null) {
                return;
            }
            dialogManager.ShowModal<AlertDialog>(title, message, callback);
        }

        [PublicAPI]
        public void ShowSystemError([CanBeNull] string message, [CanBeNull] string errorLog = null, [CanBeNull] UnityAction callback = null)
        {
            DialogManager dialogManager = _dialogManagerProvider.Get();
            if (dialogManager == null) {
                return;
            }
            errorLog = errorLog ?? Environment.StackTrace;
            dialogManager.ShowModal<ErrorDialog>(message ?? DEFAULT_ERROR_MESSAGE, callback, errorLog);
        }

        public void ShowDefaultError([CanBeNull] string message = null, [CanBeNull] UnityAction callback = null)
        {
            ShowSystemError(message, null, callback);
        }

        [PublicAPI]
        public void Confirm(string title, string message, ConfirmDialog.ConfirmDelegate callback)
        {
            Confirm(title, message, YES_BUTTON_TITLE, NO_BUTTON_TITLE, callback);
        }

        [PublicAPI]
        public void Confirm(string title, string message, string yesButtonTitle, string noButtonTitle, ConfirmDialog.ConfirmDelegate callback)
        {
            DialogManager dialogManager = _dialogManagerProvider.Get();
            if (dialogManager == null) {
                return;
            }
            dialogManager.ShowModal<ConfirmDialog>(title, message, yesButtonTitle, noButtonTitle, callback);
        }
    }
}