﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace pickture
{
    public static class RegionFileStorage
    {
        public static void SaveRegions(RegionManager region_manager, Canvas picker_canvas, string image_path, string image_filename, int img_w_px, int img_h_px)
        {
            if (string.IsNullOrEmpty(image_path))
                return;

            var xs = new XmlSerializer(typeof(PicktureDocument));
            var pck_path = System.IO.Path.ChangeExtension(image_path, ".pck");

            try
            {
                // don't save if no regions were placed
                if (picker_canvas.Children.Count == 0)
                {
                    // just delete the pck file if there is one
                    if (File.Exists(pck_path))
                        File.Delete(pck_path);

                    return;
                }

                var doc = PicktureDocumentEx.BuildDocument(region_manager, image_filename, img_w_px, img_h_px);

                using (var fs = new FileStream(pck_path, FileMode.Create))
                using (var gz = new GZipStream(fs, CompressionLevel.Optimal))
                {
                    xs.Serialize(gz, doc);
                }
            }
            catch (Exception ex)
            {
                // TODO: where to log silent errors to ?!?!
            }
        }

        public static void LoadRegions(RegionManager region_manager, string image_path)
        {
            if (string.IsNullOrEmpty(image_path))
                return;

            region_manager.RemoveAllRegions();

            var xs = new XmlSerializer(typeof(PicktureDocument));
            var pck_path = System.IO.Path.ChangeExtension(image_path, ".pck");

            if (!File.Exists(pck_path))
                return;

            try
            {
                using (var fs = new FileStream(pck_path, FileMode.Open))
                using (var gz = new GZipStream(fs, CompressionMode.Decompress))
                {
                    var doc = xs.Deserialize(gz) as PicktureDocument;
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
