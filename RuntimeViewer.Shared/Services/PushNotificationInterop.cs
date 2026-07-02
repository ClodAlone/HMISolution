// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Blazor JS interop wrapper for Web Push subscription management.
/// Uses the push-notifications.js ES module to subscribe/unsubscribe the browser
/// and communicates with the RuntimeViewer API to persist subscriptions.
/// </summary>
public sealed class PushNotificationInterop : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public PushNotificationInterop(IJSRuntime js)
    {
        _js = js;
    }

    private async Task<IJSObjectReference> GetModuleAsync()
    {
        _module ??= await _js.InvokeAsync<IJSObjectReference>(
            "import", "/push-notifications.js");
        return _module;
    }

    /// <summary>Returns true if the browser supports service workers and push notifications.</summary>
    public async Task<bool> IsSupportedAsync()
    {
        var mod = await GetModuleAsync();
        return await mod.InvokeAsync<bool>("isSupported");
    }

    /// <summary>Returns the current Notification permission state: "default", "granted", or "denied".</summary>
    public async Task<string> GetPermissionStateAsync()
    {
        var mod = await GetModuleAsync();
        return await mod.InvokeAsync<string>("getPermissionState");
    }

    /// <summary>Registers the service worker.</summary>
    public async Task RegisterServiceWorkerAsync()
    {
        var mod = await GetModuleAsync();
        await mod.InvokeVoidAsync("registerServiceWorker");
    }

    /// <summary>
    /// Subscribes the browser for push notifications using the server's VAPID public key.
    /// Returns the subscription info, or null if the user denied permission.
    /// </summary>
    public async Task<PushSubscriptionResult?> SubscribeAsync(string vapidPublicKey)
    {
        var mod = await GetModuleAsync();
        return await mod.InvokeAsync<PushSubscriptionResult?>("subscribe", vapidPublicKey);
    }

    /// <summary>Unsubscribes the browser from push notifications.</summary>
    public async Task UnsubscribeAsync()
    {
        var mod = await GetModuleAsync();
        await mod.InvokeVoidAsync("unsubscribe");
    }

    /// <summary>Gets the current push subscription, or null if not subscribed.</summary>
    public async Task<PushSubscriptionResult?> GetSubscriptionAsync()
    {
        var mod = await GetModuleAsync();
        return await mod.InvokeAsync<PushSubscriptionResult?>("getSubscription");
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.DisposeAsync();
        }
    }

    public class PushSubscriptionResult
    {
        public string Endpoint { get; set; } = "";
        public string P256dh { get; set; } = "";
        public string Auth { get; set; } = "";
    }
}
