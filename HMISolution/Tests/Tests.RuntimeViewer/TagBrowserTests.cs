using Xunit;
using RuntimeViewer.Shared.Services;

namespace Tests.RuntimeViewer;

/// <summary>
/// Tests for the BrowsedTag model used by the Runtime Tag Browser feature.
/// </summary>
public class TagBrowserTests
{
    // ──────────────────────────────────────────────────────────────
    //  BrowsedTag defaults
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BrowsedTag_DefaultValues()
    {
        var tag = new BrowsedTag();
        Assert.Equal("", tag.NodeId);
        Assert.Equal("", tag.DisplayName);
        Assert.Equal("", tag.BrowsePath);
        Assert.Equal("", tag.NodeClass);
        Assert.Equal("", tag.DataType);
        Assert.False(tag.IsFolder);
        Assert.False(tag.IsExpanded);
        Assert.Equal(0, tag.Depth);
        Assert.NotNull(tag.Children);
        Assert.Empty(tag.Children);
        Assert.False(tag.ChildrenLoaded);
    }

    // ──────────────────────────────────────────────────────────────
    //  Property assignment
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BrowsedTag_SetProperties()
    {
        var tag = new BrowsedTag
        {
            NodeId = "ns=2;s=Plant.Temperature",
            DisplayName = "Temperature",
            BrowsePath = "Plant.Temperature",
            NodeClass = "Variable",
            DataType = "Double",
            IsFolder = false,
            IsExpanded = true,
            Depth = 2,
            ChildrenLoaded = true
        };

        Assert.Equal("ns=2;s=Plant.Temperature", tag.NodeId);
        Assert.Equal("Temperature", tag.DisplayName);
        Assert.Equal("Plant.Temperature", tag.BrowsePath);
        Assert.Equal("Variable", tag.NodeClass);
        Assert.Equal("Double", tag.DataType);
        Assert.False(tag.IsFolder);
        Assert.True(tag.IsExpanded);
        Assert.Equal(2, tag.Depth);
        Assert.True(tag.ChildrenLoaded);
    }

    // ──────────────────────────────────────────────────────────────
    //  Folder tag with children
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BrowsedTag_FolderWithChildren()
    {
        var folder = new BrowsedTag
        {
            NodeId = "ns=2;s=Plant",
            DisplayName = "Plant",
            BrowsePath = "Plant",
            NodeClass = "Object",
            IsFolder = true,
            Depth = 0,
            Children =
            [
                new BrowsedTag
                {
                    NodeId = "ns=2;s=Plant.Temp",
                    DisplayName = "Temp",
                    BrowsePath = "Plant.Temp",
                    NodeClass = "Variable",
                    DataType = "Double",
                    Depth = 1
                },
                new BrowsedTag
                {
                    NodeId = "ns=2;s=Plant.Pressure",
                    DisplayName = "Pressure",
                    BrowsePath = "Plant.Pressure",
                    NodeClass = "Variable",
                    DataType = "Float",
                    Depth = 1
                }
            ],
            ChildrenLoaded = true
        };

        Assert.True(folder.IsFolder);
        Assert.Equal(2, folder.Children.Count);
        Assert.Equal("Temp", folder.Children[0].DisplayName);
        Assert.Equal("Pressure", folder.Children[1].DisplayName);
        Assert.Equal(1, folder.Children[0].Depth);
        Assert.True(folder.ChildrenLoaded);
    }

    // ──────────────────────────────────────────────────────────────
    //  Nested hierarchy
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BrowsedTag_NestedHierarchy()
    {
        var root = new BrowsedTag
        {
            NodeId = "ns=0;i=85",
            DisplayName = "Objects",
            IsFolder = true,
            Depth = 0,
            Children =
            [
                new BrowsedTag
                {
                    NodeId = "ns=2;s=Plant",
                    DisplayName = "Plant",
                    IsFolder = true,
                    Depth = 1,
                    Children =
                    [
                        new BrowsedTag
                        {
                            NodeId = "ns=2;s=Plant.Furnace",
                            DisplayName = "Furnace",
                            IsFolder = true,
                            Depth = 2,
                            Children =
                            [
                                new BrowsedTag
                                {
                                    NodeId = "ns=2;s=Plant.Furnace.Temp",
                                    DisplayName = "Temp",
                                    NodeClass = "Variable",
                                    DataType = "Double",
                                    Depth = 3
                                }
                            ],
                            ChildrenLoaded = true
                        }
                    ],
                    ChildrenLoaded = true
                }
            ],
            ChildrenLoaded = true
        };

        Assert.Equal(1, root.Children.Count);
        var plant = root.Children[0];
        Assert.Equal(1, plant.Children.Count);
        var furnace = plant.Children[0];
        Assert.Equal(1, furnace.Children.Count);
        var temp = furnace.Children[0];
        Assert.Equal("Temp", temp.DisplayName);
        Assert.Equal(3, temp.Depth);
        Assert.Equal("Double", temp.DataType);
    }

    // ──────────────────────────────────────────────────────────────
    //  Children list is mutable (supports lazy loading pattern)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BrowsedTag_ChildrenAreAddedDynamically()
    {
        var folder = new BrowsedTag
        {
            NodeId = "ns=2;s=Folder",
            DisplayName = "Folder",
            IsFolder = true,
            Depth = 0,
            ChildrenLoaded = false
        };

        Assert.Empty(folder.Children);
        Assert.False(folder.ChildrenLoaded);

        // Simulate lazy-load browse result
        folder.Children.Add(new BrowsedTag
        {
            NodeId = "ns=2;s=Folder.Tag1",
            DisplayName = "Tag1",
            DataType = "Int32",
            Depth = 1
        });
        folder.ChildrenLoaded = true;

        Assert.Single(folder.Children);
        Assert.True(folder.ChildrenLoaded);
        Assert.Equal("Tag1", folder.Children[0].DisplayName);
    }

    // ──────────────────────────────────────────────────────────────
    //  Expand/collapse state tracking
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BrowsedTag_ExpandCollapseState()
    {
        var tag = new BrowsedTag { IsFolder = true, IsExpanded = false };
        Assert.False(tag.IsExpanded);

        tag.IsExpanded = true;
        Assert.True(tag.IsExpanded);

        tag.IsExpanded = false;
        Assert.False(tag.IsExpanded);
    }
}
