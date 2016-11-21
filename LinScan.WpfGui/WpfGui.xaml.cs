using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LinScan
{
    public partial class WpfGui
    {
        private GuiHandler.RectangleAndPoint RectangleAndPoint { get; set; } = new GuiHandler.RectangleAndPoint();
        private bool ContourMode { get; set; }
        private string DatafilePath { get; set; } = " файл не выбран";

        private List<Contour> ContourList { get; } = new List<Contour>();
        private Contour Contour { get; set; } = new Contour(GuiHandler.DrawCrossLimit);

        public WpfGui()
        {
            InitializeComponent();
        }

        private void SelectDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            var pathLabel = DataFilePathLabel;
            var thisCanvas = DataCanvas;

            DatafilePath = GuiHandler.LoadDataFile(pathLabel, thisCanvas);
        }



        private void GuiMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainGrid = MainGrid;
            GuiHandler.SetMainGridView(mainGrid);

            var contourModeToggleButton = ContourModeToggleButton;
            GuiHandler.SetContourModeView(contourModeToggleButton,ContourMode);
            
            var dataCanvas = DataCanvas;
            GuiHandler.SetCanvasView(dataCanvas);

            var dataFilePathLabel = this.DataFilePathLabel;
            GuiHandler.SetDataFilePathLabelView(dataFilePathLabel, DatafilePath);
        }

        private void LoadDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            var canvas = DataCanvas;
            var path = DatafilePath;
            GuiHandler.LoadData(canvas, path);

            var contourList = ContourList;
            var contourDataGrid = ContourDataGrid;
            GuiHandler.RemoveSelections(canvas, contourDataGrid, contourList);
        }



        private void DataCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = DataCanvas;
            var cursorLocationDataGrid = CursorLocationDataGrid;
            var rectangleAndPoint = RectangleAndPoint;

            if (e != null && ContourMode)
            {
                var contour = GuiHandler.DrawRectangle(e, canvas, rectangleAndPoint, cursorLocationDataGrid);

                if (contour != null)
                {
                    Contour = contour;
                }
            }
        }

        private void DataCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = DataCanvas;

            if (ContourMode)
            {
                RectangleAndPoint = GuiHandler.SetRectangle(canvas);
            }
        }

        private void ContourModeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var contourModeToggleButton = ContourModeToggleButton;
            ContourMode = !ContourMode;
            GuiHandler.SetContourModeView(contourModeToggleButton, ContourMode);
        }

        private void ClearContoursButton_Click(object sender, RoutedEventArgs e)
        {
            var contourList = ContourList ;
            var canvas = DataCanvas;
            var contourDataGrid = ContourDataGrid;
            
            GuiHandler.RemoveSelections(canvas, contourDataGrid, contourList);
        }

        private void DataCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var contour = Contour;
            var canvas = DataCanvas;
            var contourList = ContourList;
            var contourDataGrid = ContourDataGrid;
            var rectangle = RectangleAndPoint?.Rect;

            if (ContourMode)
            {
                GuiHandler.AddContourToView(contour, contourList, contourDataGrid, rectangle, canvas);               
            }

        }
    }
}
