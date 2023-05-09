using MP_Music_Player.Views.Pages;

namespace MP_Music_Player;

public partial class AppShell : Shell {
  public AppShell() {
    this.InitializeComponent();

    Routing.RegisterRoute(nameof(QueuePage), typeof(QueuePage));

#if ANDROID || IOS
    this.FlyoutBehavior = FlyoutBehavior.Flyout;
#else
    this.FlyoutBehavior = FlyoutBehavior.Locked;
#endif
  }

  //necessary because maui flyoutBackgroundColor is bugged 
  protected override async void OnAppearing() {
    base.OnAppearing();

    await Task.Delay(2000);

    var color = this.FlyoutBackgroundColor;
    this.FlyoutBackgroundColor = Colors.DarkBlue;
    this.FlyoutBackgroundColor = color;
  }

}
