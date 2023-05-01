using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

/// <summary>
/// Page for the settings that the user can change.
/// </summary>
public partial class SettingsPage : ContentPage {
  public SettingsPage(SettingsViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
