using System;
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
    internal static class Handler
    {
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

        public static void SetUpMainGrid(Grid mainGrid)
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
                        MessageBox.Show(@" failed with load file, possible file is not correct image file ");
                    }
                }

                thisCanvas.Background = brush;

                thisCanvas.Children?.Clear();
            }
        }

        public static void DrawRectangle(MouseEventArgs e, Point point, RectangleAndPoint data )
        {

            if(data != null && e != null )
            {
                var rect = data.Rect;
                var startPoint = data.StartPoint;

                var mayPaint = e.LeftButton != MouseButtonState.Released && rect != null;

                if (mayPaint)
                {
                    var x = Math.Min(point.X, startPoint.X);
                    var y = Math.Min(point.Y, startPoint.Y);

                    var w = Math.Max(point.X, startPoint.X) - x;
                    var h = Math.Max(point.Y, startPoint.Y) - y;

                    rect.Width = w;
                    rect.Height = h;

                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                }
            }
        }

        public static Point ShowPoint(Canvas canvas, DataGrid cursorLocationDataGrid)
        {
            var point = Mouse.GetPosition(canvas);

            point.X = Math.Truncate(point.X);
            point.Y = Math.Truncate(point.Y);

            if (cursorLocationDataGrid != null)
            {
                cursorLocationDataGrid.ItemsSource = new[] {point};
            }
            return point;
        }

        public static RectangleAndPoint SetRectangle(Canvas canvas)
        {
            var startPoint = Mouse.GetPosition(canvas);
            var rect = new Rectangle
            {
                Stroke = Brushes.Crimson,
                StrokeThickness = 1
            };
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.X);
            canvas?.Children?.Add(rect);

            RectangleAndPoint data = new RectangleAndPoint() { Rect = rect , StartPoint = startPoint };
            return data;
        }

        public class RectangleAndPoint
        {
            public Point StartPoint;
            public Rectangle Rect;
        }
    }
}
