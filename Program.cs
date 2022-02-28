using System;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;

using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace thumbnailxps
{
    class Program
    {
        // First argument: Path of XPS file

        // Returns
        //      -1: Invalid number of arguments
        //      -2: XPS File not found
        //      -3: XPS File has an invalid format
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length != 1)
                return -1;

            string path = args[0];

            // Get information of XPS file
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
                return -2;

            // Load and read XPS document
            XpsDocument document;
            FixedDocumentSequence documentFixedSequence;
            try
            {
                document = new XpsDocument(path, FileAccess.Read);
                documentFixedSequence = document.GetFixedDocumentSequence();
            }
            catch (FileFormatException)
            {
                return -3;
            }

            // Get first page of the document
            PngBitmapEncoder pngFile = new PngBitmapEncoder();
            {
                DocumentPaginator documentPaginator = documentFixedSequence.DocumentPaginator;
                DocumentPage documentPage = documentPaginator.GetPage(0);
                FrameworkElement frameElement = (FrameworkElement)documentPage.Visual;
                
                int x = 300;
                int y = (int)(x * frameElement.ActualHeight / frameElement.ActualWidth);
                double dpi = 96 * x / frameElement.ActualWidth;
                
                RenderTargetBitmap bmpFrame = new RenderTargetBitmap(x, y, dpi, dpi, PixelFormats.Default);
                bmpFrame.Render(frameElement);

                pngFile.Frames.Add(BitmapFrame.Create(bmpFrame));
            }

            // Save thumbnail to png file
            Stream stream = File.Create(path.Substring(0, path.Length - 4) + ".png");
            pngFile.Save(stream);

            return 0;
        }
    }
}
