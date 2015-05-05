using SE.Halligang.CsXmpToolkit;
using SE.Halligang.CsXmpToolkit.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace pickture.RegionStorage
{
    public class RegionFileStorageXmp : IRegionFileStorage
    {
        public void SaveRegions(RegionManager region_manager, Canvas picker_canvas, string image_path, PicktureDocument document)
        {
            if (string.IsNullOrEmpty(image_path))
                return;

            try
            {
                // don't save if no regions were placed
                if (picker_canvas.Children.Count == 0)
                {
                    // just clear the XMP regions
                    using (Xmp xmp = Xmp.FromFile(image_path, XmpFileMode.WriteOnly))
                    {
                        ImageMap imageMap = new ImageMap(xmp);
                        imageMap.ImageSize.Clear();
                        imageMap.Areas.Clear();

                        xmp.Save();
                    }

                    return;
                }

                using (Xmp xmp = Xmp.FromFile(image_path, XmpFileMode.ReadWrite))
                {
                    ImageMap imageMap = new ImageMap(xmp);
                    imageMap.ImageSize.Clear();
                    imageMap.Areas.Clear();

                    imageMap.ImageSize.SetDimensions(document.OriginWidth, document.OriginHeight, "pixels");

                    foreach (var region in document.Regions)
                    {
                        imageMap.Areas.Add(new Area(
                            ShapeType.Rectangle,
                            new int[] { (int)region.X, (int)region.Y, (int)region.Width, (int)region.Height },
                            new LangEntry[] { new LangEntry("x-default", "Martin") },
                            null,
                            region.Id.ToString()));
                    }

                    xmp.Save();
                }
            }
            catch (Exception ex)
            {
                // TODO: where to log silent errors to ?!?!
            }
        }

        public void LoadRegions(RegionManager region_manager, string image_path)
        {
            if (string.IsNullOrEmpty(image_path))
                return;

            region_manager.RemoveAllRegions();

            try
            {
                using (Xmp xmp = Xmp.FromFile(image_path, XmpFileMode.ReadOnly))
                {
                    ImageMap imageMap = new ImageMap(xmp);

                    var doc = new PicktureDocument();

                    if (imageMap.ImageSize.Width.HasValue)
                        doc.OriginWidth = (int)Math.Round(imageMap.ImageSize.Width.Value);

                    if (imageMap.ImageSize.Height.HasValue)
                        doc.OriginHeight = (int)Math.Round(imageMap.ImageSize.Height.Value);

                    // TODO: how to save OriginFilename

                    foreach (var area in imageMap.Areas)
                    {
                        doc.Regions.Add(new RegionRectData
                        {
                            X = area.Cords[0],
                            Y = area.Cords[1],
                            Width = area.Cords[2],
                            Height = area.Cords[3],
                            Id = int.Parse(area.Target),
                        });
                    }

                    PicktureDocumentEx.LoadDocument(region_manager, doc);
                }
            }
            catch (Exception ex)
            {
                // TODO: where to log silent errors to ?!?!
            }
        }
    }
}
