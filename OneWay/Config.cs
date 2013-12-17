namespace OneWay
{
    public class Config
    {
        public static Config Instance { get { return new Config(); } }

		public string EncryptionKey = "<RSAKeyValue><Modulus>0F1piMadd0O1w7fJ40ROVXsYOxSP+4FDyi4PduUz7AeGJ/B+hUiRA/EDaFdx0VxQuaQhPhjMjM5BmQapza+AwHz25vig0PoxLq+z9YgDsTB/+oKQYTWJXxLecOaHJ/kAoRBqJ8Hm6xtnyOOzMr/VzHmKdnhq9AAKzYSSZ5088pILHtg9IHPZk19ebuh5klyHq11R2M4CTXV40TFMiFPh/K5Z3r8GUTHx/t/bPuOSlO7acaUaWKq7SS5OCU70gli+MKkAlYxgr8tbPmQJkzksC/Cw1eulWNLqTu5sBhPG3GREA0eO1kEFdqGC1GmBahHf4tUQLkq4Ex2T2v0FhH6id68lPUblStBRLw1dxPbr4D8OdtU/qAlA9z8RvyU5B6dtJHqN30ZAvCeJQcgD/jOt05ern1122rMu+5bCNGY1hc+Vh9OjTkNLA59u/BRGr4URp10YMTVZz2NgQRHGnTnfJuBmrzMt3s9hr/PMvjIaey3AMourlSdipMR778rZe2W5Lt9wL9ohuwXPHYorDb56zdYCjKMzNAdYBKU5LmsBCiP4Znxe3yJTtw4RDXFUSoUP8jcyl2J/e78Ee+DEJyLKBs1EZeIuomravscF/cIU6dnjjzfHpQfybvfARvSQO7vaMmmrDjKOo6D0m8eV4kWu3svUj/YkX0bQguDFpPh8Sm8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		public string DefaultFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
    }
}
