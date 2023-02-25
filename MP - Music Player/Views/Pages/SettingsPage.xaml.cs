using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class SettingsPage : ContentPage {
  public SettingsPage(SettingsViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
