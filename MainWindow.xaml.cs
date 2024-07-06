using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls; // Add this line
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace PhotoViewerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadMedia_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image and Video files (*.jpg;*.jpeg;*.png;*.gif;*.webp;*.mp4;*.wav)|*.jpg;*.jpeg;*.png;*.gif;*.webp;*.mp4;*.wav";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileExtension = Path.GetExtension(openFileDialog.FileName).ToLower();

                PhotoImage.Visibility = Visibility.Collapsed;
                MediaElement.Visibility = Visibility.Collapsed;

                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    PhotoImage.Source = bitmap;
                    PhotoImage.Visibility = Visibility.Visible;
                }
                else if (fileExtension == ".webp")
                {
                    using (var input = File.OpenRead(openFileDialog.FileName))
                    {
                        var codec = SKCodec.Create(input);
                        var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                        using (var bitmap = new SKBitmap(info))
                        {
                            codec.GetPixels(bitmap.Info, bitmap.GetPixels());
                            using (var image = SKImage.FromBitmap(bitmap))
                            using (var data = image.Encode())
                            using (var ms = new MemoryStream(data.ToArray()))
                            {
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.StreamSource = ms;
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.EndInit();
                                PhotoImage.Source = bitmapImage;
                                PhotoImage.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
                else if (fileExtension == ".mp4" || fileExtension == ".wav")
                {
                    MediaElement.Source = new Uri(openFileDialog.FileName);
                    MediaElement.LoadedBehavior = MediaState.Manual;
                    MediaElement.UnloadedBehavior = MediaState.Stop;
                    MediaElement.Play();
                    MediaElement.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
