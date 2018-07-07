using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using raylib;

namespace raywpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private readonly RenderData _renderData = new RenderData(500, 500, 5, 4, true);
    private void Render(IPixelArray pixelArray, Scene scene, Camera camera, string name)
    {
      var renderer = new Renderer(_renderData, true);
      using (new LogTimer($"Render {name}"))
      {
        renderer.Progress += (sender, args) =>
        {
          Dispatcher.Invoke(() => { RenderProgress.Value = args.PercentComplete; });
        };
        Task.Run(() =>
        {
          Dispatcher.Invoke(() => RenderButton.IsEnabled = false);
          renderer.Render(pixelArray, camera, scene, true);
          Dispatcher.Invoke(() => RenderButton.IsEnabled = true);
        });
        //string filename = UseExTracer ? $"{name}_scene_ex.png" : $"{name}_scene.png";
        //pixelArray.SaveAsFile(Path.Combine(OutputDirectory, filename));
      }
    }

    private void RenderButton_OnClick(object sender, RoutedEventArgs e)
    {
      var scene = SceneFactory.CreateBasicScene();
      var camera = new Camera(
        new PosVector(7.5, 7.5, 2.3),
        new PosVector(0.0, 0.0, 0.0),
        new PosVector(0.0, 0.0, 1.0),
        50.0);
      var pixelArray = new WpfPixelArray(Dispatcher, _renderData.Width, _renderData.Height);
      RenderImage.Source = pixelArray.WriteableBitmap;
      Render(pixelArray, scene, camera, "basic");
    }
  }
}
