#region MyRegion
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
    #region Focusable Interface
    interface IFocusable
    {
        V2 Position { get; }
        float Rotation { get; }
    }
    #endregion

    #region Drawable Object
    class Drawable : IFocusable
    {
        #region Data
        public V2 Position { get; set; }
        public float Rotation { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle? TexPart { get; set; }
        public V2 Scale { get; set; }
        public float Layer { get; set; }
        public Color Color { get; set; }
        public V2 Origin { get; set; }
        public SpriteEffects Flip { get; set; }

        public Drawable(V2 position, float rotation, 
            Texture2D texture, Rectangle? texPart, 
            V2 scale, float layer, Color color, V2 origin, SpriteEffects flip,
            bool registerDrawEvent=true)
        {
            Position = position;
            Rotation = rotation;
            Texture = texture;
            TexPart = texPart;
            Scale = scale;
            Layer = layer;
            Color = color;
            Origin = origin;
            Flip = flip;

            if (registerDrawEvent)
                Game1.Event_Draw += this.Draw;
        }

        #endregion
        public void unregister_event()
        {
            Game1.Event_Draw -= this.Draw;
        }
        #region CTOR
        public Drawable(V2 position, V2 origin, string tex, bool registerDrawEvent)
            : this(position, origin, S.cm.Load<Texture2D>(tex), registerDrawEvent)
        { }
        public Drawable(V2 position, V2 origin, Texture2D tex, bool registerDrawEvent)
        {
            this.Position = position;
            this.Texture = tex;
            this.Origin = origin;

            Rotation = 0;
            TexPart = null;
            Scale = V2.One;
            Layer = 0;
            Color = Color.White;
            Flip = SpriteEffects.None;

            if (registerDrawEvent)
                Game1.Event_Draw += this.Draw;
        }
        #endregion

        #region Draw
        public void Draw()
        {
            S.spb.Draw(
                this.Texture,
                this.Position,
                this.TexPart,
                this.Color,
                this.Rotation,
                this.Origin,
                this.Scale,
                this.Flip,
                this.Layer);
        }
        #endregion
    }
    #endregion
}
