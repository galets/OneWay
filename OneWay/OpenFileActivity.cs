using System.Linq;
using Android.App;
using Android.Widget;
using Android.Util;
using Android.OS;
using System.IO;

namespace OneWay
{
	[Activity]
	public class OpenFileActivity : Activity
    {
        public const string StartPath = "StartPath";
        public const string ResultPath = "ResultPath";
        public const string Root = "/";

		private FileSystemInfo[] entries;
        private string startPath;
		private string currentPath;

        protected override void OnCreate(Bundle savedInstance)
        {
            base.OnCreate(savedInstance);
			SetResult(Result.Canceled);
			SetContentView(Resource.Layout.OpenFileLayout);

			var list = FindViewById<ListView>(Resource.Id.list);
            list.ItemClick += OnItemClick;

			startPath = Intent.GetStringExtra(StartPath) ?? Root;
			var dir = new DirectoryInfo(startPath);
			SetPath(dir.FullName);
        }

        void OnItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            var entry = entries[e.Position];
			if ((entry.Attributes & FileAttributes.Directory) != 0)
            {
				SetPath(entry.FullName);
            }
            else
            {
				Intent.PutExtra(ResultPath, entry.FullName);
				SetResult(Result.Ok, Intent);
                Finish();
            }
        }

		public override void OnBackPressed()
		{
			var dir = new DirectoryInfo(currentPath);
			if (dir.FullName == Root)
			{
				base.OnBackPressed();
			}
			else
			{
				SetPath(dir.Parent.FullName);
			}
		}

        private void SetPath(string path)
        {
			var lbPath = FindViewById<TextView>(Resource.Id.lbPath);
			lbPath.Text = currentPath = path;

            entries = GetEntries(path);
			var names = entries.Select(x =>
				((x.Attributes & FileAttributes.Directory) != 0) ? "/" + x.Name :
				((x.Attributes & FileAttributes.ReadOnly) != 0) ? "RO: " + x.Name : x.Name
			).ToArray();

			var list = FindViewById<ListView>(Resource.Id.list);
			list.Adapter = new ArrayAdapter<string>(this, Resource.Layout.OpenFileRowLayout, names);
            
        }

		private FileSystemInfo[] GetEntries(string path)
        {
			var dir = new DirectoryInfo(path);
			if (dir.Exists)
            {
				var entries = dir.GetDirectories().Cast<FileSystemInfo>().Concat(dir.GetFiles().Cast<FileSystemInfo>())
					.Where(f => 0 == (f.Attributes & (FileAttributes.Hidden | FileAttributes.System)))
					.Where(f => !f.Name.EndsWith(".ascr", System.StringComparison.CurrentCultureIgnoreCase));
                if (entries != null)
                {
					return entries.OrderBy(x => x.Name).ToArray();
                }
            }
			return new FileSystemInfo[0];
        }
    }
}
