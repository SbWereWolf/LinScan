using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LinScan
{
    public partial class WpfGui
    {
        private Handler.RectangleAndPoint RectangleAndPoint { get; set; } = new Handler.RectangleAndPoint();
        private bool ContourMode { get; set; }

        private List<Contour> ContourList { get; } = new List<Contour>();
        private Contour Contour { get; set; } = new Contour();

        public WpfGui()
        {
            InitializeComponent();
        }

        private void SelectDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            var pathLabel = DataFilePathLabel;
            var thisCanvas = DataCanvas;

            var isSuccess = Handler.SetDataFileName(pathLabel);
            if ( isSuccess)
            {
                Handler.LoadData(pathLabel, thisCanvas);
            }
        }

        private void GuiMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainGrid = MainGrid;
            Handler.SetMainGridView(mainGrid);

            var contourModeToggleButton = ContourModeToggleButton;
            Handler.SetContourModeView(contourModeToggleButton,ContourMode);

            var contourList = ContourList;
            var dataCanvas = DataCanvas;
            var contourDataGrid = ContourDataGrid;
            Handler.ClearCanvas(dataCanvas, contourDataGrid, contourList);
            if (ContourDataGrid != null)
            {
                ContourDataGrid.ItemsSource = ContourList;
            }
        }

        private void LoadDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            var pathLabel = DataFilePathLabel;
            var canvas = DataCanvas;            
            Handler.LoadData(pathLabel, canvas);

            var contourList = ContourList;
            var contourDataGrid = ContourDataGrid;
            Handler.ClearCanvas(canvas, contourDataGrid, contourList);
        }

        private void SomeCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = DataCanvas;
            var cursorLocationDataGrid = CursorLocationDataGrid;

            if (ContourMode && e != null )
            {
                var point = Mouse.GetPosition(canvas);
                var contour = Handler.DrawRectangle(e, point, RectangleAndPoint);
                Handler.ShowCanvasContour( cursorLocationDataGrid, contour);

                Contour = contour;
            }
        }

        private void SomeCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = DataCanvas;

            if (ContourMode)
            {
                RectangleAndPoint = Handler.SetRectangle(canvas);
            }
        }

        private void ContourModeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var contourModeToggleButton = ContourModeToggleButton;
            ContourMode = !ContourMode;
            Handler.SetContourModeView(contourModeToggleButton, ContourMode);
        }

        private void ClearContoursButton_Click(object sender, RoutedEventArgs e)
        {
            var contourList = ContourList ;
            var canvas = DataCanvas;
            var contourDataGrid = ContourDataGrid;
            
            Handler.ClearCanvas(canvas, contourDataGrid, contourList);
        }

        private void DataCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var contour = Contour;
            var canvas = DataCanvas;
            var contourList = ContourList;
            var contourDataGrid = ContourDataGrid;
            var rectangle = RectangleAndPoint?.Rect;

            Handler.AddContourToView
                (
                    contour,
                    contourList,
                    contourDataGrid,
                    rectangle,
                    canvas
                );
        }
    }
}
