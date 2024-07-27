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

    #region " C " : influence general changes
    public static class C
    {
        public static bool debug = true;
    }
    #endregion

    #region Delegates
    public delegate void UpdateDelegate(GameTime gameTime);
    public delegate void DrawDelegate();
    #endregion

    #region " GroundColors " : list of ground colors
    public static class GroundColors
    {
        public static Color branch = new Color(0, 162, 232, 255);
        public static Color block = Color.Black;
        public static Color space = Color.White;
        public static Color visited = Color.Blue;
    }
    #endregion

    #region " S " : general data
    static class S
    {
        #region DATA
        public static ContentManager cm;
        public static SpriteBatch spb;
        public static GraphicsDevice gd;
        public static GraphicsDeviceManager gdm;
        public static KeyboardState kbs, prvkbs;
        public static MouseState ms, prvms;
        public static Texture2D pointTex;
        public static V2 WinSize;
        public static Map map;
        public static Random rnd = new Random();
        public static List<Drawable> sakList = new List<Drawable>();
        public static List<Drawable> agentList = new List<Drawable>();

        #endregion

        #region INIT
        public static void Init(GraphicsDeviceManager gdm, SpriteBatch spb, ContentManager cm, int width, int height)
        {
            S.gdm = gdm;
            S.gd = gdm.GraphicsDevice;
            S.spb = spb;
            S.cm = cm;

            UpdateResolution(width, height);
            map = new Map("map");

            pointTex = new Texture2D(S.gd, 1, 1);
            pointTex.SetData(new Color[] { Color.White });
        }
        #endregion

        #region PRESS AND RELEASE
        public static bool NowClicked(Keys key)
        {
            return S.kbs.IsKeyDown(key) && S.prvkbs.IsKeyUp(key);
        }

        public static bool NowReleased(Keys key)
        {
            return S.kbs.IsKeyUp(key) && S.prvkbs.IsKeyDown(key);
        }

        public static bool NowClickLeftButton()
        {
            return S.ms.LeftButton == ButtonState.Pressed && S.prvms.LeftButton == ButtonState.Released;
        }

        public static bool NowClickRightButton()
        {
            return S.ms.RightButton == ButtonState.Pressed && S.prvms.RightButton == ButtonState.Released;
        }
        #endregion

        #region update resolution
        public static void UpdateResolution(int width, int height)
        {
            S.gdm.PreferredBackBufferHeight = height;
            S.gdm.PreferredBackBufferWidth = width;
            S.gdm.ApplyChanges();
            WinSize = new V2(width, height);
        }
        #endregion

        #region UPDATE
        public static void Update()
        {
            #region update keyboard and mouse
            S.prvkbs = S.kbs;
            S.kbs = Keyboard.GetState();

            S.prvms = S.ms;
            S.ms = Mouse.GetState();
            #endregion
        }
        #endregion

        #region draw rectangles or lines
        public static void DrawRect(V2 p, int size, Color clr)
        {
            S.spb.Draw(pointTex,
                new Rectangle((int)p.X - size / 2,
                (int)p.Y - size / 2, size, size),
                clr);
        }
        public static Vector2 movetotarget(V2 pos, V2 target)
        {
            float rot = (float)Math.Atan2(target.X - pos.X, pos.Y - target.Y);
            Matrix m = Matrix.CreateRotationZ(rot);
            V2 drc = V2.Transform(-V2.UnitY, m);
            return drc;
        }
        public static void DrawLine(V2 a, V2 b, Color clr)
        {
            S.spb.Draw(
                pointTex,
                a, null, clr, (float)Math.Atan2(b.X - a.X, a.Y - b.Y),
                new V2(0.5f, 1),
                new V2(1, (a - b).Length()), SpriteEffects.None,
                1);
        }
        #endregion
    }
    #endregion

    #region Keyboard Classes

    #region BaseKeys
    abstract class BaseKeys
    {

        public abstract bool UpKey();
        public abstract bool DownKey();
        public abstract bool RightKey();
        public abstract bool LeftKey();
    }
    #endregion

    #region UserBaseKeys
    class UserBaseKeys : BaseKeys
    {
        Keys up, down, right, left;

        #region CTOR
        public UserBaseKeys(Keys up, Keys down, Keys right, Keys left)
        {
            this.up = up;
            this.down = down;
            this.right = right;
            this.left = left;
        }
        #endregion

        #region bool funcs for up down ...
        public override bool UpKey()
        {
            return S.kbs.IsKeyDown(up);
        }
        public override bool DownKey()
        {
            return S.kbs.IsKeyDown(down);
        }
        public override bool RightKey()
        {
            return S.kbs.IsKeyDown(right);
        }
        public override bool LeftKey()
        {
            return S.kbs.IsKeyDown(left);
        }
        #endregion
    }
    #endregion


    #endregion

}
