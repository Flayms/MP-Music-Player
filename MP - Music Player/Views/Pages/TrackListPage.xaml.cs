using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class TrackListPage : ContentPage {
  public TrackListPage(TrackListViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}