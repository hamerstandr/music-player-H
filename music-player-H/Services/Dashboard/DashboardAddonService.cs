using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MusicPlayerH.Models.Dashboard;

namespace MusicPlayerH.Services.Dashboard
{
    /// <summary>
    /// سرویس مدیریت افزونه‌های داشبورد
    /// مسئولیت ثبت، بارگذاری و مدیریت چرخه حیات افزونه‌ها
    /// </summary>
    public class DashboardAddonService
    {
        private static readonly Lazy<DashboardAddonService> _instance = 
            new Lazy<DashboardAddonService>(() => new DashboardAddonService());
        
        private readonly List<AddonInfo> _addons;
        private readonly string _configPath;
        private bool _isInitialized;

        /// <summary>
        /// نمونه یکتای سرویس
        /// </summary>
        public static DashboardAddonService Instance => _instance.Value;

        /// <summary>
        /// رویداد تغییر وضعیت افزونه
        /// </summary>
        public event EventHandler<AddonStateChangedEventArgs> OnAddonStateChanged;

        /// <summary>
        /// رویداد بروزرسانی لیست افزونه‌ها
        /// </summary>
        public event EventHandler OnAddonsUpdated;

        private DashboardAddonService()
        {
            _addons = new List<AddonInfo>();
            _configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MusicPlayerH",
                "addons_config.json"
            );
        }

