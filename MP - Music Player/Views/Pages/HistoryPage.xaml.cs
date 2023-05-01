using MP_Music_PLayer.ViewModels;

namespace MP_Music_PLayer.Views.Pages;

//todo: really similar to TrackListPage

/// <summary>
/// Showing all the recently played tracks.
/// </summary>
public partial class HistoryPage : ContentPage {
  public HistoryPage(HistoryViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}