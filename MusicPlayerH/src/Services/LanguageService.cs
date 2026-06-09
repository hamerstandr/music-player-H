using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MusicPlayerH.Services
{
    public enum SupportedLanguage
    {
        FA, // فارسی - Default
        AR, // العربية
        EN  // English
    }

    public partial class LanguageService : ObservableObject, IDisposable
    {
        private static LanguageService? _instance;
        private ResourceDictionary? _currentLanguageDictionary;
        
        public static LanguageService Instance => _instance ??= new LanguageService();

        [ObservableProperty]
        private SupportedLanguage _currentLanguage = SupportedLanguage.FA;

        [ObservableProperty]
        private string _currentLanguageName = "فارسی";

        public event Action<CultureInfo>? LanguageChanged;

        public CultureInfo CurrentCulture
        {
            get
            {
                return CurrentLanguage switch
                {
                    SupportedLanguage.FA => new CultureInfo("fa-IR"),
                    SupportedLanguage.AR => new CultureInfo("ar-SA"),
                    SupportedLanguage.EN => new CultureInfo("en-US"),
                    _ => new CultureInfo("fa-IR")
                };
            }
        }

        public void Initialize()
        {
            LoadLanguage(SupportedLanguage.FA);
        }

        public void LoadLanguage(SupportedLanguage language)
        {
            if (CurrentLanguage == language) return;

            var culture = GetCultureInfo(language);
            
            // Set thread culture
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Remove old language dictionary
            if (_currentLanguageDictionary != null && Application.Current.Resources.MergedDictionaries.Contains(_currentLanguageDictionary))
            {
                Application.Current.Resources.MergedDictionaries.Remove(_currentLanguageDictionary);
            }

            // Load new language dictionary
            var langPath = $"src/Resources/Languages/{language.ToString().ToLower()}.xaml";
            try
            {
                _currentLanguageDictionary = new ResourceDictionary
                {
                    Source = new Uri(langPath, UriKind.Relative)
                };

                Application.Current.Resources.MergedDictionaries.Add(_currentLanguageDictionary);
                
                CurrentLanguage = language;
                CurrentLanguageName = GetLanguageDisplayName(language);
                
                LanguageChanged?.Invoke(culture);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading language {language}: {ex.Message}");
            }
        }

        private CultureInfo GetCultureInfo(SupportedLanguage language)
        {
            return language switch
            {
                SupportedLanguage.FA => new CultureInfo("fa-IR"),
                SupportedLanguage.AR => new CultureInfo("ar-SA"),
                SupportedLanguage.EN => new CultureInfo("en-US"),
                _ => new CultureInfo("fa-IR")
            };
        }

        private string GetLanguageDisplayName(SupportedLanguage language)
        {
            return language switch
            {
                SupportedLanguage.FA => "فارسی",
                SupportedLanguage.AR => "العربية",
                SupportedLanguage.EN => "English",
                _ => "فارسی"
            };
        }

        [RelayCommand]
        private void SwitchToFA() => LoadLanguage(SupportedLanguage.FA);

        [RelayCommand]
        private void SwitchToAR() => LoadLanguage(SupportedLanguage.AR);

        [RelayCommand]
        private void SwitchToEN() => LoadLanguage(SupportedLanguage.EN);

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
