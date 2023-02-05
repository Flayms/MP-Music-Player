namespace Music_Player_Maui;

public partial class AppShell : Shell {
  public AppShell() {
    this.InitializeComponent();

#if ANDROID || IOS
    this.FlyoutBehavior = FlyoutBehavior.Flyout;
#else
    this.FlyoutBehavior = FlyoutBehavior.Locked;
#endif
  }
}
