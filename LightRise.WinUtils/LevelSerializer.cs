using LightRise.BaseClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightRise.WinUtilsLib
{
    [Serializable]
    public class LevelSerializer
    {
        MapSerializer Map { get; set; }
        Dictionary<string, CharacterSerializer> Characters { get; set; }
        Dictionary<string, InteractivesSerializer> Interactives { get; set; }
        Dictionary<string, ActiveZonesSerizlizer> ActiveZones { get; set; }

        public static void SaveToFile(string fileName, Level level)
        {
            LevelSerializer levelSerializer = new LevelSerializer();
            levelSerializer.Map = new MapSerializer();
            levelSerializer.Characters = new Dictionary<string, CharacterSerializer>();
            levelSerializer.Interactives = new Dictionary<string, InteractivesSerializer>();
            levelSerializer.ActiveZones = new Dictionary<string, ActiveZonesSerizlizer>();

            Stream fs = new FileStream(fileName, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, levelSerializer);
            fs.Close();
        }

        /*public Level LoadFromFile(string fileName)
        {
            try
            {
                Stream fs = new FileStream(fileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Deserialize(fs);
                fs.Close();
                Level level = new Level();
                level.Map = Map.LoadFromSerializer()
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }*/
    }

    [Serializable]
    public class MapSerializer
    {
        public Point size { get; set; }
        public uint[,] array { get; set; }
    }

    [Serializable]
    public class CharacterSerializer
    {
        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public Vector2 speed { get; set; }
        public Vector2 axeleration { get; set; }
        public Vector2 maxSpeed { get; set; }
        public float Gravity { get; set; }
        public Dictionary<string, object> other { get; set; }
    }

    [Serializable]
    public class InteractivesSerializer
    {
        public delegate void Act(Character interactor);
        public Act Use { get; set; }
        public Vector2 Position { get; set; }
        public float InteractionRadius { get; set; }
    }

    [Serializable]
    public class ActiveZonesSerizlizer
    {
        public delegate void Act(Character interactor);
        public Act Check { get; set; }
        public Rectangle InteractionArea { get; set; }
    }
}