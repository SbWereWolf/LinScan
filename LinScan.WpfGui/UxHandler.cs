using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LinScan.DataProcessing;
using Microsoft.Win32;

namespace LinScan
{
    public static class UxHandler
    {
        public const int LinesToRead = 131072;
        public const int ImageLineSize = 192;

        public static void LoadData(string path, Canvas thisCanvas)
        {
            const int hexMask = 0x03FF;
            const int fileLineSize = 240;
            const int bufferLimit = 10;
            const int byteSizeInBits = 8;

            var dataExtractor = new DataExtractor
                (
                LinesToRead,
                fileLineSize,
                ImageLineSize,
                byteSizeInBits,
                bufferLimit,
                hexMask
                );

            var brush = dataExtractor.GetBrushFromFile(path);

            if (brush != null && thisCanvas != null)
            {
                thisCanvas.Background = brush;
            }
        }
        public static string SetDataFileName(Label pathLabel)
        {
            var openFileDialog = new OpenFileDialog();
            var wasFileSelected = openFileDialog.ShowDialog();

            var filePath = string.Empty;
            if (wasFileSelected == true && openFileDialog.FileName != null)
            {
                filePath = openFileDialog.FileName;
            }

            if (!string.IsNullOrWhiteSpace(filePath) && pathLabel != null)
            {
                pathLabel.Content = filePath;
            }

            return filePath;
        }

        public static void AddContourToView(Contour contour, List<Contour> contourList, DataGrid contourDataGrid, Rectangle rectangle,
    Canvas canvas)
        {
            if (contour != null)
            {
                contour.AddToView(contourList, contourDataGrid);

                var letDrawCross = contour.IsDrawCross();

                if (letDrawCross && canvas != null)
                {
                    canvas.Children?.Remove(rectangle);
                    contour.AddToCanvasAsCross(canvas);
                }
            }
        }
        public static GuiHandler.RectangleAndPoint SetRectangle(Canvas canvas)
        {
            var startPoint = GetCursorPosition(canvas);

            var rect = new Rectangle
            {
                Stroke = Brushes.Crimson,
                StrokeThickness = 1
            };
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);
            canvas?.Children?.Add(rect);

            GuiHandler.RectangleAndPoint data = new GuiHandler.RectangleAndPoint
            {
                Rect = rect,
                StartPoint = startPoint
            };
            return data;
        }

        public static Point GetCursorPosition(Canvas canvas)
        {
            var point = Mouse.GetPosition(canvas);
            point = TruncatePoint(point);
            return point;
        }

        private static Point TruncatePoint(Point point)
        {
            point.X = Math.Truncate(point.X);
            point.Y = Math.Truncate(point.Y);
            return point;
        }
        public static Contour DrawRectangle(MouseEventArgs e, Point point, GuiHandler.RectangleAndPoint data)
        {
            Contour contour = null;

            point = TruncatePoint(point);

            if (data != null)
            {
                var rect = data.Rect;
                var startPoint = data.StartPoint;
                var mayDraw = false;
                if (e != null)
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
                    contour = new Contour(GuiHandler.DrawCrossLimit)
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
    }
}
