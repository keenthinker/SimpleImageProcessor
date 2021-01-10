using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace SimpleImageProcessor
{
	/// <summary>
	/// Image processor
	/// </summary>
    public class ImageProcessor
    {
		private const int WIDTH = 1600;
		private const int HEIGHT = 1280;

		/// <summary>
		/// Apply the image transformation
		/// </summary>
		/// <param name="file">The full path of the image file that should be processed</param>
		/// <param name="directory">The directory in which the new image should be saved (under the name name as the original image)</param>
		/// <param name="cornerRadius">The radius of the rounding of the corners</param>
		public void AddRoundCornersAndSave(string file, string directory, int cornerRadius)
		{
			var fileInfo = new FileInfo(file);
			using (var image = Image.FromFile(fileInfo.FullName))
			{
				var roundedImage = addTransparentRoundCorners(image, cornerRadius);
				var toLocation = Path.Combine(directory, fileInfo.Name);
				roundedImage.Save(toLocation);
				Console.WriteLine($"Saved '{fileInfo.Name}' to '{toLocation}'");
			}
		}

		/// <summary>
		/// Creates an image with rounded corners and a color border from the specified source image. 
		/// 
		/// The code is a mixture of this two SO posts: 
		/// * https://stackoverflow.com/a/20757041
		/// * https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
		/// </summary>
		/// <param name="image">The source image</param>
		/// <param name="cornerRadius">Rounding radius</param>
		/// <returns>The result (a new image) of the applied transformation on the original image</returns>
		private Image addTransparentRoundCorners(Image image, int cornerRadius)
		{
            static Tuple<int, int> swap(int v1, int v2) => new Tuple<int, int>(v2, v1);
			// always stretch
            static Tuple<int, int> stretch(int currentWidth, int currentHeight)
            {
                var maxWidth = WIDTH;
                var maxHeight = HEIGHT;
                if (currentWidth < currentHeight)
                {
                    var s = swap(maxWidth, maxHeight);
                    maxWidth = s.Item1;
                    maxHeight = s.Item2;
                }
                var width = maxWidth;
                var height = maxHeight;
                return new Tuple<int, int>(width, height);
            }

            cornerRadius *= 2;

			var imageSize = stretch(image.Width, image.Height);
			var destinationRectangle = new Rectangle(0, 0, imageSize.Item1, imageSize.Item2);

			Bitmap roundedImage = new Bitmap(imageSize.Item1, imageSize.Item2);
			roundedImage.SetResolution(image.Width, image.Height);
			// path for the rounded corners
			GraphicsPath gp = new GraphicsPath();
			gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
			gp.AddArc(0 + roundedImage.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
			gp.AddArc(0 + roundedImage.Width - cornerRadius, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
			gp.AddArc(0, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
			// path and pen for the color border
			var gpBorder = (GraphicsPath)gp.Clone();
			gpBorder.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
			var penGray = new Pen(Color.Gray, 7.5f);
			// transform
			using (Graphics g = Graphics.FromImage(roundedImage))
			{
				g.CompositingMode = CompositingMode.SourceCopy;
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;

				g.SetClip(gp);
				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					g.DrawImage(image, destinationRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
				g.DrawPath(penGray, gpBorder);
			}
			return roundedImage;
		}
	}
}
