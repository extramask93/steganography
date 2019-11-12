using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using steg_2.lib;

namespace steg_2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap source;
        Bitmap destination;
        IBmpHider hider;
        public MainWindow()
        {
            InitializeComponent();
            hider = new BmpHider3();
        }
        private void load_image_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {

                source = new Bitmap(openFileDialog.FileName);
                source_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                           source.GetHbitmap(),
                                           IntPtr.Zero,
                                           System.Windows.Int32Rect.Empty,
                                           BitmapSizeOptions.FromWidthAndHeight(source.Width, source.Height));
            }
        }

        private void save_image_button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                destination.Save(dialog.FileName, ImageFormat.Bmp);
            }
        }

        private void Do_button_Click(object sender, RoutedEventArgs e)
        {
            hider.setKey(result.Text);
            var enc_message = Rijndael.Encrypt(Encoding.ASCII.GetBytes(info.Text),
                Encoding.ASCII.GetBytes(result.Text));
            destination = hider.embed_message(source, enc_message);
            destination_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                           destination.GetHbitmap(),
                                           IntPtr.Zero,
                                           System.Windows.Int32Rect.Empty,
                                           BitmapSizeOptions.FromWidthAndHeight(destination.Width, destination.Height));
        
    }

        private void decode_button_Click(object sender, RoutedEventArgs e)
        {
            hider.setKey(result.Text);
            var enc_message = hider.get_message(source);          
            info.Text = Encoding.ASCII.GetString(
                Rijndael.Decrypt(enc_message,Encoding.ASCII.GetBytes(result.Text))
                );

        }
    }
}
