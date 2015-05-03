using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pickture
{
    public enum ImageToggleDirection
    {
        Previous,
        Next
    }

    public class ImageToggleController
    {
        public ImageToggleController(Func<string> get_image_path, Action<string> open_image)
        {
            this.get_image_path = get_image_path;
            this.open_image = open_image;

            PreviousImage = new RelayCommand(OnPreviousImage);
            NextImage = new RelayCommand(OnNextImage);
        }

        private void OnPreviousImage()
        {
            TryToggleImage(ImageToggleDirection.Previous);
        }

        private void OnNextImage()
        {
            TryToggleImage(ImageToggleDirection.Next);
        }

        private void TryToggleImage(ImageToggleDirection direction)
        {
            try
            {
                var image_path = get_image_path();
                var image_directory_path = System.IO.Path.GetDirectoryName(image_path);

                var jpgs = Directory.GetFiles(image_directory_path, "*.jpg").ToList();

                if (jpgs.Count < 2)
                    // NOTE: no other images to load
                    return;

                var index = jpgs.IndexOf(image_path);

                var next_index = index;

                if (direction == ImageToggleDirection.Previous)
                    --next_index;

                else if (direction == ImageToggleDirection.Next)
                    ++next_index;

                if (next_index < 0)
                    next_index = jpgs.Count - 1;

                else if (next_index >= jpgs.Count)
                    next_index = 0;

                var next_image_path = jpgs[next_index];
                open_image(next_image_path);
            }
            catch (Exception ex)
            {
                // TODO: where to log silent errors to ?!?!
            }
        }

        public ICommand PreviousImage { get; set; }
        public ICommand NextImage { get; set; }

        private Func<string> get_image_path;
        private Action<string> open_image;
    }
}
