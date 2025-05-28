using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniaTodo.Services;
using AvaloniaTodo.ViewModels;
using AvaloniaTodo.Views;

namespace AvaloniaTodo
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private readonly MainWindowViewModel _mainWindowViewModel = new MainWindowViewModel();

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = _mainWindowViewModel,
                };

                desktop.ShutdownRequested += DesktopOnShutdownRequested;
            }
            await InitMainViewModelAsync();
            base.OnFrameworkInitializationCompleted();
        }

        private bool _canClose;
        private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            e.Cancel = !_canClose;

            if (!_canClose)
            {
                var itemsToSave = _mainWindowViewModel.ToDoItems.Select(item => item.GetToDoItem());
                await ToDoListFileService.SaveFileToFileAsync(itemsToSave);

                _canClose = true;

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            }
        }

        private async Task InitMainViewModelAsync()
        {
            var itemsLoaded = await ToDoListFileService.LoadFromFileAsync();

            if (itemsLoaded != null)
            {
                foreach (var item in itemsLoaded)
                {
                    _mainWindowViewModel.ToDoItems.Add(new ToDoItemViewModel(item));
                }
            }
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}