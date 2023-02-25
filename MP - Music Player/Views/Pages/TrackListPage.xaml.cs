using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class TrackListPage : ContentPage {
  public TrackListPage(TrackListViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}