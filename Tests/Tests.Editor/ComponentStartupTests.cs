using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Services;
using Xunit;

namespace Tests.Editor;

public class ComponentStartupTests
{
    [Theory]
    [InlineData(null, false, true)]
    [InlineData("true", false, false)]
    [InlineData(null, true, false)]
    public async Task LandingPage_InteractiveStartup_HonorsQuickStartPreference(string? preference, bool hasRecentFiles, bool showGuide)
    {
        var editor = new NodeEditorService();
        editor.RecentFiles.Clear();
        if (hasRecentFiles) editor.RecentFiles.Add("recent-project.json");
        var js = new StartupJsRuntime(preference);
        using var services = new ServiceCollection().AddLogging()
            .AddSingleton(editor).AddSingleton<IJSRuntime>(js).BuildServiceProvider();
        await using var renderer = new StartupRenderer(services, js);

        await renderer.RenderAsync<LandingPage>();
        await renderer.WaitForAsync(() => js.StorageReads == 1);

        Assert.Equal(0, js.EarlyCalls);
        Assert.Equal(showGuide, await renderer.Dispatcher.InvokeAsync(() =>
            renderer.Frames.Any(f => f.FrameType == RenderTreeFrameType.Component && f.ComponentType == typeof(QuickStartGuideSimple))));
    }

    [Fact]
    public async Task DockContainer_PrerenderDisposal_DoesNotCallBrowserCleanup()
    {
        var js = new StartupJsRuntime();
        using var services = new ServiceCollection().AddLogging()
            .AddSingleton<DockLayoutService>().AddSingleton<IJSRuntime>(js).BuildServiceProvider();
        await using (var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>()))
        {
            await renderer.Dispatcher.InvokeAsync(() => renderer.RenderComponentAsync<DockContainer>());
        }
        Assert.Equal(0, js.EarlyCalls);
    }

    [Fact]
    public async Task SymbolLibrary_Prerender_ShowsLoadingWithoutReadingLibrary()
    {
        using var library = new LibraryFixture();
        using var services = library.Services(new StartupJsRuntime());
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var result = await renderer.RenderComponentAsync<SymbolLibraryPanel>();
            return result.ToHtmlString();
        });

        Assert.Contains("Loading symbol library...", html);
        Assert.DoesNotContain("symbol-library-item ", html);
        Assert.DoesNotContain("No symbols found.", html);
    }

    [Fact]
    public async Task SymbolLibrary_PagesAndFiltersAcrossEntireLibrary_AndRefreshesImports()
    {
        using var library = new LibraryFixture();
        for (var i = 0; i < 121; i++) library.AddSymbol("CategoryA", $"Symbol{i:D3}");
        library.AddSymbol("CategoryB", "OtherSymbol");
        var js = new StartupJsRuntime();
        using var services = library.Services(js);
        await using var renderer = new StartupRenderer(services, js);
        SymbolLibraryItem? selected = null;
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(SymbolLibraryPanel.OnSymbolSelected)] = EventCallback.Factory.Create<SymbolLibraryItem>(this, item => selected = item)
        });

        await renderer.RenderAsync<SymbolLibraryPanel>(parameters);
        await renderer.WaitForAsync(() => renderer.SymbolNames.Count > 0);
        Assert.Equal(60, renderer.SymbolNames.Count);
        Assert.Equal("Symbol000", renderer.SymbolNames[0]);
        Assert.Equal("Symbol059", renderer.SymbolNames[^1]);

        await renderer.ClickAsync("Next");
        Assert.Equal(60, renderer.SymbolNames.Count);
        Assert.Equal("Symbol060", renderer.SymbolNames[0]);
        await renderer.ClickAsync("Next");
        Assert.Equal(new[] { "Symbol120", "OtherSymbol" }, renderer.SymbolNames);

        await renderer.ClickAsync("CategoryB");
        Assert.Equal(new[] { "OtherSymbol" }, renderer.SymbolNames);
        await renderer.ClickAsync("All");
        Assert.Equal(60, renderer.SymbolNames.Count);
        await renderer.InputAsync("Symbol120");
        Assert.Equal(new[] { "Symbol120" }, renderer.SymbolNames);
        await renderer.ClickAsync("Symbol120");
        Assert.NotNull(selected);
        Assert.Equal("CategoryA", selected.Category);
        Assert.Contains("<svg", selected.SvgContent);

        library.AddSymbol("CategoryA", "NewImport");
        await renderer.InputAsync("NewImport");
        Assert.Empty(renderer.SymbolNames);
        await renderer.ClickAsync("Reload from disk");
        await renderer.WaitForAsync(() => renderer.SymbolNames.SequenceEqual(new[] { "NewImport" }));
        Assert.Equal(new[] { "NewImport" }, renderer.SymbolNames);
    }

    private sealed class StartupJsRuntime(string? preference = null) : IJSRuntime
    {
        public bool Interactive { get; set; }
        public int EarlyCalls { get; private set; }
        public int StorageReads { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => InvokeAsync<TValue>(identifier, default, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            if (!Interactive)
            {
                EarlyCalls++;
                throw new InvalidOperationException("JavaScript interop is unavailable during static rendering.");
            }
            if (identifier == "localStorage.getItem")
            {
                StorageReads++;
                return ValueTask.FromResult((TValue)(object?)preference!);
            }
            throw new InvalidOperationException($"Unexpected JavaScript call: {identifier}");
        }
    }

    private sealed class LibraryFixture : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), $"EditorSymbolTests-{Guid.NewGuid():N}");

        public LibraryFixture() => Directory.CreateDirectory(Path.Combine(_root, "SymbolLibrary"));

        public void AddSymbol(string category, string name)
        {
            var directory = Directory.CreateDirectory(Path.Combine(_root, "SymbolLibrary", category));
            File.WriteAllText(Path.Combine(directory.FullName, name + ".svg"), "<svg viewBox=\"0 0 64 64\"><rect width=\"64\" height=\"64\" /></svg>");
        }

        public ServiceProvider Services(StartupJsRuntime js) => new ServiceCollection().AddLogging()
            .AddSingleton<IWebHostEnvironment>(new TestEnvironment { ContentRootPath = _root })
            .AddSingleton<SymbolLibraryService>().AddSingleton<IJSRuntime>(js).BuildServiceProvider();

        public void Dispose() => Directory.Delete(_root, recursive: true);
    }

    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests.Editor";
        public string EnvironmentName { get; set; } = "Testing";
        public string ContentRootPath { get; set; } = "";
        public string WebRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }

