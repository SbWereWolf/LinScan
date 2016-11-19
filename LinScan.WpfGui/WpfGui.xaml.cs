using System.Windows;
using System.Windows.Input;

namespace LinScan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WpfGui
    {
        private Handler.RectangleAndPoint RectangleAndPoint { get; set; }

        public WpfGui()
        {
            InitializeComponent();
        }

        private void SelectDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            var pathLabel = DataFilePathLabel;
            var thisCanvas = SomeCanvas;

            var result = Handler.SetDataFileName(pathLabel);
            if ( result)
            {
                Handler.LoadData(pathLabel, thisCanvas);
            }
        }

        private void GuiMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainGrid = MainGrid;
            Handler.SetUpMainGrid(mainGrid);
        }

        private void LoadDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            var pathLabel = DataFilePathLabel;
            var canvas = SomeCanvas;

            Handler.LoadData(pathLabel, canvas);
        }

        private void SomeCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = SomeCanvas;
            var cursorLocationDataGrid = CursorLocationDataGrid;

            var point = Handler.ShowPoint(canvas, cursorLocationDataGrid);

            Handler.DrawRectangle(e, point, RectangleAndPoint);
        }

        private void SomeCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = SomeCanvas;

            RectangleAndPoint= Handler.SetRectangle(canvas);
        }
    }
}
