using Music_Player_Maui.Services;
using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.UserControls;

public partial class BottomTrackPlayerView : ContentView {
  public BottomTrackPlayerView() {
    this.InitializeComponent();

    var viewModel = ServiceHelper.GetService<TrackPlayerViewModel>(); //todo: find better way

    //workaround bc dependency injection for controls inside contentPages currently doesn't work yet
    this.BindingContext = viewModel;
  }
}
