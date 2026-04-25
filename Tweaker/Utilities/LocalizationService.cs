using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.Win32;

namespace Tweaker.Utilities;

public enum AppLanguage
{
    Spanish,
    English
}

public static class LocalizationService
{
    private const string AppKey = @"Software\GhostOptimizer";
    private const string LanguageValueName = "Language";

    public static AppLanguage CurrentLanguage { get; private set; } = AppLanguage.Spanish;
    public static bool IsEnglish => CurrentLanguage == AppLanguage.English;

    public static bool HasSavedPreference()
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AppKey, writable: false);
            string? rawValue = key?.GetValue(LanguageValueName)?.ToString();
            return string.Equals(rawValue, "en", StringComparison.OrdinalIgnoreCase)
                || string.Equals(rawValue, "es", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public static void Initialize()
    {
        CurrentLanguage = LoadLanguage();
        Debug.WriteLine($"🌐 Localization initialized: {CurrentLanguage}");
    }

    public static void SetLanguage(AppLanguage language)
    {
        CurrentLanguage = language;
        SaveLanguage(language);
    }

    public static string Text(string spanish, string english)
    {
        return IsEnglish ? english : spanish;
    }

    public static void ApplyToWindow(Window window)
    {
        if (window == null || !IsEnglish)
        {
            return;
        }

        window.Title = Translate(window.Title);
        TranslateElement(window);
    }

    public static string Translate(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || !IsEnglish)
        {
            return input;
        }

        if (LocalizationCatalog.SpanishToEnglishExact.TryGetValue(input, out string? exact))
        {
            return exact;
        }

        string translated = input;
        foreach (var replacement in LocalizationCatalog.SpanishToEnglishPartial.OrderByDescending(p => p.Key.Length))
        {
            translated = translated.Replace(replacement.Key, replacement.Value, StringComparison.OrdinalIgnoreCase);
        }

        return translated;
    }

    private static AppLanguage LoadLanguage()
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AppKey, writable: false);
            string? rawValue = key?.GetValue(LanguageValueName)?.ToString();

            if (string.Equals(rawValue, "en", StringComparison.OrdinalIgnoreCase))
            {
                return AppLanguage.English;
            }

            if (string.Equals(rawValue, "es", StringComparison.OrdinalIgnoreCase))
            {
                return AppLanguage.Spanish;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Could not load language preference: {ex.Message}");
        }

        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase)
            ? AppLanguage.English
            : AppLanguage.Spanish;
    }

    private static void SaveLanguage(AppLanguage language)
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.CreateSubKey(AppKey, writable: true);
            key?.SetValue(LanguageValueName, language == AppLanguage.English ? "en" : "es", RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Could not save language preference: {ex.Message}");
        }
    }

    private static void TranslateElement(DependencyObject parent)
    {
        if (parent is TextBlock textBlock)
        {
            textBlock.Text = Translate(textBlock.Text);
            foreach (Inline inline in textBlock.Inlines)
            {
                if (inline is Run run)
                {
                    run.Text = Translate(run.Text);
                }
            }
        }

        if (parent is ContentControl contentControl)
        {
            if (contentControl.Content is string content)
            {
                contentControl.Content = Translate(content);
            }

            if (contentControl is HeaderedContentControl headered && headered.Header is string header)
            {
                headered.Header = Translate(header);
            }
        }

        if (parent is HeaderedItemsControl headeredItemsControl && headeredItemsControl.Header is string headerText)
        {
            headeredItemsControl.Header = Translate(headerText);
        }

        if (parent is ToolTip toolTip && toolTip.Content is string ttContent)
        {
            toolTip.Content = Translate(ttContent);
        }

        if (parent is FrameworkElement frameworkElement)
        {
            if (frameworkElement.ToolTip is string tooltip)
            {
                frameworkElement.ToolTip = Translate(tooltip);
            }
            else if (frameworkElement.ToolTip is ToolTip tt && tt.Content is string ttInner)
            {
                tt.Content = Translate(ttInner);
            }
        }

        if (parent is Selector selector)
        {
            TranslateSelectorItems(selector.Items);
        }

        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            TranslateElement(VisualTreeHelper.GetChild(parent, i));
        }
    }

    private static void TranslateSelectorItems(IList items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] is string text)
            {
                items[i] = Translate(text);
            }
        }
    }
}



