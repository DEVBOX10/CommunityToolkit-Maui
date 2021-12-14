﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Views;
using CoreGraphics;
using Microsoft.Maui;
using Microsoft.Maui.Platform;
using UIKit;

namespace CommunityToolkit.Maui.Alerts;

public partial class Snackbar
{
	readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

	static SnackbarView? _nativeSnackbar;

	/// <summary>
	/// Dismiss Snackbar
	/// </summary>
	public async Task Dismiss()
	{
		if (_nativeSnackbar is null)
			return;

		await _semaphoreSlim.WaitAsync();

		try
		{
			_nativeSnackbar.Dismiss();
			_nativeSnackbar = null;

			OnDismissed();
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	/// <summary>
	/// Show Snackbar
	/// </summary>
	public async Task Show()
	{
		await Dismiss();

		var cornerRadius = GetCornerRadius(VisualOptions.CornerRadius);
		var padding = GetMaximum(cornerRadius.X, cornerRadius.Y, cornerRadius.Width, cornerRadius.Height) + SnackbarView.DefaultPadding;
		_nativeSnackbar = new SnackbarView(Text,
											VisualOptions.BackgroundColor.ToNative(),
											cornerRadius,
											VisualOptions.TextColor.ToNative(),
											UIFont.SystemFontOfSize((float)VisualOptions.Font.Size),
											VisualOptions.CharacterSpacing,
											ActionButtonText,
											VisualOptions.ActionButtonTextColor.ToNative(),
											UIFont.SystemFontOfSize((float)VisualOptions.ActionButtonFont.Size),
											padding)
		{
			Action = Action,
			Anchor = Anchor?.Handler?.NativeView as UIView,
			Duration = Duration
		};

		_nativeSnackbar.Show();

		OnShown();

		static T? GetMaximum<T>(params T[] items) => items.Max();
	}

	static CGRect GetCornerRadius(CornerRadius cornerRadius)
	{
		return new CGRect(cornerRadius.BottomLeft, cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight);
	}
}