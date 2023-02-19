using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class TrackDetailsPage : ContentPage {
  public TrackDetailsPage(TrackDetailsViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}