        /// <summary>
        /// مقداردهی اولیه سرویس
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                // اطمینان از وجود پوشه تنظیمات
                var configDir = Path.GetDirectoryName(_configPath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                // ثبت افزونه‌های پیش‌فرض
                RegisterDefaultAddons();

                // بارگذاری تنظیمات ذخیره شده
                LoadSavedSettings();

                // بررسی وضعیت نصب افزونه‌ها
                _ = ScanAllAddonsAsync();

                _isInitialized = true;
                
                System.Diagnostics.Debug.WriteLine("DashboardAddonService initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing DashboardAddonService: {ex.Message}");
            }
        }

        /// <summary>
        /// ثبت افزونه‌های پیش‌فرض
        /// </summary>
        private void RegisterDefaultAddons()
        {
            // Music Player Addon (اصلی - همیشه فعال)
            if (!_addons.Any(a => a.Id == "music-player"))
            {
                _addons.Add(new MusicPlayerAddonInfo());
            }
        }

        /// <summary>
        /// بارگذاری تنظیمات ذخیره شده
        /// </summary>
        private async void LoadSavedSettings()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    // TODO: خواندن تنظیمات از فایل JSON
                    // فعلاً از تنظیمات پیش‌فرض استفاده می‌شود
                    await Task.Delay(1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading saved settings: {ex.Message}");
            }
        }

        /// <summary>
        /// ذخیره تنظیمات
        /// </summary>
        public async Task SaveAddonsAsync()
        {
            try
            {
                // TODO: نوشتن تنظیمات به فایل JSON
                await Task.Delay(1);
                System.Diagnostics.Debug.WriteLine("Addon settings saved");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving addon settings: {ex.Message}");
            }
        }

        /// <summary>
        /// اسکن تمام افزونه‌ها برای بررسی وضعیت نصب
        /// </summary>
        public async Task ScanAllAddonsAsync()
        {
            foreach (var addon in _addons)
            {
                await ScanAddonAsync(addon);
            }
            
            OnAddonsUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// اسکن یک افزونه خاص
        /// </summary>
        private async Task ScanAddonAsync(AddonInfo addon)
        {
            try
            {
                bool wasInstalled = addon.IsInstalled;

                // موزیک پلیر داخلی است، همیشه نصب محسوب می‌شود
                addon.IsInstalled = true;

                // ارسال رویداد در صورت تغییر وضعیت
                if (wasInstalled != addon.IsInstalled)
                {
                    OnAddonStateChanged?.Invoke(this, new AddonStateChangedEventArgs
                    {
                        Addon = addon,
                        PreviousState = wasInstalled ? "installed" : "not_installed",
                        NewState = addon.IsInstalled ? "installed" : "not_installed"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scanning addon {addon.Id}: {ex.Message}");
                addon.IsInstalled = false;
            }
        }

        /// <summary>
        /// دریافت تمام افزونه‌ها
        /// </summary>
        public ReadOnlyCollection<AddonInfo> GetAllAddons()
        {
            return _addons.AsReadOnly();
        }

        /// <summary>
        /// دریافت افزونه بر اساس شناسه
        /// </summary>
        public AddonInfo GetAddonById(string id)
        {
            return _addons.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// دریافت افزونه‌های فعال و نصب شده
        /// </summary>
        public List<AddonInfo> GetEnabledAddons()
        {
            return _addons.Where(a => a.IsEnabled && a.IsInstalled).ToList();
        }

        /// <summary>
        /// فعال/غیرفعال کردن افزونه
        /// </summary>
        public void SetAddonEnabled(string id, bool enabled)
        {
            var addon = GetAddonById(id);
            if (addon != null)
            {
                bool wasEnabled = addon.IsEnabled;
                addon.IsEnabled = enabled;

                if (wasEnabled != enabled)
                {
                    OnAddonStateChanged?.Invoke(this, new AddonStateChangedEventArgs
                    {
                        Addon = addon,
                        PreviousState = wasEnabled ? "enabled" : "disabled",
                        NewState = enabled ? "enabled" : "disabled"
                    });

                    _ = SaveAddonsAsync();
                }
            }
        }

        /// <summary>
        /// تغییر ترتیب نمایش افزونه
        /// </summary>
        public void SetAddonDisplayOrder(string id, int order)
        {
            var addon = GetAddonById(id);
            if (addon != null)
            {
                addon.DisplayOrder = order;
                _addons.Sort((a, b) => a.DisplayOrder.CompareTo(b.DisplayOrder));
                OnAddonsUpdated?.Invoke(this, EventArgs.Empty);
                _ = SaveAddonsAsync();
            }
        }

        /// <summary>
        /// بروزرسانی تنظیمات افزونه
        /// </summary>
        public void UpdateAddonSettings(string id, Dictionary<string, object> settings)
        {
            var addon = GetAddonById(id);
            if (addon != null)
            {
                addon.Settings = settings;
                addon.LastUpdated = DateTime.Now;
                _ = SaveAddonsAsync();
            }
        }

        /// <summary>
        /// دریافت endpoint API افزونه
        /// </summary>
        public string GetAddonApiEndpoint(string id)
        {
            var addon = GetAddonById(id);
            if (addon != null && addon.ApiPort > 0)
            {
                return $"http://127.0.0.1:{addon.ApiPort}";
            }
            return null;
        }

        /// <summary>
        /// افزودن افزونه سفارشی
        /// </summary>
        public void AddCustomAddon(AddonInfo addon)
        {
            if (addon == null) throw new ArgumentNullException(nameof(addon));
            
            var existing = GetAddonById(addon.Id);
            if (existing != null)
            {
                System.Diagnostics.Debug.WriteLine($"Addon with ID {addon.Id} already exists");
                return;
            }

            _addons.Add(addon);
            _ = ScanAddonAsync(addon);
            OnAddonsUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// حذف افزونه سفارشی
        /// </summary>
        public void RemoveCustomAddon(string id)
        {
            var addon = GetAddonById(id);
            if (addon != null)
            {
                _addons.Remove(addon);
                OnAddonsUpdated?.Invoke(this, EventArgs.Empty);
                _ = SaveAddonsAsync();
            }
        }
    }

    /// <summary>
    /// آرگومان‌های رویداد تغییر وضعیت افزونه
    /// </summary>
    public class AddonStateChangedEventArgs : EventArgs
    {
        public AddonInfo Addon { get; set; }
        public string PreviousState { get; set; }
        public string NewState { get; set; }
    }
}
