using Soenneker.Quark.DatePickers.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Utils.AsyncSingleton;

namespace Soenneker.Quark.DatePickers;

/// <inheritdoc cref="IDatePickerInterop"/>
public sealed class DatePickerInterop : IDatePickerInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<DatePickerInterop> _logger;
    private readonly AsyncSingleton _cssInitializer;
    private readonly AsyncSingleton _jsInitializer;

    private const string _cssPath = "_content/Soenneker.Quark.DatePickers/css/datepicker.css";
    private const string _jsInit = "QuarkDatePicker.attach";
    private const string _jsPath = "_content/Soenneker.Quark.DatePickers/js/datepicker.js";
    private const string _jsOutside = "QuarkDatePicker.registerOutsideClose";

    public DatePickerInterop(IJSRuntime jSRuntime, ILogger<DatePickerInterop> logger, IResourceLoader resourceLoader)
    {
        _jsRuntime = jSRuntime;
        _logger = logger;

        IResourceLoader loader = resourceLoader;
        _cssInitializer = new AsyncSingleton(async (token, _) =>
        {
            await loader.LoadStyle(_cssPath, cancellationToken: token);
            return new object();
        });

        _jsInitializer = new AsyncSingleton(async (token, _) =>
        {
            await loader.LoadScript(_jsPath, cancellationToken: token);
            return new object();
        });
    }

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        await _cssInitializer.Init(cancellationToken);
        await _jsInitializer.Init(cancellationToken);
    }

    public async ValueTask Attach(ElementReference element, CancellationToken cancellationToken = default)
    {
        await _cssInitializer.Init(cancellationToken);
        await _jsInitializer.Init(cancellationToken);

        try
        {
            await _jsRuntime.InvokeVoidAsync(_jsInit, cancellationToken, element);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "DatePicker attach failed (non-fatal)");
        }
    }

    public async ValueTask RegisterOutsideClose<T>(ElementReference container, DotNetObjectReference<T> dotNetRef, string methodName, CancellationToken cancellationToken = default) where T : class
    {
        await _cssInitializer.Init(cancellationToken);
        await _jsInitializer.Init(cancellationToken);

        try
        {
            await _jsRuntime.InvokeVoidAsync(_jsOutside, cancellationToken, container, dotNetRef, methodName);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "DatePicker outside-close registration failed (non-fatal)");
        }
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.WhenAll(_cssInitializer.DisposeAsync().AsTask(), _jsInitializer.DisposeAsync().AsTask()));
    }
}
