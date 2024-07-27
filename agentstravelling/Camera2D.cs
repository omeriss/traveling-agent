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

namespace agentstravelling
{
    class Camera2D
    {
        #region DATA
        IFocusable Focus;
        public MX Transform { get; private set; }
        public V3 Scale { get; set; }
        Viewport vp;
        #endregion
        #region PUBLIC FUNCTIONS
        public Camera2D(Viewport vp,IFocusable focus)
        {
            this.Focus = focus;
            this.vp = vp;
            this.Scale = V3.One;
        }

        public void update()
        {
            Transform = MX.Identity *
                //MX.CreateTranslation(-Focus.Position.X, -Focus.Position.Y, 0) *
                //MX.CreateRotationZ(-Focus.Rotation * 0.05f) *
                      MX.CreateScale(Scale);
                      //MX.CreateTranslation(vp.Width / 2, vp.Height / 2, 0);
        }
        #endregion
    }
}
