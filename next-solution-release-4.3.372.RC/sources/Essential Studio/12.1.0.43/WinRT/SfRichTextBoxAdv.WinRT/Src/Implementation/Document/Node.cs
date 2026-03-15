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
#if WPF
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public interface INode
    {
        #region Properties
        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        Node Owner
        {
            get;
        }
        #endregion
    }
    public interface ICompositeNode
    {
        #region Properties
        NodeCollection ChildNodes
        {
            get;
        }
        #endregion
    }
    public abstract class BaseNode : DependencyObject
    {
        #region Fields
        BaseNode ownerBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the owner base.
        /// </summary>
        /// <value>
        /// The owner base.
        /// </value>
        internal BaseNode OwnerBase
        {
            get
            {
                return ownerBase;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNode"/> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        internal BaseNode(BaseNode baseNode)
        {
            ownerBase = baseNode;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Sets the owner base.
        /// </summary>
        /// <param name="owner">The base node.</param>
        internal virtual void SetOwner(BaseNode owner)
        {
            ownerBase = owner;
        }
        /// <summary>
        /// Sets the owner base.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void SetOwnerBase(BaseNode owner)
        {
            ownerBase = owner;
        }
        #endregion
    }
    public abstract class Node : BaseNode, INode
    {
        #region Properties
        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public Node Owner
        {
            get
            {
                return OwnerBase as Node;
            }
        }
        /// <summary>
        /// Gets the next node.
        /// </summary>
        /// <value>
        /// The next node.
        /// </value>
        public Node NextNode
        {
            get
            {
                CompositeNode compositeNode = Owner as CompositeNode;
                return compositeNode != null ? compositeNode.ChildNodes.GetNextNode(this) : null;
            }
        }
        /// <summary>
        /// Gets the previous node.
        /// </summary>
        /// <value>
        /// The previous node.
        /// </value>
        public Node PreviousNode
        {
            get
            {
                CompositeNode compositeNode = Owner as CompositeNode;
                return compositeNode != null ? compositeNode.ChildNodes.GetPreviousNode(this) : null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is composite.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is composite; otherwise, <c>false</c>.
        /// </value>
        internal bool IsComposite
        {
            get
            {
                return this is CompositeNode;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Node" /> class.
        /// </summary>
        /// <param name="ownerNode">The owner node.</param>
        public Node(Node ownerNode)
            : base(ownerNode)
        {
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Sets the owner base.
        /// </summary>
        /// <param name="owner">The base node.</param>
        internal override void SetOwner(BaseNode owner)
        {
            if (OwnerBase != owner && OwnerBase is CompositeNode)
            {
                if (this is FieldCharacterAdv)
                {
                    if ((this as FieldCharacterAdv).IsLinkedFieldCharacter())
                        (this as FieldCharacterAdv).UnlinkFieldCharacter("", false);
                    //Handles removal of field character from existing owner node.
                    (OwnerBase as CompositeNode).RemoveFieldCharacter(this as FieldCharacterAdv);
                    (this as FieldCharacterAdv).CheckUnlinkFieldCharacter("", false);
                }
                if ((OwnerBase as CompositeNode).ChildNodes.Contains(this))
                    (OwnerBase as CompositeNode).ChildNodes.Remove(this);
            }
            SetOwnerBase(owner);
            if (owner is CompositeNode)
            {
                if (this is FieldCharacterAdv)
                    //Handles insertion of field character to new owner node.
                    (owner as CompositeNode).AttachFieldCharacter(this as FieldCharacterAdv);
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the index in owner collection.
        /// </summary>
        /// <returns></returns>
        internal int GetIndexInOwnerCollection()
        {
            CompositeNode compositeNode = Owner as CompositeNode;
            return compositeNode != null ? compositeNode.ChildNodes.IndexOf(this) : -1;
        }
        /// <summary>
        /// Gets the top level owner.
        /// </summary>
        /// <returns></returns>
        internal Node GetTopLevelOwner()
        {
            Node node = this;
            while (node.Owner is Node)
            {
                node = node.Owner;
                if (node is DocumentAdv)
                    break;
            }
            return node;
        }
        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <returns></returns>
        internal DocumentAdv GetOwnerDocument()
        {
            Node node = GetTopLevelOwner();
            return node as DocumentAdv;
        }
        #endregion
    }

    public abstract class CompositeNode : Node
    {
        #region Fields
        private NodeCollection nodeCollection;
        internal List<FieldBeginAdv> Fields;
        internal List<FieldCharacterAdv> FieldCharacters;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        /// <value>
        /// The child nodes.
        /// </value>
        internal NodeCollection ChildNodes
        {
            get
            {
                return nodeCollection;
            }
            set
            {
                nodeCollection = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeNode"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public CompositeNode(Node owner)
            : base(owner)
        {
            FieldCharacters = new List<FieldCharacterAdv>();
            Fields = new List<FieldBeginAdv>();
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Sets the owner base.
        /// </summary>
        /// <param name="owner">The base node.</param>
        internal override void SetOwner(BaseNode owner)
        {
            if (OwnerBase != owner && OwnerBase is CompositeNode)
            {
                if (Fields.Count > 0)
                    (OwnerBase as CompositeNode).RemoveField(this);
                if (FieldCharacters.Count > 0)
                    //Handles removal of field character from existing owner node.
                    (OwnerBase as CompositeNode).RemoveFieldCharacter(this);
                if ((OwnerBase as CompositeNode).ChildNodes.Contains(this))
                    (OwnerBase as CompositeNode).ChildNodes.Remove(this);
            }
            SetOwnerBase(owner);
            if (owner is CompositeNode)
            {
                if (FieldCharacters.Count > 0)
                    //Handles insertion of field character to new owner node.
                    (owner as CompositeNode).AttachFieldCharacter(this);
                if (Fields.Count > 0)
                    (owner as CompositeNode).AddField(this);
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Clears the unlinked fields.
        /// </summary>
        internal void ClearUnlinkedFields()
        {
            for (int i = 0; i < FieldCharacters.Count; i++)
            {
                FieldCharacterAdv fieldCharacter = FieldCharacters[i];
                fieldCharacter.OwnerParagraph.Inlines.Remove(fieldCharacter);
                FieldCharacters.Remove(fieldCharacter);
                i--;
            }
            DocumentAdv document = GetOwnerDocument();
            if (document != null)
            {
                SfRichTextBoxAdv richTextBoxAdv = document.OwnerControl;
                if (richTextBoxAdv != null)
                {
                    if (!richTextBoxAdv.IsDocumentLoaded && richTextBoxAdv.IsControlLoaded && !richTextBoxAdv.IsLayoutEnabled)
                    {
                        richTextBoxAdv.IsLayoutEnabled = true;
                        //Restarts lay outing from holded block.
                        if (richTextBoxAdv.Viewer.FieldToLayout != null)
                            richTextBoxAdv.Viewer.FieldToLayout.OwnerParagraph.Relayout(richTextBoxAdv.Viewer.FieldToLayout.GetIndexInOwnerCollection());
                    }
                }
            }
        }
        /// <summary>
        /// Gets the field code.
        /// </summary>
        /// <param name="fieldBegin">The field begin.</param>
        /// <param name="isNestedFieldCode">if set to <c>true</c> [is nested field code].</param>
        /// <param name="isFieldCodeParsed">if set to <c>true</c> [is field code parsed].</param>
        /// <param name="previousNode">The previous node.</param>
        /// <returns></returns>
        internal string GetFieldCode(FieldBeginAdv fieldBegin, ref bool isNestedFieldCode, ref bool isFieldCodeParsed, Node previousNode)
        {
            string fieldCode = "";
            int index = 0;
            if (ChildNodes.Contains(previousNode))
                index = ChildNodes.IndexOf(previousNode) + 1;
            for (int i = index; i < ChildNodes.Count; i++)
            {
                Node childNode = ChildNodes[i];
                if (childNode is CompositeNode)
                {
                    fieldCode += (childNode as CompositeNode).GetFieldCode(fieldBegin, ref isNestedFieldCode, ref isFieldCodeParsed, null);
                    if (isFieldCodeParsed)
                        return fieldCode;
                }
                else
                {
                    if (childNode is FieldCharacterAdv)
                    {
                        if (childNode is FieldBeginAdv)
                            isNestedFieldCode = (fieldBegin.FieldEnd != null && fieldBegin != childNode);
                        if (childNode is FieldEndAdv)
                        {
                            if (fieldBegin.FieldEnd == childNode)
                            {
                                isFieldCodeParsed = true;
                                return fieldCode;
                            }
                            else
                                isNestedFieldCode = false;
                        }
                        else if (childNode is FieldSeparatorAdv)
                        {
                            if (fieldBegin.FieldSeparator == childNode)
                            {
                                isFieldCodeParsed = true;
                                return fieldCode;
                            }
                            else
                                isNestedFieldCode = false;
                        }
                    }
                    else if (childNode is SpanAdv && !isNestedFieldCode)
                        fieldCode += (childNode as SpanAdv).Text;
                }
            }
            if (Owner is CompositeNode && !(this is HeaderFooter))
                fieldCode += (Owner as CompositeNode).GetFieldCode(fieldBegin, ref isNestedFieldCode, ref isFieldCodeParsed, this);
            return fieldCode;
        }
        /// <summary>
        /// Links the field traversing forward.
        /// </summary>
        /// <param name="fieldBegin">The field begin.</param>
        /// <param name="fieldNestingLevel">The field nesting level.</param>
        /// <param name="previousNode">The previous node.</param>
        internal bool LinkFieldTraversingForward(FieldBeginAdv fieldBegin, ref int fieldNestingLevel, Node previousNode)
        {
            int index = 0;
            if (ChildNodes.Contains(previousNode))
                index = ChildNodes.IndexOf(previousNode) + 1;
            for (int i = index; i < ChildNodes.Count; i++)
            {
                Node childNode = ChildNodes[i];
                if (childNode is CompositeNode)
                {
                    bool isEndReached = (childNode as CompositeNode).LinkFieldTraversingForward(fieldBegin, ref fieldNestingLevel, null);
                    if (fieldBegin.FieldEnd != null || isEndReached)
                        return true;
                }
                else
                {
                    if (childNode is FieldCharacterAdv)
                    {
                        if (childNode is FieldBeginAdv && fieldBegin != childNode)
                            fieldNestingLevel++;
                        if (childNode is FieldEndAdv)
                        {
                            if (fieldNestingLevel == 0)
                            {
                                if ((childNode as FieldEndAdv).FieldBegin == null)
                                    fieldBegin.FieldEnd = childNode as FieldEndAdv;
                                return fieldBegin.FieldEnd != null;
                            }
                            else
                                fieldNestingLevel--;
                        }
                        else if (fieldNestingLevel == 0 && fieldBegin.FieldSeparator == null)
                        {
                            if ((childNode as FieldSeparatorAdv).FieldBegin == null)
                            {
                                fieldBegin.FieldSeparator = childNode as FieldSeparatorAdv;
                                if ((childNode as FieldSeparatorAdv).FieldEnd != null)
                                {
                                    fieldBegin.FieldEnd = (childNode as FieldSeparatorAdv).FieldEnd;
                                    return true;
                                }
                            }
                            else
                                return false;
                        }
                    }
                }
            }
            if (Owner is CompositeNode && !(this is HeaderFooter))
                return (Owner as CompositeNode).LinkFieldTraversingForward(fieldBegin, ref fieldNestingLevel, this);
            else
                return true;
        }
        /// <summary>
        /// Links the field traversing backward.
        /// </summary>
        /// <param name="fieldEnd">The field end.</param>
        /// <param name="fieldNestingLevel">The field nesting level.</param>
        /// <param name="previousNode">The previous node.</param>
        internal bool LinkFieldTraversingBackward(FieldEndAdv fieldEnd, ref int fieldNestingLevel, Node previousNode)
        {
            int index = ChildNodes.Count - 1;
            if (ChildNodes.Contains(previousNode))
                index = ChildNodes.IndexOf(previousNode) - 1;
            for (int i = index; i >= 0; i--)
            {
                Node childNode = ChildNodes[i];
                if (childNode is CompositeNode)
                {
                    bool isDocumentStartReached = (childNode as CompositeNode).LinkFieldTraversingBackward(fieldEnd, ref fieldNestingLevel, null);
                    if (fieldEnd.FieldBegin != null || isDocumentStartReached)
                        return true;
                }
                else
                {
                    if (childNode is FieldCharacterAdv)
                    {
                        if (childNode is FieldEndAdv && fieldEnd != childNode)
                            fieldNestingLevel++;
                        if (childNode is FieldBeginAdv)
                        {
                            if (fieldNestingLevel == 0)
                            {
                                if ((childNode as FieldBeginAdv).FieldEnd == null)
                                    fieldEnd.FieldBegin = childNode as FieldBeginAdv;
                                return fieldEnd.FieldBegin != null;
                            }
                            else
                                fieldNestingLevel--;
                        }
                        else if (fieldNestingLevel == 0)
                        {
                            if ((childNode as FieldSeparatorAdv).FieldEnd == null)
                            {
                                fieldEnd.FieldSeparator = childNode as FieldSeparatorAdv;
                                if ((childNode as FieldSeparatorAdv).FieldBegin != null)
                                {
                                    fieldEnd.FieldBegin = (childNode as FieldSeparatorAdv).FieldBegin;
                                    return true;
                                }
                            }
                            else
                                return false;
                        }
                    }
                }
            }
            if (Owner is CompositeNode && !(this is HeaderFooter))
                return (Owner as CompositeNode).LinkFieldTraversingBackward(fieldEnd, ref fieldNestingLevel, this);
            else
                return true;
        }
        /// <summary>
        /// Links the field traversing backward.
        /// </summary>
        /// <param name="fieldSeparator">The field separator.</param>
        /// <param name="fieldNestingLevel">The field nesting level.</param>
        /// <param name="previousNode">The previous node.</param>
        internal bool LinkFieldTraversingBackward(FieldSeparatorAdv fieldSeparator, ref int fieldNestingLevel, Node previousNode)
        {
            int index = ChildNodes.Count - 1;
            if (ChildNodes.Contains(previousNode))
                index = ChildNodes.IndexOf(previousNode) - 1;
            for (int i = index; i >= 0; i--)
            {
                Node childNode = ChildNodes[i];
                if (childNode is CompositeNode)
                {
                    bool isDocumentStartReached = (childNode as CompositeNode).LinkFieldTraversingBackward(fieldSeparator, ref fieldNestingLevel, null);
                    if (fieldSeparator.FieldBegin != null || isDocumentStartReached)
                        return true;
                }
                else
                {
                    if (childNode is FieldCharacterAdv)
                    {
                        if (childNode is FieldEndAdv)
                            fieldNestingLevel++;
                        if (childNode is FieldBeginAdv)
                        {
                            if (fieldNestingLevel == 0)
                            {
                                if ((childNode as FieldBeginAdv).FieldSeparator == null)
                                    fieldSeparator.FieldBegin = childNode as FieldBeginAdv;
                                return fieldSeparator.FieldBegin != null;
                            }
                            else
                                fieldNestingLevel--;
                        }
                        else if (fieldNestingLevel == 0)
                            return false;
                    }
                }
            }
            if (Owner is CompositeNode && !(this is HeaderFooter))
                return (Owner as CompositeNode).LinkFieldTraversingBackward(fieldSeparator, ref fieldNestingLevel, this);
            else
                return true;
        }
        /// <summary>
        /// Links the field characters.
        /// </summary>
        /// <param name="fieldCharacter">The field character.</param>
        /// <param name="previousNode">The previous node.</param>
        /// <returns></returns>
        internal bool LinkFieldCharacters(FieldCharacterAdv fieldCharacter, Node previousNode)
        {
            int fieldNestingLevel = 0;
            if (fieldCharacter is FieldBeginAdv)
            {
                FieldBeginAdv fieldBegin = fieldCharacter as FieldBeginAdv;
                if (fieldBegin.FieldEnd == null)
                    LinkFieldTraversingForward(fieldBegin, ref fieldNestingLevel, previousNode);
                if (fieldBegin.FieldEnd != null)
                {
                    //Removes the field begin, separator, end characters and inserts linked field to Fields collection.
                    fieldBegin.AddToLinkedFields();
                    return true;
                }
                if (!fieldBegin.HasFieldEnd)
                {
                    DocumentAdv document = GetOwnerDocument();
                    //Check if field character is attached.
                    if (document != null)
                    {
                        SfRichTextBoxAdv richTextBoxAdv = document.OwnerControl;
                        if (richTextBoxAdv != null)
                        {
                            //Sets to hold lay outing this instance.
                            if (!richTextBoxAdv.IsDocumentLoaded && richTextBoxAdv.IsControlLoaded && richTextBoxAdv.IsLayoutEnabled)
                            {
                                richTextBoxAdv.Viewer.FieldToLayout = fieldBegin;
                                richTextBoxAdv.IsLayoutEnabled = false;
                            }
                        }
                    }
                }
            }
            else if (fieldCharacter is FieldSeparatorAdv)
            {
                FieldSeparatorAdv fieldSeparator = fieldCharacter as FieldSeparatorAdv;
                //Links the field begin for the current separator.
                if (fieldSeparator.FieldBegin == null)
                    LinkFieldTraversingBackward(fieldSeparator, ref fieldNestingLevel, previousNode);
                if (fieldSeparator.FieldBegin != null)
                {
                    //Links to field end traversing from field separator.
                    if (fieldSeparator.FieldEnd == null)
                        LinkFieldTraversingForward(fieldSeparator.FieldBegin, ref fieldNestingLevel, previousNode);
                    if (fieldSeparator.FieldEnd != null)
                    {
                        fieldSeparator.FieldBegin.AddToLinkedFields();
                        return true;
                    }
                }
            }
            else if (fieldCharacter is FieldEndAdv)
            {
                FieldEndAdv fieldEnd = fieldCharacter as FieldEndAdv;
                //Links the field begin and separator for the current end.
                if (fieldEnd.FieldBegin == null)
                    LinkFieldTraversingBackward(fieldEnd, ref fieldNestingLevel, previousNode);
                if (fieldEnd.FieldBegin != null)
                {
                    fieldEnd.FieldBegin.AddToLinkedFields();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Attaches the field character.
        /// </summary>
        /// <param name="fieldCharacter">The field character.</param>
        internal void AttachFieldCharacter(FieldCharacterAdv fieldCharacter)
        {
            if (LinkFieldCharacters(fieldCharacter, fieldCharacter))
            {
                //Handles lay outing the contents from field begin.
            }
            else
            {
                AddFieldCharacter(fieldCharacter);
            }
        }
        /// <summary>
        /// Attaches the field character.
        /// </summary>
        /// <param name="compositeNode">The composite node.</param>
        internal void AttachFieldCharacter(CompositeNode compositeNode)
        {
            if (compositeNode is HeaderFooter)
                return;
            for (int i = 0; i < compositeNode.FieldCharacters.Count; i++)
            {
                int count = compositeNode.FieldCharacters.Count;
                FieldCharacterAdv fieldCharacter = compositeNode.FieldCharacters[i];
                if (LinkFieldCharacters(fieldCharacter, compositeNode))
                {
                    //Handles lay outing the contents from field begin.
                    if (count > compositeNode.FieldCharacters.Count)
                        i -= count - compositeNode.FieldCharacters.Count;
                    if (i < -1)
                        i = -1;
                }
                else
                {
                    AddFieldCharacter(fieldCharacter);
                }
            }
        }
        /// <summary>
        /// Adds the field character.
        /// </summary>
        /// <param name="fieldCharacter">The fieldCharacter.</param>
        internal void AddFieldCharacter(FieldCharacterAdv fieldCharacter)
        {
            FieldCharacters.Add(fieldCharacter);
            if (Owner is CompositeNode && !(this is HeaderFooter))
                (Owner as CompositeNode).AddFieldCharacter(fieldCharacter);
        }
        /// <summary>
        /// Adds the field character.
        /// </summary>
        /// <param name="compositeNode">The compositeNode.</param>
        internal void AddFieldCharacter(CompositeNode compositeNode)
        {
            if (compositeNode is HeaderFooter)
                return;
            foreach (FieldCharacterAdv fieldCharacter in compositeNode.FieldCharacters)
            {
                AddFieldCharacter(fieldCharacter);
            }
        }
        /// <summary>
        /// Removes the field character.
        /// </summary>
        /// <param name="fieldCharacter">The fieldCharacter.</param>
        internal void RemoveFieldCharacter(FieldCharacterAdv fieldCharacter)
        {
            if (FieldCharacters.Contains(fieldCharacter))
            {
                FieldCharacters.Remove(fieldCharacter);
                if (Owner is CompositeNode && !(this is HeaderFooter))
                    (Owner as CompositeNode).RemoveFieldCharacter(fieldCharacter);
            }
        }
        /// <summary>
        /// Removes the field character.
        /// </summary>
        /// <param name="compositeNode">The compositeNode.</param>
        internal void RemoveFieldCharacter(CompositeNode compositeNode)
        {
            if (compositeNode is HeaderFooter)
                return;
            string compositeNodeIndex = compositeNode.GetHierarchicalIndex("");
            for (int i = 0; i < compositeNode.FieldCharacters.Count; i++)
            {
                FieldCharacterAdv fieldCharacter = compositeNode.FieldCharacters[i];
                int count = compositeNode.Fields.Count;
                fieldCharacter.CheckUnlinkFieldCharacter(compositeNodeIndex, true);
                RemoveFieldCharacter(fieldCharacter);
                if (count > compositeNode.FieldCharacters.Count)
                    i -= count - compositeNode.FieldCharacters.Count;
                if (i < -1)
                    i = -1;
            }
        }
        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="field">The field.</param>
        internal void AddField(FieldBeginAdv field)
        {
            Fields.Add(field);
            if (Owner is CompositeNode && !(this is HeaderFooter))
                (Owner as CompositeNode).AddField(field);
        }
        /// <summary>
        /// Removes the field.
        /// </summary>
        /// <param name="field">The field.</param>
        internal void RemoveField(FieldBeginAdv field)
        {
            Fields.Remove(field);
            if (Owner is CompositeNode && !(this is HeaderFooter))
                (Owner as CompositeNode).RemoveField(field);
        }
        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="compositeNode">The composite node.</param>
        internal void AddField(CompositeNode compositeNode)
        {
            if (compositeNode is HeaderFooter)
                return;
            foreach (FieldBeginAdv field in compositeNode.Fields)
            {
                AddField(field);
            }
        }
        /// <summary>
        /// Removes the field.
        /// </summary>
        /// <param name="compositeNode">The composite node.</param>
        internal void RemoveField(CompositeNode compositeNode)
        {
            if (compositeNode is HeaderFooter)
                return;
            string compositeNodeIndex = compositeNode.GetHierarchicalIndex("");
            for (int i = 0; i < compositeNode.Fields.Count; i++)
            {
                FieldBeginAdv field = compositeNode.Fields[i];
                int count = compositeNode.Fields.Count;
                if (field.IsUnlinkFieldCharacters(compositeNodeIndex))
                    field.UnlinkFieldCharacter(compositeNodeIndex, true);
                RemoveField(field);
                if (count > compositeNode.Fields.Count)
                    i -= count - compositeNode.Fields.Count;
                if (i < -1)
                    i = -1;
            }
        }
        /// <summary>
        /// Gets the hyperlink field.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <param name="CheckedFields">The checked fields.</param>
        /// <returns></returns>
        internal FieldBeginAdv GetHyperlinkField(Inline inline, List<FieldBeginAdv> CheckedFields)
        {
            foreach (FieldBeginAdv field in Fields)
            {
                if (CheckedFields.Contains(field) || field.FieldSeparator == null)
                    continue;
                else
                    CheckedFields.Add(field);
                string fieldCode = field.GetFieldCode();
                fieldCode = fieldCode.TrimStart(' ').ToLowerInvariant();
                if (fieldCode.StartsWith("hyperlink ") && field.InlineIsInFieldResult(inline))
                    return field;
            }
            if (Owner is CompositeNode && !(this is HeaderFooter))
                return (Owner as CompositeNode).GetHyperlinkField(inline, CheckedFields);

            return null;
        }
        /// <summary>
        /// Gets the hyperlink field.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="CheckedFields">The checked fields.</param>
        /// <returns></returns>
        internal FieldBeginAdv GetHyperlinkField(ParagraphAdv paragraph, List<FieldBeginAdv> CheckedFields)
        {
            foreach (FieldBeginAdv field in Fields)
            {
                if (CheckedFields.Contains(field) || field.FieldSeparator == null)
                    continue;
                else
                    CheckedFields.Add(field);
                string fieldCode = field.GetFieldCode();
                fieldCode = fieldCode.TrimStart(' ').ToLowerInvariant();
                if (fieldCode.StartsWith("hyperlink ") && field.ParagraphIsInFieldResult(paragraph))
                    return field;
            }
            if (Owner is CompositeNode && !(this is HeaderFooter))
                return (Owner as CompositeNode).GetHyperlinkField(paragraph, CheckedFields);

            return null;
        }
        /// <summary>
        /// Gets the hierarchical index of the composite node.
        /// </summary>
        /// <param name="hierarchicalIndex">The hierarchicalIndex.</param>
        /// <returns></returns>
        internal string GetHierarchicalIndex(string hierarchicalIndex)
        {
            if (Owner is CompositeNode)
            {
                if (this is HeaderFooters)
                    hierarchicalIndex = "HF;" + hierarchicalIndex;
                else
                    hierarchicalIndex = GetIndexInOwnerCollection().ToString() + ";" + hierarchicalIndex;
                if (!(Owner is DocumentAdv))
                    return (Owner as CompositeNode).GetHierarchicalIndex(hierarchicalIndex);
            }
            return hierarchicalIndex;
        }
        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        internal ParagraphAdv GetParagraph(ref string position)
        {
            if (string.IsNullOrEmpty(position))
                return null;
            int index = position.IndexOf(";");
            string value = "0";
            if (index >= 0)
            {
                value = position.Substring(0, index);
                position = position.Substring(index).TrimStart(';');
            }
            if (this is SectionAdv && value == "HF")
                //Gets the paragraph in Header footers.
                return (this as SectionAdv).HeaderFooters.GetParagraph(ref position);
            index = int.Parse(value);
            if (this is TableRowAdv && index >= ChildNodes.Count)
            {
                position = "0;0";
                index = ChildNodes.Count - 1;
            }
            if (index >= 0 && index < ChildNodes.Count)
            {
                Node child = ChildNodes[index];
                if (child is ParagraphAdv)
                {
                    if (position.Contains(";"))
                        position = "0";
                    return child as ParagraphAdv;
                }
                if (child is CompositeNode)
                {
                    if (position.Contains(";"))
                        return (child as CompositeNode).GetParagraph(ref position);
                    else
                    {
                        //If table is shifted to previous text position then return the first paragraph within table.
                        if (child is TableAdv)
                            return (child as TableAdv).GetFirstParagraphInFirstCell();
                        return null;
                    }
                }
            }
            else if (NextNode is CompositeNode)
            {
                position = "0";
                if (NextNode is TableAdv)
                    return (NextNode as TableAdv).GetFirstParagraphInFirstCell();
                return NextNode as ParagraphAdv;
            }
            return null;
        }
        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        internal BlockAdv GetBlock(ref string position)
        {
            if (string.IsNullOrEmpty(position))
                return null;
            int index = position.IndexOf(";");
            string value = position.Substring(0, index);
            position = position.Substring(index).TrimStart(';');
            if (this is SectionAdv && value == "HF")
                //Gets the block in Header footers.
                return (this as SectionAdv).HeaderFooters.GetBlock(ref position);
            index = int.Parse(value);
            if (index >= 0 && index < ChildNodes.Count)
            {
                Node child = ChildNodes[index];
                if (position.Contains(";"))
                {
                    if (child is CompositeNode)
                        return (child as CompositeNode).GetBlock(ref position);
                }
                else
                    return child as BlockAdv;
            }
            return this as BlockAdv;
        }
        #endregion
    }
}
