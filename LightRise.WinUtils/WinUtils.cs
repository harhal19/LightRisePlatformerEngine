using LightRise.BaseClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightRise.WinUtilsLib {

    public static class WinUtils {


        public static void Save(this Map map, Point player_position) {
            SaveFileDialog sfd = new SaveFileDialog( );
            sfd.Filter = "map files (*.lrmap)|*.lrmap";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog( ) == DialogResult.OK) {
                using (FileStream sw = new FileStream(sfd.FileName, FileMode.Create))
                using (BinaryWriter bw = new BinaryWriter(sw)) {
                    bw.Write(player_position.X);
                    bw.Write(player_position.Y);
                    map.Save(bw);
                }
            }
        }

        public static void Save(this Map map, BinaryWriter sw) {
            uint left = map.Left;
            uint right = map.Right;
            uint top = map.Top;
            uint bottom = map.Bottom;
            sw.Write(right - left + 1);
            sw.Write(bottom - top + 1);
            for (uint i = left; i <= right; i++) {
                for (uint j = top; j <= bottom; j++) {
                    sw.Write(map[i, j]);
                }
            }
        }

        public static Tuple<Map, Point> LoadMap( ) {
            OpenFileDialog ofd = new OpenFileDialog( );
            ofd.Filter = "map files (*.lrmap)|*.lrmap";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog( ) == DialogResult.OK) {
                return LoadMap(ofd.FileName);
            }
            throw new Exception("File is not opened");
        }

        public static Tuple<Map, Point> LoadMap(string path) {
            Point playerPosition = new Point( );
            Map result = null;
            using (FileStream sw = new FileStream(path, FileMode.Open))
            using (BinaryReader br = new BinaryReader(sw)) {
                playerPosition.X = (int)br.ReadUInt32( );
                playerPosition.Y = (int)br.ReadUInt32( );
                result = new Map(br.ReadUInt32( ), br.ReadUInt32( ));
                for (uint i = 0; i < result.Width; i++) {
                    for (uint j = 0; j < result.Height; j++) {
                        result[i, j] = br.ReadUInt32( );
                    }
                }
            }
            return new Tuple<Map, Point>(result, playerPosition);
        }
    }
}
