using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class TrackDetailsPage : ContentPage {
  public TrackDetailsPage(TrackDetailsViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}