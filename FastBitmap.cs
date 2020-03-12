using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Kard02.FastImaging
{
    /// <summary>
    /// Encapsulates <see cref="Bitmap"/> for better <c>GetPixel</c> and <c>SetPixel</c> performance. 
    /// Uses <see cref="PixelFormat.Format32bppArgb"/> internally.
    /// </summary>
    public class FastBitmap : IDisposable
    {
        /// <summary>
        /// Gets the width in pixels.
        /// </summary>
        public int Width { get => bitmap.Width; }
        /// <summary>
        /// Gets the height in pixels.
        /// </summary>
        public int Height { get => bitmap.Height; }
        /// <summary>
        /// Gets the width and height in pixels.
        /// </summary>
        public Size Size { get => bitmap.Size; }

        private Bitmap bitmap;
        private BitmapData bitmapData;
        private IntPtr scan0;
        private int stride;
        private bool lockedBits;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class from the specified existing bitmap.
        /// </summary>
        /// <param name="original">The <see cref="Bitmap"/> from which to create the new bitmap. Do not dispose if consumed.</param>
        /// <param name="consume">When false copies specified <see cref="Bitmap"/> instead of storing provided one.</param>
        public FastBitmap(Bitmap original, bool consume = true)
        {
            Bitmap clone = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            Bitmap target = consume ? original : new Bitmap(original);
            
            // Converting bitmap to argb format
            using (Graphics graphics = Graphics.FromImage(clone))
                graphics.DrawImage(target, new Rectangle(0, 0, clone.Width, clone.Height));

            bitmap = target;
            LockBits();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class with the specified size.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        public FastBitmap(int width, int height) : this(new Bitmap(width, height)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class from the specified file.
        /// </summary>
        /// <param name="filename">The bitmap file name and path.</param>
        public FastBitmap(string filename) : this(new Bitmap(filename)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class from the specified data stream.
        /// </summary>
        /// <param name="stream">The data stream used to load the image.</param>
        public FastBitmap(Stream stream) : this(new Bitmap(stream)) { }

        /// <summary>
        /// Bits must be locked for using <see cref="GetPixel"/> and <see cref="SetPixel"/> methods. This method is called by the constructor.
        /// </summary>
        public void LockBits()
        {
            if (!lockedBits)
            {
                bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                scan0 = bitmapData.Scan0;
                stride = bitmapData.Stride;
                lockedBits = true;
            }
        }

        /// <summary>
        /// Get underlying <see cref="Bitmap"/> object. Use <see cref="LockBits"/> method to lock bits again.
        /// </summary>
        /// <returns>Encapsulated bitmap.</returns>
        public Bitmap UnlockBits()
        {
            if (lockedBits)
                bitmap.UnlockBits(bitmapData);

            lockedBits = false;
            return bitmap;
        }

        /// <summary>
        /// Gets the color of the specified pixel in this <see cref="FastBitmap"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to retrieve.</param>
        /// <param name="y">The y-coordinate of the pixel to retrieve.</param>
        /// <returns>Color of the pixel.</returns>
        public FastColor GetPixel(int x, int y) => new FastColor(GetPixelPosition(x, y));
        /// <summary>
        /// Sets the color of the specified pixel in this <see cref="FastBitmap"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to set.</param>
        /// <param name="y">The y-coordinate of the pixel to set.</param>
        /// <param name="fastColor">The color to assign to the specified pixel.</param>
        public void SetPixel(int x, int y, FastColor fastColor) => Marshal.WriteInt32(GetPixelPosition(x, y), fastColor.ToInt());

        private IntPtr GetPixelPosition(int x, int y) => scan0 + (x * 4 + y * stride);

        #region IDisposable
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases all resources used by this <see cref="FastBitmap"/>.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    bitmap.Dispose();

                bitmapData = null;
                disposedValue = true;
            }
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~FastBitmap()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by this <see cref="FastBitmap"/>.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
