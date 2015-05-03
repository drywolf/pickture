using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace pickture
{
    public class CreateRegionArgs : EventArgs
    {
        public Rect Rect;
    }

    public class CreateRegionOperation : IAdornerDragOperation
    {
        public void DragFinished(Rect r)
        {
            if (CreateRegion != null)
                CreateRegion(this, new CreateRegionArgs { Rect = r });
        }

        public event EventHandler<CreateRegionArgs> CreateRegion;
    }
}
