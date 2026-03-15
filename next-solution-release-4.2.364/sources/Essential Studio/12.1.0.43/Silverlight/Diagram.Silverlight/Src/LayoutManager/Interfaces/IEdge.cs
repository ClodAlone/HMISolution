#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interface to a Connection in a hierarchy or graph of objects.
    /// </summary>
    public interface IEdge : ICommon, INotifyPropertyChanged
    {
        #region  Properties
#if WPF
        /// <summary>
        /// Gets or sets the head decorator angle.
        /// </summary>
        /// <value>The head decorator angle.</value>
        double HeadDecoratorAngle { get; set; }

        /// <summary>
        /// Gets or sets the tail decorator angle.
        /// </summary>
        /// <value>The tail decorator angle.</value>
        double TailDecoratorAngle { get; set; }
#endif
        /// <summary>
        /// Gets or sets the connection head port.
        /// </summary>
        /// <value>The connection head port.</value>
        ConnectionPort ConnectionHeadPort { get; set; }

        /// <summary>
        /// Gets or sets the connection tail port.
        /// </summary>
        /// <value>The connection tail port.</value>
        ConnectionPort ConnectionTailPort { get; set; }

        /// <summary>
        /// Gets or sets the head node.
        /// </summary>
        /// <value>The head node.</value>
        IShape HeadNode { get; set; }

        /// <summary>
        /// Gets or sets the tail node.
        /// </summary>
        /// <value>The tail node.</value>
        IShape TailNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is directed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directed; otherwise, <c>false</c>.
        /// </value>
        bool IsDirected { get; set; }
              
        /// <summary>
        /// Gets or sets the type of the connector.
        /// </summary>
        /// <value>The type of the connector.</value>
        ConnectorType ConnectorType { get; set; }

        /// <summary>
        /// Gets or sets the head decorator position.
        /// </summary>
        /// <value>The head decorator position.</value>
        Point HeadDecoratorPosition { get; set; }

        /// <summary>
        /// Gets or sets the tail decorator position.
        /// </summary>
        /// <value>The tail decorator position.</value>
        Point TailDecoratorPosition { get; set; }        

        /// <summary>
        /// Gets or sets the head decorator shape.
        /// </summary>
        /// <value>The head decorator shape.</value>
        DecoratorShape HeadDecoratorShape { get; set; }

        /// <summary>
        /// Gets or sets the tail decorator shape.
        /// </summary>
        /// <value>The tail decorator shape.</value>
        DecoratorShape TailDecoratorShape { get; set; }

        /// <summary>
        /// Gets or sets the head decorator style.
        /// </summary>
        /// <value>The head decorator style.</value>
        DecoratorStyle HeadDecoratorStyle { get; set; }

        /// <summary>
        /// Gets or sets the tail decorator style.
        /// </summary>
        /// <value>The tail decorator style.</value>
        DecoratorStyle TailDecoratorStyle { get; set; }

        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        /// <value>The line style.</value>
        LineStyle LineStyle { get; set; }

        /// <summary>
        /// Given a Node upon which this Edge is incident, the opposite incident
        /// Node is returned. Throws an exception if the input node is not incident
        /// on this Edge.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The node at the other end.</returns>
        IShape AdjacentNode(IShape node);

        #endregion
    }
}
