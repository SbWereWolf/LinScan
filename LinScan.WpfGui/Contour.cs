using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LinScan
{
    public class Contour
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }

        private int DrawCrossLimit { get; } 

        public Contour( int drawCrossLimit)
        {
            DrawCrossLimit = drawCrossLimit;
        }

        private Contour Copy()
        {
            var copy = new Contour(this.DrawCrossLimit)
            {
                Y = this.Y,
                Width = this.Width,
                Length = this.Length,
                X = this.X
            };

            return copy;
        }

        public void AddToView(List<Contour> contourList, DataGrid contourDataGrid)
        {
            var contourCopy = this.Copy();
            contourList?.Add(contourCopy);

            if (contourDataGrid != null && contourList != null)
            {
                contourDataGrid.ItemsSource = contourList.ToArray();
            }
        }

        public bool IsDrawCross()
        {
            var letDrawCross = this.Length < DrawCrossLimit || this.Width < DrawCrossLimit;
            
            return letDrawCross;
        }

        public void AddToCanvasAsCross(Canvas canvas)
        {
            var cross = this.GetCross(canvas);

            if (cross != null && canvas != null)
            {
                foreach (var line in cross)
                {
                    if (line != null)
                    {
                        canvas.Children?.Add(line);
                    }
                }
            }
        }
        private List<Line> GetCross(Canvas canvas)
        {
            double upLeftX = 0;
            double upLeftY = 0;
            double width = 0;
            double height = 0;
            var letCalculateCross = false;
            if (canvas != null)
            {
                upLeftX = this.X;
                upLeftY = this.Y;
                width = this.Width;
                height = this.Length;

                letCalculateCross = true;
            }

            List<Line> cross = null;
            if (letCalculateCross)
            {
                var crossX = upLeftX + Math.Truncate(width / 2);
                var crossY = upLeftY + Math.Truncate(height / 2);

                var upRightX = upLeftX + width;
                var bottomLeftY = upLeftY + height;

                cross = GetLinesForCross
                    (
                        crossX,
                        upLeftY,
                        bottomLeftY,
                        upLeftX,
                        crossY,
                        upRightX
                    );
            }
            return cross;
        }

        private static List<Line> GetLinesForCross(double crossX, double upLeftY, double bottomLeftY, double upLeftX, double crossY,
            double upRightX)
        {
            var cross = new List<Line>();

            const int crossSize = 3;
            var crossColor = Brushes.Red;
            const int crossThickness = 1;

            // Top
            var line = new Line
            {
                X1 = crossX,
                Y1 = upLeftY,
                X2 = crossX,
                Y2 = upLeftY - crossSize,
                Stroke = crossColor,
                StrokeThickness = crossThickness
            };
            cross.Add(line);

            // Bottom
            line = new Line
            {
                X1 = crossX,
                Y1 = bottomLeftY,
                X2 = crossX,
                Y2 = bottomLeftY + crossSize,
                Stroke = crossColor,
                StrokeThickness = crossThickness
            };
            cross.Add(line);

            // Left
            line = new Line
            {
                X1 = upLeftX,
                Y1 = crossY,
                X2 = upLeftX - crossSize,
                Y2 = crossY,
                Stroke = crossColor,
                StrokeThickness = crossThickness
            };
            cross.Add(line);

            // Right
            line = new Line
            {
                X1 = upRightX,
                Y1 = crossY,
                X2 = upRightX + crossSize,
                Y2 = crossY,
                Stroke = crossColor,
                StrokeThickness = crossThickness
            };
            cross.Add(line);
            return cross;
        }
    }
}
