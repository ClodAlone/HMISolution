using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    }

    static public class Pads
    {
        static public event EventHandler<PadEventArgs> PromptPads;
        static void OnPromptPads(PadEventArgs padInfo)
        {
            var e = PromptPads;
            if (e != null)
            {
                e(null, padInfo);
            }
        }

        static public String ShowNumericPad(String value, Window owner, String title = null, double? min = null, double? max = null)
        {
            return ShowNumericPad(-1, -1, false, value, owner, title, min, max);
        }

        static public String ShowNumericPad(double x, double y, bool relative, String value, Window owner, String title = null, double? min = null, double? max = null)
        {
            var padInfo = new PadEventArgs()
            {
                value = value,
                min = min,
                max = max,
                x = x,
                y = y,
                isRelative = relative,
                title = title
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

            var pad = new NumericPad(value, min, max);

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

        static public String ShowAlphaNumericPad(String value, Window owner, String title = null, int? max = null)
        {
            return ShowAlphaNumericPad(-1, -1, false, value, owner, title, max);
        }

        static public String ShowAlphaNumericPad(double x, double y, bool relative, String value, Window owner, String title = null, int? max = null)
        {
            var padInfo = new PadEventArgs()
            {
                value = value,
                max = max,
                isAlphaNumeric = true,
                x = x,
                y = y,
                isRelative = relative,
                title = title
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

            var pad = new AlphaNumericPad(value, max);

            var dialog = new GeneralDialogContent(x, y, relative, true, pad, GeneralDialogButtons.OkCancelButtons) { Owner = owner };
            dialog.Title =/* title ?? */Properties.Resources.PadTitle;
            if (dialog.ShowDialog() != true)
                return null;

            return pad.GetValue();
        }

        static public String ShowPasswordPad(String value, Window owner, String title = null, int? max = null)
        {
            return ShowPasswordPad(-1, -1, false, value, owner, title, max);
        }
        static public String ShowPasswordPad(double x, double y, bool relative, String value, Window owner, String title = null, int? max = null)
        {
            var padInfo = new PadEventArgs()
            {
                value = value,
                max = max,
                isPassword = true,
                isAlphaNumeric = true,
                x = x,
                y = y,
                isRelative = relative,
                title = title
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

            var pad = new PasswordPad(value, max);

            var dialog = new GeneralDialogContent(x, y, relative, true, pad, GeneralDialogButtons.OkCancelButtons) { Owner = owner };
            dialog.Title =/* title ?? */Properties.Resources.PadTitle;
            if (dialog.ShowDialog() != true)
                return null;

            return pad.GetValue();
        }

        static public String ShowNumericPasswordPad(String value, Window owner, String title = null, double? min = null, double? max = null)
        {
            return ShowNumericPasswordPad(-1, -1, false, value, owner, title, min, max);
        }
        static public String ShowNumericPasswordPad(double x, double y, bool relative, String value, Window owner, String title = null, double? min = null, double? max = null)
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
                title = title
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

            var pad = new NumericPasswordPad(value, min, max);

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
