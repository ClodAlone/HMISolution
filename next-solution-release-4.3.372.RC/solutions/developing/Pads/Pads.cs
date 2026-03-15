using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace Pads
{
    public class PadEventArgs : EventArgs
    {
        public bool requestingPads;
        public bool dialogResult;

        public bool isAlphaNumeric;
        public bool isPassword;
        public String value;
        public double? min;
        public double? max;
        public double x;
        public double y;
        public bool isRelative;
        public String title;
        public string TagDescription;
        public bool ShowTagDescription;
    }

    public static class Pads
    {
        public static event EventHandler<PadEventArgs> PromptPads;
        static void OnPromptPads(PadEventArgs padInfo)
        {
            var e = PromptPads;
            if (e != null)
            {
                e(null, padInfo);
            }
        }

        public static string ShowNumericPad(String value, Window owner, String title = null, double? min = null, double? max = null)
        {
            return ShowNumericPad(-1, -1, false, value, owner, title, min, max);
        }

        public static string ShowNumericPad(double x, double y, bool relative, String value, Window owner, String title = null, double? min = null, double? max = null, string padTagDescription = null, bool padShowTagDescription = false)
        {
            var padInfo = new PadEventArgs
            {
                value = value,
                min = min,
                max = max,
                x = x,
                y = y,
                isRelative = relative,
                title = title,
                TagDescription = padTagDescription,
                ShowTagDescription = padShowTagDescription
            };
            OnPromptPads(padInfo);
            if (padInfo.requestingPads)
            {
                if (padInfo.dialogResult)
                {
                    return null;
                }

                return padInfo.value;
            }

            var pad = new NumericPad(value, min, max, padTagDescription);

            var dialog = new GeneralDialogContent(x, y, relative, true, pad, GeneralDialogButtons.OkCancelButtons) { Owner = owner };
            dialog.Title = /*title ?? */Properties.Resources.PadTitle;
            dialog.Closing += (o, e) =>
            {
                if(dialog.DialogResult == true && pad.CheckErrors(true))
                {
                    e.Cancel = true;
                }
            };

            if (dialog.ShowDialog() != true)
                return null;

            return pad.GetValue();
        }

        public static string ShowAlphaNumericPad(String value, Window owner, String title = null, int? max = null)
        {
            return ShowAlphaNumericPad(-1, -1, false, value, owner, title, max);
        }

        public static string ShowAlphaNumericPad(double x, double y, bool relative, String value, Window owner, String title = null, int? max = null, string padTagDescription = null, bool padShowTagDescription = false)
        {
            var padInfo = new PadEventArgs
            {
                value = value,
                max = max,
                isAlphaNumeric = true,
                x = x,
                y = y,
                isRelative = relative,
                title = title,
                TagDescription = padTagDescription,
                ShowTagDescription = padShowTagDescription
            };
            OnPromptPads(padInfo);
            if (padInfo.requestingPads)
            {
                if (padInfo.dialogResult)
                {
                    return null;
                }

                return padInfo.value;
            }
            
            var pad = new AlphaNumericPad(value, max, padTagDescription);

            var dialog = new GeneralDialogContent(x, y, relative, true, pad, GeneralDialogButtons.OkCancelButtons) { Owner = owner };
            dialog.Title =/* title ?? */Properties.Resources.PadTitle;
            if (dialog.ShowDialog() != true)
                return null;

            return pad.GetValue();
        }

        public static string ShowPasswordPad(String value, Window owner, String title = null, int? max = null)
        {
            return ShowPasswordPad(-1, -1, false, value, owner, title, max);
        }
        public static string ShowPasswordPad(double x, double y, bool relative, String value, Window owner, String title = null, int? max = null, string padTagDescription = null, bool padShowTagDescription = false)
        {
            var padInfo = new PadEventArgs
            {
                value = value,
                max = max,
                isPassword = true,
                isAlphaNumeric = true,
                x = x,
                y = y,
                isRelative = relative,
                title = title,
                TagDescription = padTagDescription,
                ShowTagDescription = padShowTagDescription
            };
            OnPromptPads(padInfo);
            if (padInfo.requestingPads)
            {
                if (padInfo.dialogResult)
                {
                    return null;
                }

                return padInfo.value;
            }

            var pad = new PasswordPad(value, max, padTagDescription);

            var dialog = new GeneralDialogContent(x, y, relative, true, pad, GeneralDialogButtons.OkCancelButtons) { Owner = owner };
            dialog.Title =/* title ?? */Properties.Resources.PadTitle;
            if (dialog.ShowDialog() != true)
                return null;

            return pad.GetValue();
        }

        public static string ShowNumericPasswordPad(String value, Window owner, String title = null, double? min = null, double? max = null)
        {
            return ShowNumericPasswordPad(-1, -1, false, value, owner, title, min, max);
        }
        public static string ShowNumericPasswordPad(double x, double y, bool relative, String value, Window owner, String title = null, double? min = null, double? max = null, string padTagDescription = null, bool padShowTagDescription = false)
        {
            var padInfo = new PadEventArgs()
            {
                value = value,
                min = min,
                max = max,
                isPassword = true,
                x = x,
                y = y,
                isRelative = relative,
                title = title,
                TagDescription = padTagDescription,
                ShowTagDescription = padShowTagDescription
            };
            OnPromptPads(padInfo);
            if (padInfo.requestingPads)
            {
                if (padInfo.dialogResult)
                {
                    return null;
                }

                return padInfo.value;
            }

            var pad = new NumericPasswordPad(value, min, max, padTagDescription);

            var dialog = new GeneralDialogContent(x, y, relative, true, pad, GeneralDialogButtons.OkCancelButtons) { Owner = owner };
            dialog.Title =/* title ?? */Properties.Resources.PadTitle;
            dialog.Closing += (o, e) =>
            {
                if (dialog.DialogResult == true && pad.CheckErrors(true))
                {
                    e.Cancel = true;
                }
            };

            if (dialog.ShowDialog() != true)
                return null;

            return pad.GetValue();
        }

    }
}
