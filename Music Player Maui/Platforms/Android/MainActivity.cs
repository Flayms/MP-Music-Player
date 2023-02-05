using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace Music_Player_Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity {

  public static MainActivity Instance { get; private set; }

  protected override void OnCreate(Bundle savedInstanceState) {
    Instance = this;

    base.OnCreate(savedInstanceState);
  }


  public void CheckAppPermissions() {
    if (Build.VERSION.SdkInt >= BuildVersionCodes.M) {
      if (!(this._CheckPermissionGranted(Manifest.Permission.ReadExternalStorage)
            && !this._CheckPermissionGranted(Manifest.Permission.WriteExternalStorage)))
        this._RequestPermission();
    }

    while (!this._CheckPermissionGranted(Manifest.Permission.WriteExternalStorage)
           || !this._CheckPermissionGranted(Manifest.Permission.ReadExternalStorage))
      Task.Delay(50);

    //if ((int)Build.VERSION.SdkInt < 23)
    //  return;

   // if (this.PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, this.PackageName) == Permission.Granted
   //     && this.PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, this.PackageName) == Permission.Granted)
   //   return;

    //var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
    //this.RequestPermissions(permissions, 1);
  }

  private void _RequestPermission() {
    ActivityCompat.RequestPermissions(this, new string[] {
      Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 0);
  }

  // Check if the permission is already available.
  private bool _CheckPermissionGranted(string Permissions)
    => ContextCompat.CheckSelfPermission(this, Permissions) == Permission.Granted;

}
