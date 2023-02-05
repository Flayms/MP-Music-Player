using Music_Player_Maui.Services;
using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.UserControls;

public partial class BottomTrackView : ContentView {
  public BottomTrackView() {
    this.InitializeComponent();

    var viewModel = ServiceHelper.GetService<TrackViewModel>(); //todo: find better way

    //workaround bc dependency injection for controls inside contentPages currently doesn't work yet
    this.BindingContext = viewModel;
  }
}
