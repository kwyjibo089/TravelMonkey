using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TravelMonkey.Helpers
{
    public static class DialogsHelper
    {
        public enum MessageType
        {
            NetworkError,
            Defined,
            UndefinedError,
            InputError
        }

        static readonly string UndefinedError = "Something went wrong, please try again later.";
        static readonly string NetworkError = "Network Error.";
        static readonly string InputError = " is required.";

        public static void HandleMessageDialog(MessageType error, string message = "")
        {
            switch (error)
            {
                case MessageType.NetworkError:
                    message = "    " + NetworkError + "    ";
                    break;
                case MessageType.UndefinedError:
                    message = "    " + UndefinedError + "    ";
                    break;
                case MessageType.Defined:
                    message = "    " + message + "    ";
                    break;
                case MessageType.InputError:
                    message = "    " + message + InputError + "    ";
                    break;
            }
            UserDialogs.Instance.Toast(new ToastConfig(message)
            .SetBackgroundColor(Color.FromHex("#333333"))
            .SetMessageTextColor(Color.White)
            .SetDuration(TimeSpan.FromSeconds(3))
            .SetPosition(ToastPosition.Bottom)
            );
        }
    }
}
