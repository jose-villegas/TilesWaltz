using UnityEngine;
using ZXing;
using ZXing.QrCode;

namespace TilesWalk.General.QR
{
	public static class TextQRConverter
	{
		public static Color32[] Encode(string textForEncoding, int width, int height)
		{
			var writer = new BarcodeWriter
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Height = height,
					Width = width,
				}
			};

			return writer.Write(textForEncoding);
		}

		public static Texture2D GenerateTexture(string text)
		{
			var encoded = new Texture2D(256, 256);
			var color32 = Encode(text, encoded.width, encoded.height);
			encoded.SetPixels32(color32);
			encoded.Apply();
			return encoded;
		}
	}
}