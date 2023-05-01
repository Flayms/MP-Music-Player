using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

/// <summary>
/// The page that gets displayed when a specific track is clicked.
/// It displays the information about that track bigger, as well as having all the playback controls.
/// </summary>
public partial class BigTrackPage : ContentPage {
  public BigTrackPage(BigTrackViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}