#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region	file using directives
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Thrown when trying to add a node with a duplicate name into the model.
    /// </summary>
    public class DuplicateNodeNameException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateNodeNameException"/> class.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <remarks>
        /// A node's name must be unique within the scope of its parent. This
        /// exception is thrown by the model only if it cannot auto-generate
        /// a unique name by appending numeric suffixes to the name.
        /// </remarks>
        public DuplicateNodeNameException(string nodeName)
        {
            this.nodeName = nodeName;
        }

        /// <summary>
        /// Gets node name that is duplicate.
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.nodeName;
            }
        }

        /// <summary>
        /// Returns the error message for this exception.
        /// </summary>
        public override string Message
        {
            get
            {
                string msgFormat = Resources.Strings.Messages.Get("DuplicateNodeName");
                string nodeName = this.nodeName;
                if (this.nodeName == null)
                {
                    nodeName = "{null}";
                }
                return String.Format(msgFormat, nodeName);
            }
        }

        private string nodeName;
    }

    /// <summary>
    /// Thrown when trying to add a layer with a duplicate name into the model.
    /// </summary>
    public class DuplicateLayerNameException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateLayerNameException"/> class.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        public DuplicateLayerNameException(string layerName)
        {
            this.layerName = layerName;
        }

        /// <summary>
        /// Gets layer name that is duplicate.
        /// </summary>
        public string LayerName
        {
            get
            {
                return this.layerName;
            }
        }

        /// <summary>
        /// Returns the error message for this exception.
        /// </summary>
        public override string Message
        {
            get
            {
                string msgFormat = Resources.Strings.Messages.Get("DuplicateLayerName");
                string layerName = this.layerName;
                if (this.layerName == null)
                {
                    layerName = "{null}";
                }
                return String.Format(msgFormat, layerName);
            }
        }

        private string layerName;
    }

    /// <summary>
    /// Indicates that the X vector in a slope calculation is 0.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Slope is undefined for vertical lines. This exception is thrown when attempting
    /// to calculate the slope of a vertical line.
    /// </para>
    /// </remarks>
    public class SlopeUndefinedException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlopeUndefinedException"/> class.
        /// </summary>
        public SlopeUndefinedException()
        {
        }
    }

    /// <summary>
    /// Thrown when an invalid layer name is referenced.
    /// </summary>
    public class LayerNotFoundException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerNotFoundException"/> class.
        /// </summary>
        /// <param name="layerName">Layer name that is invalid.</param>
        public LayerNotFoundException(string layerName)
        {
            this.layerName = new object[] { layerName };
        }

        /// <summary>
        /// Returns a message describing the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                string msgFormat = Resources.Strings.Messages.Get("LayerNotFound");
                return String.Format(msgFormat, this.layerName);
            }
        }

        private object[] layerName;
    }

    /// <summary>
    /// Thrown when attempting to perform an operation on a disabled layer.
    /// </summary>
    public class LayerDisabledException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerDisabledException"/> class.
        /// </summary>
        /// <param name="layer">Layer that is disabled.</param>
        public LayerDisabledException(Layer layer)
        {
            this.layer = layer;
        }

        /// <summary>
        /// Gets disabled layer involved in the attempted operation.
        /// </summary>
        /// <value>The layer.</value>
        public Layer Layer
        {
            get
            {
                return this.layer;
            }
        }

        /// <summary>
        /// Returns a message describing the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                string msgFormat = Resources.Strings.Messages.Get("LayerDisabled");
                return String.Format(msgFormat, this.layer.Name);
            }
        }

        private Layer layer = null;
    }

    /// <summary>
    /// Thrown when an invalid label name is referenced.
    /// </summary>
    public class LabelNotFoundException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LabelNotFoundException"/> class.
        /// </summary>
        /// <param name="labelName">Label name that is invalid.</param>
        public LabelNotFoundException(string labelName)
        {
            this.labelName = new object[] { labelName };
        }

        /// <summary>
        /// Returns a message describing the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                string msgFormat = Resources.Strings.Messages.Get("LabelNotFound");
                return String.Format(msgFormat, this.labelName);
            }
        }

        private object[] labelName;
    }

    /// <summary>
    /// Thrown when a node is moved or sized to a location that violates its
    /// boundary constraints.
    /// </summary>
    public class BoundaryConstraintException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundaryConstraintException"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public BoundaryConstraintException(INode node)
        {
            this.node = node;
        }

        /// <summary>
        /// Gets node that violated its boundary constraints.
        /// </summary>
        public INode Node
        {
            get
            {
                return this.node;
            }
        }

        /// <summary>
        /// Returns a message describing the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                string msgFormat = Resources.Strings.Messages.Get("BoundaryConstraint");

                string nodeName;
                if (this.node != null)
                {
                    nodeName = this.node.Name;
                }
                else
                {
                    nodeName = "{null}";
                }

                return String.Format(msgFormat, nodeName);
            }
        }

        private INode node = null;
    }

    /// <summary>
    /// Provides a default handler for ThreadExceptions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A thread exception is generated when unhandled exceptions occur on a thread.
    /// This is a last resort exception handler that allows your application to
    /// gracefully deal with unhandled exceptions.
    /// </para>
    /// </remarks>
    public class DefaultExceptionHandler
    {
        /// <summary>
        /// Singleton instance of the DefaultExceptionHandler.
        /// </summary>
        public static DefaultExceptionHandler Singleton = new DefaultExceptionHandler();

        /// <summary>
        /// Called when a thread exception occurs.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="evtArgs">Event arguments.</param>
        public void OnThreadException(object sender, ThreadExceptionEventArgs evtArgs)
        {
            System.Exception ex = evtArgs.Exception;

            if (ex.GetType().Equals(typeof(LayerDisabledException)))
            {
                this.ShowUserErrorDialog(ex);
            }
            else if (ex.GetType().Equals(typeof(BoundaryConstraintException)))
            {
                this.ShowUserErrorDialog(ex);
            }
            else
            {
                if (this.ShowApplicationErrorDialog(ex))
                {
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// Shows a model dialog that displays a user error message.
        /// </summary>
        /// <param name="ex">Exception thrown by the error.</param>
        public void ShowUserErrorDialog(System.Exception ex)
        {
            string dlgCaption = Resources.Strings.Captions.Get("UserErrorDialog");
            MessageBox.Show(ex.Message, dlgCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        /// <summary>
        /// Shows the application error dialog.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/>.</param>
        /// <returns>true, if show application error dialog.</returns>
        public bool ShowApplicationErrorDialog(System.Exception ex)
        {
            bool exitApp = false;

            string dlgCaption = Resources.Strings.Captions.Get("SystemErrorDialog");
            string msgFmt = Resources.Strings.Messages.Get("SystemErrorDialog");
            string msgText = String.Format(msgFmt, ex.Message);
            if (MessageBox.Show(msgText, dlgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Error) != DialogResult.Yes)
            {
                exitApp = true;
            }

            return exitApp;
        }
    }

    /// <summary>
    /// Class for script file not found exception.
    /// </summary>
    public class ScriptFileNotFoundException : System.Exception
    {
        #region	Class constants
        /// <summary>
        /// Default message.
        /// </summary>
        private const string DEF_MESSAGE = @"Script	file not found.";
        #endregion

        #region	Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileNotFoundException"/> class.
        /// </summary>
        public ScriptFileNotFoundException()
            : this(DEF_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileNotFoundException"/> class.
        /// </summary>
        /// <param name="innerExc">The inner exception.</param>
        public ScriptFileNotFoundException(Exception innerExc)
            : this(DEF_MESSAGE, innerExc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ScriptFileNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerExc">The inner exception.</param>
        public ScriptFileNotFoundException(string message, Exception innerExc)
            : base(message, innerExc)
        {
        }
        #endregion
    }
}