#pragma warning disable BL0006 // Exercise real component rendering and event dispatch without a browser.
    private sealed class StartupRenderer(IServiceProvider services, StartupJsRuntime js)
        : Renderer(services, services.GetRequiredService<ILoggerFactory>())
    {
        private int _rootId;
        private Exception? _failure;
        private TaskCompletionSource _nextRender = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();
        public RenderTreeFrame[] Frames
        {
            get
            {
                var frames = GetCurrentRenderTreeFrames(_rootId);
                return frames.Array.Take(frames.Count).ToArray();
            }
        }
        public List<string> SymbolNames => Frames.Where(f => f.FrameType == RenderTreeFrameType.Attribute && f.AttributeName == "title")
            .Select(f => f.AttributeValue?.ToString() ?? "")
            .Where(name => !name.StartsWith("Delete category") && name != "Import SVG files from a folder" && name != "Reload from disk").ToList();

        public Task RenderAsync<T>(ParameterView? parameters = null) where T : IComponent => Dispatcher.InvokeAsync(async () =>
        {
            _rootId = AssignRootComponentId(InstantiateComponent(typeof(T)));
            await RenderRootComponentAsync(_rootId, parameters ?? ParameterView.Empty);
        });

        protected override void HandleException(Exception exception)
        {
            _failure = exception;
            _nextRender.TrySetResult();
        }

        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            js.Interactive = true;
            var previous = _nextRender;
            _nextRender = new(TaskCreationOptions.RunContinuationsAsynchronously);
            previous.TrySetResult();
            return Task.CompletedTask;
        }

        public Task WaitForAsync(Func<bool> condition) => Dispatcher.InvokeAsync(async () =>
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            while (!condition())
            {
                if (_failure != null) throw new InvalidOperationException("Component rendering failed.", _failure);
                await _nextRender.Task.WaitAsync(timeout.Token);
            }
        });

        public Task ClickAsync(string label) => Dispatcher.InvokeAsync(async () =>
        {
            var frames = Frames;
            for (var i = 0; i < frames.Length; i++)
            {
                if (frames[i].FrameType != RenderTreeFrameType.Element) continue;
                var end = i + frames[i].ElementSubtreeLength;
                var element = frames.Skip(i + 1).Take(end - i - 1).ToArray();
                var eventFrame = element.TakeWhile(f => f.FrameType == RenderTreeFrameType.Attribute)
                    .FirstOrDefault(f => f.AttributeName == "onclick");
                if (eventFrame.AttributeEventHandlerId == 0) continue;
                if (!element.Any(f => (f.FrameType == RenderTreeFrameType.Text && f.TextContent == label)
                    || (f.FrameType == RenderTreeFrameType.Attribute && f.AttributeName == "title" && Equals(f.AttributeValue, label)))) continue;
                await DispatchEventAsync(eventFrame.AttributeEventHandlerId, null, new MouseEventArgs());
                return;
            }
            throw new InvalidOperationException($"Clickable element not found: {label}");
        });

        public Task InputAsync(string text) => Dispatcher.InvokeAsync(async () =>
        {
            var input = Frames.First(f => f.FrameType == RenderTreeFrameType.Attribute && f.AttributeName == "oninput");
            await DispatchEventAsync(input.AttributeEventHandlerId, null, new ChangeEventArgs { Value = text });
        });
    }
#pragma warning restore BL0006
}
