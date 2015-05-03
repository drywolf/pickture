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
        }

        public ImageToggleController ToggleImage { get; set; }
    }
}
