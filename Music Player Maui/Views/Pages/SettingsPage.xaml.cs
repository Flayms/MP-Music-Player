using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class SettingsPage : ContentPage {
  public SettingsPage(SettingsViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
