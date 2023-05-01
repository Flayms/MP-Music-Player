using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

//todo: remake more beautiful

/// <summary>
/// A page for all tag and meta information about the current track.
/// </summary>
public partial class TrackDetailsPage : ContentPage {
  public TrackDetailsPage(TrackDetailsViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}