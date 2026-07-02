// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

namespace ServerEditorWeb.Components.Editor;

public class DockPanelDragEventArgs
{
    public string PanelId { get; }
    public double StartX { get; }
    public double StartY { get; }

    public DockPanelDragEventArgs(string panelId, double startX, double startY)
    {
        PanelId = panelId;
        StartX = startX;
        StartY = startY;
    }
}

public class DockPanelContextMenuEventArgs
{
    public string PanelId { get; }
    public double ClientX { get; }
    public double ClientY { get; }

    public DockPanelContextMenuEventArgs(string panelId, double clientX, double clientY)
    {
        PanelId = panelId;
        ClientX = clientX;
        ClientY = clientY;
    }
}

public class DockPanelState
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "📄";
    public string Zone { get; set; } = "center"; // "center", "right", "bottom", "floating", "hidden", "autohide-right", "autohide-bottom"
    public bool IsFloating => Zone == "floating";
    public bool IsVisible => Zone != "hidden";
    public double FloatX { get; set; } = 150;
    public double FloatY { get; set; } = 100;
    public double FloatWidth { get; set; } = 500;
    public double FloatHeight { get; set; } = 400;
    public int ZIndex { get; set; } = 100;
}
