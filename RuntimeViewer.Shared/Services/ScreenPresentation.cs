// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using SharedModels;

namespace RuntimeViewer.Shared.Services;

public static class ScreenPresentation
{
    public static string Transition(ScreenConfig screen, bool reverse = false)
    {
        var transition = screen.NavigationTransition;
        if (transition is not ("fade" or "slide-left" or "slide-right" or "slide-up" or "slide-down"))
            return "none";
        return reverse ? Reverse(transition) : transition;
    }

    public static string Reverse(string transition) => transition switch
    {
        "slide-left" => "slide-right",
        "slide-right" => "slide-left",
        "slide-up" => "slide-down",
        "slide-down" => "slide-up",
        "fade" => "fade",
        _ => "none"
    };

    public static int Duration(ScreenConfig screen) => Math.Clamp(screen.NavigationDurationMs, 0, 2000);
    public static int DockSize(ScreenConfig screen) => Math.Clamp(screen.DockSize, 24, 1200);

    public static string SwipeTarget(ScreenConfig screen, string direction) => direction switch
    {
        "up" => screen.SwipeUpScreen,
        "down" => screen.SwipeDownScreen,
        "left" => screen.SwipeLeftScreen,
        "right" => screen.SwipeRightScreen,
        _ => ""
    };

    public static string OpeningDirection(string edge) => edge switch
    {
        "top" => "down",
        "bottom" => "up",
        "left" => "right",
        "right" => "left",
        _ => ""
    };
}

public record ScreenSwipeNavigation(string Target, string Direction);
