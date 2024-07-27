#region using
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using V2 = Microsoft.Xna.Framework.Vector2;
using MH = Microsoft.Xna.Framework.MathHelper;
using V3 = Microsoft.Xna.Framework.Vector3;
using MX = Microsoft.Xna.Framework.Matrix; 
#endregion

namespace agentstravelling
{
    class Map : Drawable
    {
        #region GroundType  public enum GroundType
        public enum GroundType
        {
            branch,
            block,
            space
        }
        #endregion

        private GroundType[,] grounds;
        public  List<Vector2 > spaceList= new List<Vector2>();
        public  List<Vector2> branchList = new List<Vector2>();
        public GroundType this[float x, float y]
        {
            get
            {
                x /= Scale.X;
                y /= Scale.Y;
                if (x < 0 || x >= grounds.GetLength(0) ||
                    y < 0 || y >= grounds.GetLength(1))
                    return GroundType.block;
                return grounds[(int)(x), (int)(y)];
            }
        }

        public int Width
        {
            get { return grounds.GetLength(0); }
        }

        public int Height
        {
            get { return grounds.GetLength(1); }
        }

        public V2 calc_ray_to_wall(V2 drc, V2 position)
        {
            Vector2 newPos = position;
            while (this[newPos.X, newPos.Y ] != GroundType.block)
            {
                newPos += drc;
            }
            return newPos;
        }
        #region MyRegion
        //public float? find_road_x(float y)
        //{
        //    int yi = (int)(y / Scale.Y);
        //    for (int x = 0; x < grounds.GetLength(1); x++)
        //    {
        //        if (grounds[x, yi] == GroundType.Road)
        //        {
        //            return x * Scale.X;
        //        }
        //    }
        //    return null;
        //} 
        #endregion
        public bool isWallClose(V2 position, V2 lookTo,out V2 meetPoint)
        {
            meetPoint = position + lookTo;
            float maxlength = lookTo.Length();
            lookTo.Normalize();
            V2 rayPos = position;
            while (this[rayPos.X, rayPos.Y] != GroundType.block)
            {
                if ((rayPos - position).Length() > maxlength)
                    return false;
                rayPos += lookTo;
            }
            meetPoint = rayPos;
            return true;
        }
        #region CTORS
        public Map(string path)
            : this(S.cm.Load<Texture2D>(path))
        {

        }
        public Map(Texture2D tex)
            : base(V2.Zero, V2.Zero, tex, true)
        {
            grounds = new GroundType[tex.Width, tex.Height];

            Color[] texColor = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(texColor);

            #region FILL GROUND ARRAY BY TEX
            for (int x = 0; x < tex.Width; x++)
            {
                for (int y = 0; y < tex.Height; y++)
                {
                    grounds[x, y] = GroundType.block ; //default is wall

                    if (texColor[x + y * tex.Width] == GroundColors.space)
                    {
                        grounds[x, y] = GroundType.space;
                        spaceList.Add(new Vector2(x, y));
                        
                    }

                    if (texColor[x + y * tex.Width] == GroundColors.branch)
                    {
                        grounds[x, y] = GroundType.branch;
                        branchList.Add(new Vector2(x, y));
                    }
   
                }
            }
            #endregion

        }
        #endregion
    }
}
