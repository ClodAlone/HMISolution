//-------------------------------------------------------------------------------------------------
// <copyright file="RecurringEditConfirmationForm.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Schedule;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// Displays a confirmation dialog for editing a recurring appointment.
    /// </summary>
    public class RecurringEditConfirmationForm : Form
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public RecurringEditConfirmationForm()
        {
            InitializeComponent();

            InitStrings();

            this.button1.Click += new EventHandler(button1_Click);
            this.button2.Click += new EventHandler(button2_Click);
            this.button3.Click += new EventHandler(button3_Click);
            this.button4.Click += new EventHandler(button4_Click);
        }

        #region RightToLeft support

        private static bool ignoreScheduleRTLSettings = false;

        /// <summary>
        /// Gets or sets whether this form will  use the RTL settings from 
        /// the ScheduleControl to decide whether to mirror the form.
        /// </summary>
        public static bool IgnoreScheduleRTLSettings
        {
            get { return ignoreScheduleRTLSettings; }
            set { ignoreScheduleRTLSettings = value; }
        }

        const int WS_EX_LAYOUTRTL = 0x400000;
        const int WS_EX_NOINHERITLAYOUT = 0x100000;
        
        /// <internalonly/>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!base.DesignMode && ScheduleControl.isMirrored && !IgnoreScheduleRTLSettings)
                {
                    cp.ExStyle = cp.ExStyle | WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
                }

                return cp;
            }
        }
        #endregion

        #region button handlers

        void button1_Click(object sender, EventArgs e)
        {
            result = RecurringAppointmentEditAction.ChangeSingleAppointmentOnly;
            Close();
        }

        void button2_Click(object sender, EventArgs e)
        {
            result = RecurringAppointmentEditAction.ChangeAllFutureAppointments;
            Close();
        }

        void button3_Click(object sender, EventArgs e)
        {
            result = RecurringAppointmentEditAction.ChangeAllAppointments;
            Close();
        }

        void button4_Click(object sender, EventArgs e)
        {
            result = RecurringAppointmentEditAction.Cancel;
            Close();
        }

        #endregion

        #region Result return Property

        private RecurringAppointmentEditAction result;

        /// <summary>
        /// A read-only property that gets the edit action specified by the user in this dialog.
        /// </summary>
        /// <remarks>This is the return value to indicate what choice the user made 
        /// in the dialog.</remarks>
        public RecurringAppointmentEditAction Result
        {
            get { return result; }
        }

        #endregion

        #region designer code
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(26, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "This appointment is part of a recurring series of appointments.";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(40, 83);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(297, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Change this single appointment.";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(40, 116);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(297, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Change this appointment and all future appointments.";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(40, 149);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(297, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Change all appointments.";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(40, 183);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(297, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "Cancel";
            // 
            // RecurringEditConfirmationForm
            // 
            this.ClientSize = new System.Drawing.Size(379, 240);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "RecurringEditConfirmationForm";
            this.Text = "RecurringEditConfirmationForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;

        #endregion

        #region Strings

        /// <summary>
        /// Holds various strings used in the AppointmentForm.
        /// </summary>
        /// <remarks>
        /// Modifying the strings in this array will affect the display of the AppointmentForm.
        /// You can use these strings to localize the labels that appear on the form.
        /// </remarks>
        /// <example>
        /// Here are the default values of this string array.
        /// <code lang="C#">
        /// public static string[] DisplayStrings = new string[]
        ///                    {        "This appointment is part of a recurring series of appointments.",  // 0
        ///                             "Change this single appointment.",                                  // 1
        ///                              "Change this appointment and all future appointments.",             // 2
        ///                              "Change all appointments.",                                         // 3  
        ///                              "Cancel",                                                        // 4
        ///                              "Recurring Edit Confirmation"                                     // 5
        ///                    };
        /// </code>
        /// <code lang="VB">
        /// Public Shared DisplayStrings() As String = _
        ///                    {        "This appointment is part of a recurring series of appointments.", _
        ///                             "Change this single appointment.", _
        ///                             "Change this appointment and all future appointments.", _
        ///                             "Change all appointments.",  _
        ///                               "Cancel",  _
        ///                                "Recurring Edit Confirmation" _
        ///                     }
        /// </code>
        /// </example>
        public static string[] DisplayStrings = new string[]
                                    {  
                                        "This appointment is part of a recurring series of appointments.", 
                                        "Change this single appointment.",
                                        "Change this appointment and all future appointments.",
                                        "Change all appointments.",  
                                        "Cancel",  
                                        "Recurring Edit Confirmation"
                                    };

        private const int _This_appointment_is_part_of_a_recurring_series_of_appointments = 0;
        private const int _Change_this_single_appointment = 1;
        private const int _Change_this_appointment_and_all_future_appointments = 2;
        private const int _Change_all_appointments = 3;
        private const int _Cancel = 4;
        private const int _Recurring_Edit_Confirmation = 5;

        private void InitStrings()
        {
            this.label1.Text = DisplayStrings[_This_appointment_is_part_of_a_recurring_series_of_appointments];
            this.button1.Text = DisplayStrings[_Change_this_single_appointment];
            this.button2.Text = DisplayStrings[_Change_this_appointment_and_all_future_appointments];
            this.button3.Text = DisplayStrings[_Change_all_appointments];
            this.button4.Text = DisplayStrings[_Cancel];
            this.Text = DisplayStrings[_Recurring_Edit_Confirmation];
        }

        #endregion
    }
}