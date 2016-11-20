using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

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
            var isDataLoad = false;
            byte[] byteArray = null;
            if (pathLabel?.Content != null && thisCanvas != null)
            {
                var path = pathLabel.Content.ToString();
                try
                {
                    byteArray = File.ReadAllBytes(path);
                    isDataLoad = true;
                }
                catch (Exception)
                {
                    MessageBox.Show($@"failed with open file '{path}' ");
                }
            }

            if (isDataLoad)
            {
                ImageBrush brush = new ImageBrush();

                using (var stream = new MemoryStream(byteArray))
                {
                    try
                    {
                        brush.ImageSource = BitmapFrame.Create
                            (stream,
                                BitmapCreateOptions.IgnoreImageCache,
                                BitmapCacheOption.OnLoad
                            );
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(@" failed with load file, possible is not correct file format ");
                    }
                }

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


        public static IEnumerable<byte[]> ReadChunks(string path)
        {
            var lengthBytes = new byte[sizeof(int)];

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int n = fs.Read(lengthBytes, 0, sizeof(int));  // Read block size.

                if (n == 0)      // End of file.
                    yield break;

                if (n != sizeof(int))
                    throw new InvalidOperationException("Invalid header");

                int blockLength = BitConverter.ToInt32(lengthBytes, 0);
                var buffer = new byte[blockLength];
                n = fs.Read(buffer, 0, blockLength);

                if (n != blockLength)
                    throw new InvalidOperationException("Missing data");

                yield return buffer;
            }
        }



    }
}
