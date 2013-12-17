using System;
using System.IO;
using Android.Widget;
using Android.App;
using Android.Content;
using Android.OS;
using System.Security.Cryptography;
using System.Threading;
using Android.Util;
using Android.Media;
using Android.Views;

namespace OneWay
{
	[Activity(Label = "OneWay", MainLauncher = true)]
	public class MainActivity : Activity
	{
		const int REQUEST_BROWSEFILE = 23;
		const int REQUEST_CONFIGURE = 24;

		Button cmdSelectFile { get { return FindViewById<Button>(Resource.Id.cmdSelectFile); } }
		TextView txtStatus { get { return FindViewById<TextView>(Resource.Id.txtStatus); } }
		Button cmdEncryptFile { get { return FindViewById<Button>(Resource.Id.cmdEncryptFile); } }

		string fileToEncrypt;
		Config configuration;

		protected override void OnCreate(Bundle savedInstance)
		{
			base.OnCreate(savedInstance);
			SetContentView(Resource.Layout.Main);

			var prefs = GetPreferences(FileCreationMode.Private);
			configuration = new Config()
			{
				DefaultFolder = prefs.GetString("DefaultFolder", Android.OS.Environment.ExternalStorageDirectory.AbsolutePath),
				EncryptionKey = prefs.GetString("EncryptionKey", ""),
			};

			cmdSelectFile.Click += cmdSelectFile_Click;
			cmdEncryptFile.Click += cmdEncryptFile_Click;
		}

		public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.menuConfigure:
					Configure();
					break;
			}
			return base.OnOptionsItemSelected(item);
		}

		protected override void OnResume()
		{
			base.OnResume();

			if (string.IsNullOrEmpty(configuration.EncryptionKey))
			{
				Configure();
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == REQUEST_BROWSEFILE && resultCode == Result.Ok)
			{
				fileToEncrypt = data.GetStringExtra(OpenFileActivity.ResultPath);
				txtStatus.Text = fileToEncrypt;
				cmdEncryptFile.Visibility = Android.Views.ViewStates.Visible;
			}

			if (requestCode == REQUEST_CONFIGURE && resultCode == Result.Ok)
			{
				var key = data.GetStringExtra(ConfigureActivity.Key);
				if (!Crypto.IsValidKey(key))
				{
					Toast.MakeText(this, "The key submitted is not valid", ToastLength.Long).Show();
					return;
				}

				configuration.EncryptionKey = key;

				var prefs = GetPreferences(FileCreationMode.Private);
				var prefsEditor = prefs.Edit();
				prefsEditor.PutString("EncryptionKey", configuration.EncryptionKey);
				prefsEditor.Commit();
			}
		}

		void cmdSelectFile_Click(object sender, EventArgs e)
		{
			var openFileActivity = new Intent(this, typeof(OpenFileActivity));
			openFileActivity.PutExtra(OpenFileActivity.StartPath, configuration.DefaultFolder);
			StartActivityForResult(openFileActivity, REQUEST_BROWSEFILE);
		}

		void cmdEncryptFile_Click(object sender, EventArgs e)
		{
			var pd = new ProgressDialog(this);

			var t = new Thread(() =>
				{
					try
					{
						RunOnUiThread(() =>
						{
							pd.SetTitle("Encrypting...");
							pd.Show();
						});

						var fileInfoInput = new FileInfo(fileToEncrypt);
						if (!fileInfoInput.Exists)
						{
							throw new Exception("File does not exist");
						}

						if ((fileInfoInput.Attributes & FileAttributes.ReadOnly) > 0)
						{
							throw new Exception("Source file is read only");
						}

						var outputFileName = fileToEncrypt + ".ascr";
						if (File.Exists(outputFileName))
						{
							var fileInfoOutput = new FileInfo(outputFileName);
							if ((fileInfoOutput.Attributes & FileAttributes.ReadOnly) > 0)
							{
								throw new Exception("Output file is read only");
							}
						}

						using (var input = fileInfoInput.OpenRead())
						using (var output = File.OpenWrite(outputFileName))
						using (var alg = Crypto.CreateRsaProvider(configuration.EncryptionKey))
						{
							Crypto.Encrypt(input, output, alg);
						}

						SendBroadcast(new Intent(Intent.ActionMediaScannerScanFile, Android.Net.Uri.Parse("file://" + outputFileName)));

						RunOnUiThread(() => pd.SetTitle("Shredding original..."));

						long fileSize = fileInfoInput.Length;
						using (var shred = fileInfoInput.OpenWrite())
						{
							var buffer = Crypto.GetRandomBytes(1024);
							for (long i = 0; i < fileSize; i += 1024)
							{
								shred.Write(buffer, 0, buffer.Length);
							}
						}
						fileInfoInput.Delete();

						SendBroadcast(new Intent(Intent.ActionMediaScannerScanFile, Android.Net.Uri.Parse("file://" + fileToEncrypt)));

						RunOnUiThread(() =>
							{
								pd.Dismiss();

								txtStatus.Text = "";
								fileToEncrypt = "";
								cmdEncryptFile.Visibility = Android.Views.ViewStates.Invisible;

								var msg = string.Format("file {0} successfully encrypted and original was shredded", outputFileName);
								Toast.MakeText(this, msg, ToastLength.Long).Show();
							});
					}
					catch (Exception ex)
					{
						RunOnUiThread(() =>
							{
								pd.Dismiss();
								Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
							});

						Log.Error("EncryptFile", ex.ToString());
					}
				});

			t.Start();
		}

		void Configure()
		{
			var configureActivity = new Intent(this, typeof(ConfigureActivity));
			configureActivity.PutExtra(ConfigureActivity.Key, configuration.EncryptionKey);
			StartActivityForResult(configureActivity, REQUEST_CONFIGURE);
		}	
	}
}


