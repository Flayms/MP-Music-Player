using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class BigTrackPage : ContentPage {
  public BigTrackPage(BigTrackViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}