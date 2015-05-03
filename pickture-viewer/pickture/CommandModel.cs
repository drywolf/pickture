using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pickture
{
    public class CommandModel
    {
        public CommandModel()
        {
            create_region = new CreateRegionOperation();
        }

        public ImageToggleController ToggleImage { get; set; }

        public IAdornerDragOperation CurrentDragOperation
        {
            get { return create_region; }
        }

        public CreateRegionOperation create_region; 
    }
}
