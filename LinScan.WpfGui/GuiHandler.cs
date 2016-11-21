using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace LinScan
{
    public static class GuiHandler
    {

        public class RectangleAndPoint
        {
            public Point StartPoint { get; set; }
            public Rectangle Rect { get; set; } = new Rectangle();
        }

        public const int DrawCrossLimit = 2;

        public static string LoadDataFile(Label pathLabel, Canvas thisCanvas)
        {
            var path = UxHandler.SetDataFileName(pathLabel);
            if (!string.IsNullOrWhiteSpace(path))
            {
                UxHandler.LoadData(path, thisCanvas);
            }

            return path;
        }

        public static void LoadData(Canvas canvas,string path)
        {
            UxHandler.LoadData(path, canvas);
        }
        public static GuiHandler.RectangleAndPoint SetRectangle(Canvas canvas)
        {
            return UxHandler.SetRectangle(canvas);
        }

        public static void AddContourToView(Contour contour, List<Contour> contourList, DataGrid contourDataGrid, Rectangle rectangle,
            Canvas canvas)
        {
            UxHandler.AddContourToView
                (
                    contour,
                    contourList,
                    contourDataGrid,
                    rectangle,
                    canvas
                );
        }

        public static Contour DrawRectangle(MouseEventArgs e, Canvas canvas, GuiHandler.RectangleAndPoint rectangleAndPoint,
            DataGrid cursorLocationDataGrid)
        {
            var point = UxHandler.GetCursorPosition(canvas);
            var contour = UxHandler.DrawRectangle(e, point, rectangleAndPoint);
            if (contour == null)
            {
                GuiHandler.ShowMousePoint(cursorLocationDataGrid, point);
            }
            else
            {
                GuiHandler.ShowCanvasContour(cursorLocationDataGrid, contour);
            }
            return contour;
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

        public static void RemoveSelections(Canvas thisCanvas, DataGrid contourDataGrid, List<Contour> contourList)
        {
            thisCanvas?.Children?.Clear();
            if (contourDataGrid != null)
            {
                contourDataGrid.ItemsSource = new List<Contour>();
            }
            contourList?.Clear();
        }

        private static void ShowCanvasContour( DataGrid cursorLocationDataGrid, Contour contour)
        {

            if (cursorLocationDataGrid != null && contour != null )
            {
                cursorLocationDataGrid.ItemsSource = new[] { contour };
            }
        }

        private static void ShowMousePoint(DataGrid cursorLocationDataGrid, Point mousePoint)
        {

            if (cursorLocationDataGrid != null)
            {
                cursorLocationDataGrid.ItemsSource = new[] { mousePoint };
            }
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

        public static void SetDataFilePathLabelView(ContentControl dataFilePathLabel, string datafilePath)
        {
            if (dataFilePathLabel != null)
            {
                dataFilePathLabel.Content = datafilePath;
            }
        }

        public static void SetCanvasView(Canvas dataCanvas)
        {
            if (dataCanvas != null)
            {
                dataCanvas.Width = UxHandler.ImageLineSize;
                dataCanvas.Height = UxHandler.LinesToRead;
            }
        }
    }
}
