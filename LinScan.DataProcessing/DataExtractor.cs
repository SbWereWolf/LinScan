using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace LinScan.DataProcessing
{
    public class DataExtractor
    {
        private readonly int _linesToRead; // = 131072;
        private readonly int _hexMask; // = 0x03FF;
        private readonly int _imageLineSize; // = 192;
        private readonly int _fileLineSize; //  = 240;
        private readonly int _bufferLimit; //  = 10;
        private readonly int _byteSizeInBits; // = 8;

        public DataExtractor( int linesToRead, int fileLineSize,int imageLineSize, int byteSizeInBits,int bufferLimit, int hexMask)
        {
            _linesToRead = linesToRead;
            _hexMask = hexMask;
            _imageLineSize = imageLineSize;
            _fileLineSize = fileLineSize;
            _bufferLimit = bufferLimit;
            _byteSizeInBits = byteSizeInBits;
        }

        public ImageBrush GetBrushFromFile(string path)
        {
            var pixels = GetPixels(path);
            var bitmap = GetBitmap(pixels);
            var brush = GetImageBrush(bitmap);

            return brush;
        }

        private ImageBrush GetImageBrush(Bitmap bmp)
        {
            ImageBrush brush = null;
            if (bmp != null)
            {
                var second = CreateBitmapSourceFromGdiBitmap(bmp);
                brush = new ImageBrush(second);
            }
            return brush;
        }

        private static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            BitmapSource result = null;
            if (bitmap != null)
            {
                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                var bitmapData = bitmap.LockBits(
                    rect,
                    ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                try
                {
                    var size = (rect.Width * rect.Height) * 4;

                    if (bitmapData != null)
                    {
                        result = BitmapSource.Create(
                            bitmap.Width,
                            bitmap.Height,
                            bitmap.HorizontalResolution,
                            bitmap.VerticalResolution,
                            PixelFormats.Bgra32,
                            null,
                            bitmapData.Scan0,
                            size,
                            bitmapData.Stride);}
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }

            return result;
        }

        private Bitmap GetBitmap(int[] pixelValues)
        {
            Bitmap bmp = null;
            if (pixelValues != null)
            {
                bmp = new Bitmap(_imageLineSize, _linesToRead);

                for (var lineNumber = 0; lineNumber < _linesToRead; lineNumber++)
                {
                    for (var linePosition = 0; linePosition < _imageLineSize; linePosition++) 
                    {
                        long index = linePosition + _imageLineSize*lineNumber;
                        var pixelValue = pixelValues[index];

                        var colorIndex = ConvertIntToColor(pixelValue, _hexMask);
                        var color = Color.FromArgb(colorIndex, colorIndex, colorIndex);
                        bmp.SetPixel(linePosition, lineNumber, color );
                    }
                }
            }
            return bmp;
        }

        private static int ConvertIntToColor(int pixelValues, int hexMask)
        {
            var result = (int)Math.Truncate((double)pixelValues* byte.MaxValue / hexMask);
            return result;
        }

        private int[] GetPixels(string path)
        {
            int[] pixelValues = null;
            if (!string.IsNullOrWhiteSpace(path))
            {
                
                var chunk = new byte[_fileLineSize];
                var unpackedData = new List<int>();

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bytesNumber = _fileLineSize;
                    var counter = 0;
                    while (bytesNumber > 0 && counter < _linesToRead)
                    {
                        counter++;
                        bytesNumber = fs.Read(chunk, 0, _fileLineSize);

                        UnpackData(chunk, unpackedData);
                    }
                }

                pixelValues = unpackedData.ToArray();
            }
            return pixelValues;
        }

        private void UnpackData(byte[] chunk, ICollection<int> unpackedData)
        {
            var buffer = 0;
            var bufferSize = 0;
            if (chunk != null)
                foreach (var nextByte in chunk)
                {
                    var nextPie = nextByte << bufferSize;
                    buffer += nextPie;
                    bufferSize += _byteSizeInBits;

                    if (bufferSize >= _bufferLimit)
                    {
                        var portion = buffer & _hexMask;
                        unpackedData?.Add(portion);
                        buffer >>= _bufferLimit;
                        bufferSize -= _bufferLimit;
                    }
                }
        }
    }
}
