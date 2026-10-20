using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class TreeViewContextMenuTests
{
    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 1)]
    [InlineData(false, 3)]
    [InlineData(true, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 3)]
    public async Task Delete_FromPopup_ReachesHandlerAtEveryTreeDepth(bool screen, int depth)
    {
        TreeNode target = screen
            ? new ScreenNode(new ScreenConfig { Name = "Target" })
            : new VariableNode(new Variable { Name = "Target", Type = "Double" });
        TreeNode root = target;
        for (var i = 0; i < depth; i++)
        {
            var parent = new FolderNode(new Folder { Name = $"Folder{i}" }) { IsExpanded = true };
            root.Parent = parent;
            parent.Children.Add(root);
            root = parent;
        }
        var container = new FolderNode(new Folder { Name = "Container" });
        root.Parent = container;
        container.Children.Add(root);

        var editor = new NodeEditorService();
        var undo = new UndoRedoService();
        editor.SetUndoService(undo);
        TreeNode? deleted = null;
        using var services = new ServiceCollection().AddLogging()
            .AddSingleton<ClipboardService>().AddSingleton<IJSRuntime>(new MenuJsRuntime()).BuildServiceProvider();
        await using var renderer = new MenuRenderer(services);
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(TreeViewItem.Node)] = root,
            [nameof(TreeViewItem.OnDelete)] = EventCallback.Factory.Create<TreeNode>(this, node =>
            {
                deleted = node;
                editor.SelectNode(node);
                editor.DeleteSelected();
            })
        });
        await renderer.RenderAsync(parameters);
        await renderer.OpenMenuAsync(target);
        await renderer.DeleteAsync();

        Assert.Same(target, deleted);
        Assert.DoesNotContain(target, target.Parent!.Children);
        Assert.Empty(editor.SelectedItems);
        Assert.True(undo.Undo());
        Assert.Contains(target, target.Parent.Children);
    }

    private sealed class MenuJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            InvokeAsync<TValue>(identifier, default, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Assert.Equal("editorShortcuts.clampPopup", identifier);
            return ValueTask.FromResult(default(TValue)!);
        }
    }

#pragma warning disable BL0006 // Exercise recursive component rendering and actual popup events.
    private sealed class MenuRenderer(IServiceProvider services)
        : Renderer(services, services.GetRequiredService<ILoggerFactory>())
    {
        private int _rootId;
        public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

        protected override void HandleException(Exception exception) => throw new InvalidOperationException("Tree rendering failed.", exception);
        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch) => Task.CompletedTask;

        public Task RenderAsync(ParameterView parameters) => Dispatcher.InvokeAsync(async () =>
        {
            _rootComponent = (TreeViewItem)InstantiateComponent(typeof(TreeViewItem));
            _rootId = AssignRootComponentId(_rootComponent);
            await RenderRootComponentAsync(_rootId, parameters);
        });

        private IEnumerable<RenderTreeFrame> Frames(int componentId)
        {
            var range = GetCurrentRenderTreeFrames(componentId);
            var frames = range.Array.Take(range.Count).ToArray();
            foreach (var frame in frames)
            {
                yield return frame;
                if (frame.FrameType == RenderTreeFrameType.Component)
                    foreach (var child in Frames(frame.ComponentId))
                        yield return child;
            }
        }

        public Task OpenMenuAsync(TreeNode node) => Dispatcher.InvokeAsync(async () =>
        {
            var componentId = ReferenceEquals(_rootComponent!.Node, node)
                ? _rootId
                : Frames(_rootId).Single(f => f.FrameType == RenderTreeFrameType.Component
                    && f.Component is TreeViewItem item && ReferenceEquals(item.Node, node)).ComponentId;
            var handler = Frames(componentId).First(f => f.FrameType == RenderTreeFrameType.Attribute
                && f.AttributeName == "oncontextmenu").AttributeEventHandlerId;
            await DispatchEventAsync(handler, null, new MouseEventArgs());
        });

        private TreeViewItem? _rootComponent;

        public Task DeleteAsync() => Dispatcher.InvokeAsync(async () =>
        {
            var menu = Frames(_rootId).Single(f => f.FrameType == RenderTreeFrameType.Component
                && f.Component is TreeViewContextMenu);
            var range = GetCurrentRenderTreeFrames(menu.ComponentId);
            var frames = range.Array.Take(range.Count).ToArray();
            var deleteIndex = Array.FindIndex(frames, f => f.FrameType == RenderTreeFrameType.Attribute
                && f.AttributeName == "class" && Equals(f.AttributeValue, "dock-context-item dock-context-danger"));
            Assert.True(deleteIndex > 0, "The popup must contain a Delete menu item.");
            var handler = frames.Skip(deleteIndex + 1).TakeWhile(f => f.FrameType == RenderTreeFrameType.Attribute)
                .Single(f => f.AttributeName == "onclick").AttributeEventHandlerId;
            await DispatchEventAsync(handler, null, new MouseEventArgs());
        });
    }
#pragma warning restore BL0006
}
