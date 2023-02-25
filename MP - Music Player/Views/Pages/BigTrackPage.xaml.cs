using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class BigTrackPage : ContentPage {
  public BigTrackPage(BigTrackViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}