using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace OneWay
{
	[Activity]
	[IntentFilter(new[] {"com.google.zxing.client.android.SCAN"})]
	public class ConfigureActivity : Activity
	{
		public static readonly string Key = "Key";

		TextView txtPublicKey { get { return FindViewById<TextView>(Resource.Id.txtPublicKey); } }
		Button cmdSubmit { get { return FindViewById<Button>(Resource.Id.cmdSubmit); } }
		Button cmdUseQrCode { get { return FindViewById<Button>(Resource.Id.cmdUseQrCode); } }


		protected override void OnCreate(Bundle savedInstance)
		{
			base.OnCreate(savedInstance);
			SetContentView(Resource.Layout.ConfigureLayout);

			var key = Intent.GetStringExtra(Key);
			txtPublicKey.Text = key;

			cmdSubmit.Click += HandleSubmit;
			cmdUseQrCode.Click += HandleUseQRCode;
		}

		void HandleUseQRCode (object sender, EventArgs e)
		{
			Intent intent = new Intent("com.google.zxing.client.android.SCAN");
			intent.PutExtra("SCAN_MODE", "QR_CODE_MODE");
			intent.PutExtra("SAVE_HISTORY", false);

			StartActivityForResult(intent, 0);		
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (resultCode == Result.Ok)
			{
				var key = data.Extras.GetString("SCAN_RESULT");
				Intent.PutExtra(Key, key);
				SetResult(Result.Ok, Intent);
				Finish();
			}
		}

		void HandleSubmit (object sender, EventArgs e)
		{
			Intent.PutExtra(Key, txtPublicKey.Text);
			SetResult(Result.Ok, Intent);
			Finish();
		}
    }
}

