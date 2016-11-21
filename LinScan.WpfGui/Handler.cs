using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using PixelFormat = System.Windows.Media.PixelFormat;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace LinScan
{
    public static class Handler
    {

        public class RectangleAndPoint
        {
            public Point StartPoint { get; set; }
            public Rectangle Rect { get; set; } = new Rectangle();
        }

        public static bool SetDataFileName(Label pathLabel)
        {
            var result = false;

            var openFileDialog = new OpenFileDialog();
            var wasFileSelected = openFileDialog.ShowDialog();

            var filePath = string.Empty;
            if (wasFileSelected == true && openFileDialog.FileName != null)
            {
                filePath = openFileDialog.FileName;
            }

            if (!string.IsNullOrWhiteSpace(filePath) && pathLabel != null)
            {
                pathLabel.Content = openFileDialog.FileName;
                result = true;
            }

            return result;
        }

        public static void SetMainGridView(Grid mainGrid)
        {
            const int toolbarMainGridRowIndex = 0;
            const int dataMainGridRowIndex = 2;
            const int dataMainGridColumnIndex = 0;
            const int informationMainGridColumnIndex = 2;
            const int toolbarInitialSize = 2;
            const int maingridInitialSizeInRow = 15;
            const int dataInitialSize = 3;
            const int maingridInitialSizeInColumn = 5;
            const int dataRowInitialSize = maingridInitialSizeInRow - toolbarInitialSize;
            const int informationColumnInitialSize = maingridInitialSizeInColumn - dataInitialSize;


            if (mainGrid != null)
            {
                var mainGridRowDefinitions = mainGrid.RowDefinitions;
                var mainGridColumnDefinitions = mainGrid.ColumnDefinitions;

                var toolbarDefinition = mainGridRowDefinitions[toolbarMainGridRowIndex];
                var dataRowDefinition = mainGridRowDefinitions[dataMainGridRowIndex];
                var dataColumnDefinition = mainGridColumnDefinitions[dataMainGridColumnIndex];
                var informationDefinition = mainGridColumnDefinitions[informationMainGridColumnIndex];

                if (toolbarDefinition != null
                    && dataRowDefinition != null
                    && dataColumnDefinition != null
                    && informationDefinition != null)
                {
                    const GridUnitType star = GridUnitType.Star;

                    toolbarDefinition.Height = new GridLength(toolbarInitialSize, star);
                    dataRowDefinition.Height = new GridLength(dataRowInitialSize, star);
                    dataColumnDefinition.Width = new GridLength(dataInitialSize, star);
                    informationDefinition.Width = new GridLength(informationColumnInitialSize, star);
                }
            }
        }

        public static void LoadData(Label pathLabel, Canvas thisCanvas)
        {
            string path = null;
            if (pathLabel?.Content != null && thisCanvas != null)
            {
                path = pathLabel.Content.ToString();

            }

            if ( !string.IsNullOrWhiteSpace(path)) 
            {
                const int fileLineSize = 240;
                const int linesToRead = 131072;
                const int hexMask = 0x03FF;

                var chunk = new byte[fileLineSize];
                var unpackData = new List<int>();

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bytesNumber = fileLineSize;
                    var counter = 0;
                    int buffer =0;
                    var bufferSize = 0;
                    const int bufferLimit = 10;
                    const int byteSize = 8;


                    

                    while ( bytesNumber > 0 && counter < linesToRead)
                    {
                        counter++;
                        bytesNumber = fs.Read(chunk, 0, fileLineSize);

                        foreach (byte nextByte in chunk)
                        {
                            var nextPie = nextByte << bufferSize;
                            buffer += nextPie;
                            bufferSize += byteSize;

                            if ( bufferSize >= bufferLimit )
                            {
                                var portion = buffer & hexMask;
                                unpackData.Add(portion);
                                buffer >>= bufferLimit;
                                bufferSize -= bufferLimit;
                            }
                        }
                    }
                }

                const int imageLineSize = 192;
                int[] pixelValues = unpackData.ToArray();

                Bitmap bmp = new Bitmap(imageLineSize, linesToRead);

                for (int lineNumber = 0; lineNumber < linesToRead; lineNumber++)
                {
                    for (int linePosition = 0; linePosition < imageLineSize; linePosition++)
                    {
                        long index = linePosition + imageLineSize * lineNumber;
                        var color = (int)Math.Truncate((double)pixelValues[index] * 255 / hexMask);
                        bmp.SetPixel(linePosition, lineNumber, System.Drawing.Color.FromArgb(color, color, color));
                    }
                }

                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
                bmp.Dispose();
                var brush = new ImageBrush(bitmapSource);

                thisCanvas.Background = brush;


            }
        }



        public static void ClearCanvas(Canvas thisCanvas, DataGrid contourDataGrid, List<Contour> contourList)
        {
            thisCanvas?.Children?.Clear();
            if (contourDataGrid != null)
            {
                contourDataGrid.ItemsSource = new List<Contour>();
            }
            contourList?.Clear();
        }

        public static Contour DrawRectangle(MouseEventArgs e, Point point, RectangleAndPoint data )
        {
            Contour contour = null;

            point.X = Math.Truncate(point.X);
            point.Y = Math.Truncate(point.Y);

            if (data != null)
            {
                var rect = data.Rect;
                var startPoint = data.StartPoint;
                var mayDraw = false;
                if (e != null )
                {
                    mayDraw = e.LeftButton != MouseButtonState.Released && rect != null;
                }

                double w = 0;
                double h = 0;
                double x = 0;
                double y = 0;
                if (mayDraw)
                {
                    x = Math.Min(point.X, startPoint.X);
                    y = Math.Min(point.Y, startPoint.Y);

                    w = Math.Max(point.X, startPoint.X) - x;
                    h = Math.Max(point.Y, startPoint.Y) - y;

                    rect.Width = w;
                    rect.Height = h;

                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);

                    w = Math.Truncate(w);
                    h = Math.Truncate(h);
                }

                if (w > 0 && h > 0)
                {
                    contour = new Contour
                    {
                        Width = w,
                        X = x,
                        Length = h,
                        Y = y
                    };
                }
            }

            return contour;
        }

        public static void ShowCanvasContour( DataGrid cursorLocationDataGrid, Contour contour)
        {

            if (cursorLocationDataGrid != null && contour != null )
            {
                cursorLocationDataGrid.ItemsSource = new[] { contour };
            }
        }

        public static RectangleAndPoint SetRectangle(Canvas canvas)
        {
            var startPoint = Mouse.GetPosition(canvas);

            startPoint.X = Math.Truncate(startPoint.X);
            startPoint.Y = Math.Truncate(startPoint.Y);

            var rect = new Rectangle
            {
                Stroke = Brushes.Crimson,
                StrokeThickness = 1
            };
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y); // X 
            canvas?.Children?.Add(rect);

            RectangleAndPoint data = new RectangleAndPoint
            {
                Rect = rect,
                StartPoint = startPoint
            };
            return data;
        }

        public static void SetContourModeView(Button contourModeToggleButton, bool contourMode)
        {
            if (contourModeToggleButton != null)
            {
                if (contourMode)
                {
                    contourModeToggleButton.Content = "ВКЛ";
                    contourModeToggleButton.Background = Brushes.DeepPink;
                }
                else
                {
                    contourModeToggleButton.Content = "ВЫКЛ";
                    contourModeToggleButton.Background = Brushes.Gold;
                }
            }
        }

        public static void AddContourToView(Contour contour, List<Contour> contourList, DataGrid contourDataGrid, Rectangle rectangle,
            Canvas canvas)
        {
            if (contour != null)
            {
                contour.AddToView( contourList, contourDataGrid);
            
                var letDrawCross = contour.IsDrawCross();

                if (letDrawCross && canvas != null)
                {
                    canvas.Children?.Remove(rectangle);
                    contour.AddToCanvasAsCross( canvas);
                }
            }
        }
    }
}
