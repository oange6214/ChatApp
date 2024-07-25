﻿using System.Windows;
using System.Windows.Controls;

namespace ChatApp.AttachedProperties;

public class ScrollViewerAttachedProperty
{
    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached(
            "AutoScroll",
            typeof(bool),
            typeof(ScrollViewerAttachedProperty),
            new PropertyMetadata(false, AutoScrollPropertyChanged));

    public static bool GetAutoScroll(DependencyObject obj)
    {
        return (bool)obj.GetValue(AutoScrollProperty);
    }

    public static void SetAutoScroll(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoScrollProperty, value);
    }

    private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var scrollViewer = d as ScrollViewer;

        if (scrollViewer != null && (bool)e.NewValue)
        {
            scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            scrollViewer.ScrollToEnd();
        }
        else
        {
            scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }
    }

    private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        // Only Scroll to bottom when the extent is changed. Otherwise you won't be able to scroll up...
        if (e.ExtentHeightChange != 0)
        {
            var scrollViewer = sender as ScrollViewer;
            scrollViewer?.ScrollToBottom();
        }
    }
}