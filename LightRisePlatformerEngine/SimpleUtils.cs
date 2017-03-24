﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LightRisePlatformerEngine {
    static class SimpleUtils {

        private static Random _rand = new Random( );
        public static Texture2D WhiteRect { get; private set; }

        static SimpleUtils( ) {
            WhiteRect = new Texture2D(Program.MainThread.GraphicsDevice, 1, 1);
            WhiteRect.SetData(new Color[ ] { Color.White });
        }

        public static T[ ][ ] Create2DArray<T>(UInt32 width, UInt32 height, T @default) {
            T[ ][ ] result = new T[width][ ];
            for (UInt32 i = 0; i < width; i++) {
                result[i] = new T[height];
                for (UInt32 j = 0; j < height; j++) {
                    result[i][j] = @default;
                }
            }
            return result;
        }

        public static T Choose<T>(T[ ] variants) {
            return variants[_rand.Next( ) % variants.Length];
        }

    }
}