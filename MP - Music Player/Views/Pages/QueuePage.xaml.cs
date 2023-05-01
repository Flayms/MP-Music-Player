using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

/// <summary>
/// A page displaying the current playback-queue, consisting of the current track, next up tracks and next queued tracks.
/// </summary>
public partial class QueuePage : ContentPage {
  public QueuePage(QueueViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
