#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    internal class RelationshipController : ISharedData
    {
        public void Init(SharedData shared)
        {
            //sourceChanged.Subscribe(SourceChanged);
            //targetChanged.Subscribe(TargetChanged);
        }

        public void SourceChanged(
            IInternalConnector sender,
            IInternalNode oldValue,
            IInternalNode newValue)
        {
            if (oldValue != null)
            {
                if (oldValue.InternalInOutConnectors == null)
                {
                    oldValue.InializeRelationship();
                }
                (oldValue.InternalInOutConnectors as List<IInternalConnector>).Remove(sender);
                (oldValue.InternalOutConnectors as List<IInternalConnector>).Remove(sender);

                if (sender.KnownTargetNode != null)
                {
                    (oldValue.InternalNeighbors as List<IInternalNode>).Remove(sender.KnownTargetNode);
                    (oldValue.InternalOutNeighbors as List<IInternalNode>).Remove(sender.KnownTargetNode);
                    (oldValue.InternalChildren as List<IInternalNode>).Remove(sender.KnownTargetNode);
                    if (newValue == null && (sender.KnownTargetNode.InternalInNeighbors as List<IInternalNode>).Count == 0)
                    {
                        (oldValue.InternalChildren as List<IInternalNode>).Add(sender.KnownTargetNode.InternalInNeighbors.First());
                    }
                    (sender.KnownTargetNode.InternalInNeighbors as List<IInternalNode>).Remove(oldValue);
                    (sender.KnownTargetNode.InternalNeighbors as List<IInternalNode>).Remove(oldValue);
                }
            }

            if (newValue != null)
            {
                if (newValue.InternalInOutConnectors == null)
                {
                    newValue.InializeRelationship();
                }
                (newValue.InternalInOutConnectors as List<IInternalConnector>).Add(sender);
                (newValue.InternalOutConnectors as List<IInternalConnector>).Add(sender);

                if (sender.KnownTargetNode != null)
                {
                    (newValue.InternalNeighbors as List<IInternalNode>).Add(sender.KnownTargetNode);
                    (newValue.InternalOutNeighbors as List<IInternalNode>).Add(sender.KnownTargetNode);
                    (sender.KnownTargetNode.InternalInNeighbors as List<IInternalNode>).Add(newValue);
                    if ((sender.KnownTargetNode.InternalInNeighbors as List<IInternalNode>).Count == 1)
                    {
                        (newValue.InternalChildren as List<IInternalNode>).Add(sender.KnownTargetNode);
                    }
                    (sender.KnownTargetNode.InternalNeighbors as List<IInternalNode>).Add(newValue);
                }
            }
        }

        public void TargetChanged(
            IInternalConnector sender,
            IInternalNode oldValue,
            IInternalNode newValue)
        {
            if (oldValue != null)
            {
                if (oldValue.InternalInOutConnectors == null)
                {
                    oldValue.InializeRelationship();
                }
                (oldValue.InternalInOutConnectors as List<IInternalConnector>).Remove(sender);
                (oldValue.InternalInConnectors as List<IInternalConnector>).Remove(sender);

                if (sender.KnownSourceNode != null)
                {
                    (oldValue.InternalNeighbors as List<IInternalNode>).Remove(sender.KnownSourceNode);
                    (oldValue.InternalInNeighbors as List<IInternalNode>).Remove(sender.KnownSourceNode);
                    (sender.KnownSourceNode.InternalOutNeighbors as List<IInternalNode>).Remove(oldValue);
                    (sender.KnownSourceNode.InternalChildren as List<IInternalNode>).Remove(oldValue);
                    if (newValue == null && (oldValue.InternalInNeighbors as List<IInternalNode>).Count != 0)
                    {
                        (sender.KnownSourceNode.InternalChildren as List<IInternalNode>).Add(oldValue.InternalInNeighbors.First());
                    }
                    (sender.KnownSourceNode.InternalNeighbors as List<IInternalNode>).Remove(oldValue);
                }
            }

            if (newValue != null)
            {
                if (newValue.InternalInOutConnectors == null)
                {
                    newValue.InializeRelationship();
                }
                (newValue.InternalInOutConnectors as List<IInternalConnector>).Add(sender);
                (newValue.InternalInConnectors as List<IInternalConnector>).Add(sender);

                if (sender.KnownSourceNode != null)
                {
                    (newValue.InternalNeighbors as List<IInternalNode>).Add(sender.KnownSourceNode);
                    (newValue.InternalInNeighbors as List<IInternalNode>).Add(sender.KnownSourceNode);
                    (sender.KnownSourceNode.InternalOutNeighbors as List<IInternalNode>).Add(newValue);
                    if ((newValue.InternalInNeighbors as List<IInternalNode>).Count == 1)
                    {
                        (sender.KnownSourceNode.InternalChildren as List<IInternalNode>).Add(newValue);
                    }
                    (sender.KnownSourceNode.InternalNeighbors as List<IInternalNode>).Add(newValue);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
