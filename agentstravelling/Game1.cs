using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;

namespace agentstravelling
{
    public delegate void DlgDraw();
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static DlgDraw Event_Draw;
        int[] sakindex = new int[4];
        List<Vector2> totalPath = new List<Vector2>();
        int totalPathIndex = 0;
        const int SPEED = 1;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            S.Init(_graphics, _spriteBatch, Content, 1400, 800);
            
            // Generate  the list of points to reach
            for (int i = 0; i < 40; i++)
            {

                int index = S.rnd.Next(S.map.spaceList.Count);
                Vector2 pos = S.map.spaceList[index];
                S.sakList.Add(new Drawable(pos,
                    0, Content.Load<Texture2D>("astro"),
                    null, new Vector2(0.1f), 0, Color.White,
                    new Vector2(100), SpriteEffects.None));
            }

            //  Generate the list of starting positions for the alian - 1 alian right now
            for (int i = 0; i < 1; i++)
            {
                int index = S.rnd.Next(S.map.branchList.Count);
                Vector2 pos = S.map.branchList[index];
                S.agentList.Add(new Drawable(pos,
                    0, Content.Load<Texture2D>("weirdi"),
                    null, new Vector2(0.3f), 0, Color.White,
                    new Vector2(100), SpriteEffects.None));
                sakindex[i] = S.rnd.Next(S.sakList.Count);
            }

            // init an array of all the points to reach + the starting position
            var positionList = new List<Vector2>();
            positionList.Add(S.agentList[0].Position);
            positionList.AddRange(S.sakList.Select(sak => sak.Position));


            // caculate the dist between each 2 points
            var distMatrix = new int[positionList.Count, positionList.Count];
            var pathMatrix = new List<Vector2>[positionList.Count, positionList.Count];

            for (int i = 0; i < positionList.Count; i++)
            {
                var bfsRes = ShortPath.Bfs(positionList.GetRange(i, positionList.Count - i), S.map, 0);

                for (int j = i; j < positionList.Count; j++)
                {
                    pathMatrix[i, j] = bfsRes[j - i];
                    pathMatrix[j, i] = bfsRes[j - i].AsEnumerable().Reverse().ToList();

                    distMatrix[j, i] = bfsRes[j - i].Count - 1;
                    distMatrix[i, j] = bfsRes[j - i].Count - 1;
                }
            }

            // find the min spanning tree from the distances we found
            var mst = ShortPath.Mst(distMatrix);

            // make a scan of the tree, this is the path we should take with the alian
            var path = ShortPath.GetRoute(mst, distMatrix);

            for(int i = 1; i < path.Count; i++)
            {
                totalPath.AddRange(pathMatrix[path[i - 1], path[i]]);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (totalPathIndex + SPEED < totalPath.Count)
            {
                totalPathIndex+= SPEED;
                S.agentList[0].Position = totalPath[totalPathIndex];
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();
            Event_Draw?.Invoke();
            for (int i = 0; i < totalPath.Count - SPEED; i += SPEED)
            {
                S.DrawLine(totalPath[i], totalPath[i + SPEED], Color.Blue);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}