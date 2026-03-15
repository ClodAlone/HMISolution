//-------------------------------------------------------------------------------------------------
// <copyright file="CalcQuick.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

namespace Syncfusion.Calculate
{
    /// <summary>
    /// A class that allows you to quickly add calculation support for controls on a form, or usercontrol.
    /// </summary>
    public class CalcQuick: CalcQuickBase
    {
        #region code that handle Controls as key objects

        /// <summary>
        /// A Virtual method that registers an array of controls as formula objects in this CalcQuick instance.
        /// </summary>
        /// <param name="controls">The control array.</param>
        public virtual void RegisterControlArray(Control[] controls)
        {
            foreach (Control c in controls)
            {
                RegisterControl(c);
            }

            base.AutoCalc = true;
        }

        /// <summary>
        /// Used to register a control as a calculation object in this CalcQuick instance.
        /// </summary>
        /// <param name="c">The control to register.</param>
        /// <remarks>
        /// To reference this calculation object from another calculation in this CalcQuick
        /// object, you use the Control.Name string. The value of this calculation object is
        /// bound to the Control.Text property. 
        /// </remarks>
        protected virtual void RegisterControl(Control c)
        {
            ////Subscribe once.
            if (this.NameToControlMap.Count == 0)
            {
                this.ValueSet += new QuickValueSetEventHandler(CalcQuickValueSet);
            }

            if (this.ControlModifiedFlags.ContainsKey(c) ||
                this.NameToControlMap.ContainsKey(c.Name))
            {
                throw new ArgumentException(string.Format(this.Engine.FormulaErrorStrings[this.Engine.already_registered], c.Name));
            }

            this.ControlModifiedFlags.Add(c, false);
            NameToControlMap.Add(c.Name.ToUpper(), c);

            this[c.Name] = c.Text;
            if (c.GetType() == typeof(ComboBox))
            {
                ((ComboBox)c).SelectedIndexChanged += new EventHandler(control_TextChanged);
            }
            else
            {
                c.TextChanged += new EventHandler(control_TextChanged);
            }

            c.Leave += new EventHandler(control_Leave);
        }

        ////Used to autoset the value back to the control.
        private void CalcQuickValueSet(object sender, QuickValueSetEventArgs e)
        {
            if (NameToControlMap.ContainsKey(e.Key))
            {
                Control c = this.NameToControlMap[e.Key] as Control;
                c.Text = e.Value;
            }
        }

        ////When using controls as keys, this event is
        ////used to mark a control as modified.
        private void control_TextChanged(object sender, EventArgs e)
        {
            if (sender != null && !this.ignoreChanges
                && this.ControlModifiedFlags.ContainsKey(sender))
            {
                this.ControlModifiedFlags[sender] = true;
            }
        }

        ////When using controls as keys, this event is 
        ////used to trigger an autochange if needed.
        private void control_Leave(object sender, EventArgs e)
        {
            if (sender != null)
            {
                Control c = sender as Control;
                if (this.ControlModifiedFlags.ContainsKey(c))
                {
                    if ((bool)this.ControlModifiedFlags[sender])
                    {
                        this[c.Name] = c.Text; ////triggers a possible recalculate
                        this.ControlModifiedFlags[c] = false;
                    }
                    ////this.ControlModifiedFlags[c] = false;
                }
            }
        }

        #endregion
    }
}
