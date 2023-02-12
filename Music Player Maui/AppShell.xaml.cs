using Music_Player_Maui.Views.Pages;

namespace Music_Player_Maui;

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

}
