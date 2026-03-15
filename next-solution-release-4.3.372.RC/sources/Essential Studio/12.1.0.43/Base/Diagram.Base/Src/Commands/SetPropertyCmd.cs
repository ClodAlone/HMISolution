#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Reflection;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Sets the value of a property in one or more nodes.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IPropertyContainer"/>
    /// </remarks>
    public sealed class SetPropertyCmd
        : ICommand
    {
        #region Class members
        private IPropertyContainer m_propertyContainer;
        private string m_strPropertyName;
        private string m_strContainerName;
        private object m_currentPropertyValue;
        private SizeF m_szMoveoffset;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyCmd"/> class.
        /// </summary>
        /// <param name="propertyContainer">The property container.</param>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public SetPropertyCmd(IPropertyContainer propertyContainer, string strPropertyContainerName, string strPropertyName)
            : this(propertyContainer, strPropertyContainerName, strPropertyName, SizeF.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyCmd"/> class.
        /// </summary>
        /// <param name="propertyContainer">The property container.</param>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="szMoveOffset">The move offset.</param>
        public SetPropertyCmd(IPropertyContainer propertyContainer, string strPropertyContainerName, string strPropertyName, SizeF szMoveOffset)
        {
            if (propertyContainer == null)
                throw new ArgumentNullException("nodeAffected");

            if (strPropertyName == null)
                throw new ArgumentNullException("strPropertyName");

            m_propertyContainer = propertyContainer;

            m_strContainerName = strPropertyContainerName;
            m_strPropertyName = strPropertyName;
            m_szMoveoffset = szMoveOffset;

            m_currentPropertyValue = GetOldValue();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <value>The property container.</value>
        public IPropertyContainer PropertyContainer
        {
            get { return m_propertyContainer; }
        }

        /// <summary>
        /// Gets the affected property path.
        /// </summary>
        /// <value>The affected property path.</value>
        public string AffectedPropertyPath
        {
            get { return m_strContainerName; }
        }

        /// <summary>
        /// Gets the affected property.
        /// </summary>
        /// <value>The affected property.</value>
        public string AffectedProperty
        {
            get { return m_strPropertyName; }
        }
        #endregion

        #region ICommand Members
        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return "SetProperty"; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        /// <value></value>
        public bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Determines whether this instance can merge the specified CMD merging.
        /// </summary>
        /// <param name="cmdMerging">The CMD merging.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified CMD merging; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMerge(ICommand cmdMerging)
        {
            if (cmdMerging != null && cmdMerging != this)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Merges the specified command to merge.
        /// </summary>
        /// <param name="cmtToMerge">The command to merge.</param>
        public void Merge(ICommand cmtToMerge)
        {
            commands.Add(cmtToMerge);
        }
      
        /// <summary>
        /// Reverses the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Undo()
        {
            bool success = true;
            
            if (m_strPropertyName == DPN.PinPoint)
            {
                IUnitIndependent node = this.m_propertyContainer as IUnitIndependent;

                if (node != null)
                {
                    PointF ptPin = node.GetPinPoint(MeasureUnits.Pixel);
                    ptPin.X -= m_szMoveoffset.Width;
                    ptPin.Y -= m_szMoveoffset.Height;

                    node.SetPinPoint(ptPin, MeasureUnits.Pixel);
                }
            }
            else
            {
                m_currentPropertyValue = SetValue(m_currentPropertyValue);
            }

            if (commands.Count > 0)
            {
                ICommand cmd;
                
                // foreach( ICommand cmd in cmds )
                for (int nCounter = commands.Count - 1; nCounter >= 0; nCounter--)
                {
                    cmd = (ICommand)this.commands[nCounter];
                    success = cmd.Undo();

                    if (!success)
                    {
                        break;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// List of commands to execute.
        /// </summary>
        private ArrayList commands = new ArrayList();
        #endregion

        #region IVerb Members
        /// <summary>
        /// Does the specified command target.
        /// </summary>
        /// <param name="cmdTarget">The command target.</param>
        /// <returns>true, if do the specified command target.</returns>
        public bool Do(object cmdTarget)
        {
            bool success = true;
            if (m_strPropertyName == DPN.PinPoint)
            {
                IUnitIndependent node = this.m_propertyContainer as IUnitIndependent;

                if (node != null)
                {
                    PointF ptPin = node.GetPinPoint(MeasureUnits.Pixel);
                    ptPin.X += m_szMoveoffset.Width;
                    ptPin.Y += m_szMoveoffset.Height;

                    node.SetPinPoint(ptPin, MeasureUnits.Pixel);
                }
            }
            else
            {
                m_currentPropertyValue = SetValue(m_currentPropertyValue);
            }
            if (commands.Count > 0)
            {
                foreach (ICommand cmd in this.commands)
                {
                    success = cmd.Do(cmdTarget);

                    if (!success)
                    {
                        break;
                    }
                }
            }

            return success;
        }
        #endregion

        #region Class helper methods
        private object SetValue(object newValue)
        {
            object oldValue;
            
            // 1 - get property container
            object propertyContainer;

            if (m_strContainerName == null || m_strContainerName == string.Empty)
            {
                propertyContainer = m_propertyContainer;
            }
            else
            {
                propertyContainer = m_propertyContainer.GetPropertyContainerByName(m_strContainerName);
            }

            // 2 - get property current value
            PropertyInfo infoProperty =
            propertyContainer.GetType().GetProperty(m_strPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            object value = infoProperty.GetValue(propertyContainer, new object[] { });

            // 3 - copy value
            ICloneable clone = value as ICloneable;

            if (clone != null)
            {
                oldValue = clone.Clone();
            }
            else
            {
                oldValue = value;
            }

            // 4 - set value using reflection
            infoProperty.SetValue(propertyContainer, newValue, new object[] { });

            return oldValue;
        }
        private object GetOldValue()
        {
            // 1 - get property container
            object propertyContainer;

            if (m_strContainerName == null || m_strContainerName == string.Empty)
            {
                propertyContainer = m_propertyContainer;
            }
            else
            {
                propertyContainer = m_propertyContainer.GetPropertyContainerByName(m_strContainerName);
            }

            if (propertyContainer == null)
                return null;

            // 2 - get property current value
            PropertyInfo infoProperty =
                propertyContainer.GetType().GetProperty(m_strPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            object valueToReturn = infoProperty.GetValue(propertyContainer, new object[] { });

            // 3 - copy value
            ICloneable clone = valueToReturn as ICloneable;

            if (clone != null)
            {
                valueToReturn = clone.Clone();
            }

            return valueToReturn;
        }
        #endregion
    }
}
