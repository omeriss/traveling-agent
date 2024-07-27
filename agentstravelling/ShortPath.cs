using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agentstravelling;

class ShortPath
{
    public static List<Vector2> GetPath(Vector2[,] parent, Vector2 from, Vector2 to)
    {
        var path = new List<Vector2>();

        while (to != from)
        {
            path.Add(to);
            to = parent[(int)to.X, (int)to.Y];
        }

        path.Add(from);
        path.Reverse();

        return path;
    }

    public static List<List<Vector2>> Bfs(List<Vector2> points, Map map, int start)
    {
        // matrix of the positions we visited with the bfs
        var visited = new bool[map.Width, map.Height];
        visited[(int)points[start].X, (int)points[start].Y] = true;

        // array for each point where we got to it
        var parent = new Vector2[map.Width, map.Height];

        // dict of the points we need to reach
        var dists = points.ToDictionary(point => point, point => 0);

        // diractions arrays for the dirs the alian can move in
        int[] dx = { 0, 0, -1, 1, 1, 1, -1, -1 };
        int[] dy = { -1, 1, 0, 0, -1, 1, -1, 1 };

        //List<int> dists = new(points.Count);

        // int - the dist from the start point
        // vec2 - the current pos
        var queue = new Queue<Tuple<int, Vector2>>();

        // push the starting position to the queue
        queue.Enqueue(new(0, points[start]));

        while (queue.Count > 0)
        {
            // get the current pos from the queue
            var curr = queue.Dequeue();
            Vector2 pos = curr.Item2;
            int dist = curr.Item1;

            // save the dist to the point we need to reach if it is one of the points we are looking for
            if (dists.ContainsKey(pos))
            {
                dists[pos] = dist;
            }

            // push all the points the alian can move to from the current point to the queue
            for ( int i = 0; i < dx.Length; i++)
            {
                int newX = (int)(dx[i] + pos.X);
                int newY = (int)(dy[i] + pos.Y);

                if (map[newX, newY] != Map.GroundType.block && !visited[newX, newY])
                {
                    visited[newX, newY] = true;
                    parent[newX, newY] = pos;
                    queue.Enqueue(new(dist + 1, new(newX, newY)));
                }
            } 
        }

        return points.Select(point => GetPath(parent, points[start], point)).ToList();
    }

    /// <summary>
    /// find the point that is closest to the tree we built so far
    /// </summary>
    private static int GetMinNode(int[] distFromTree, bool[] visited)
    {
        int minIndex = 0;
        int minValue = int.MaxValue;

        for (int i = 0; i < distFromTree.Length; i++)
        {
            if (!visited[i] && distFromTree[i] < minValue)
            {
                minIndex = i;
                minValue = distFromTree[i];
            }
        }

        return minIndex;
    }

    /// <summary>
    /// find the min spanning tree from the full graph discribed by dists
    /// </summary>
    /// <param name="dists">the dist between each 2 points the the graph</param>
    /// <returns>array representing the tree</returns>
    public static int[] Mst(int[,] dists)
    {
        // parent - for each node the the mst, the parent node in the tree
        var parent = new int[dists.GetLength(0)];
        // the array of the nodes we visited
        var visited = new bool[dists.GetLength(0)];
        // distance of each node from the current tree we built
        var distFromTree = new int[dists.GetLength(0)];

        // init the dist from tree
        for( int i =0;i < parent.Length; i++)
        {
            distFromTree[i] = int.MaxValue;
        }


        parent[0] = -1;
        // set the dist from tree of the first point
        distFromTree[0] = 0;

        for (int i = 0; i < parent.Length - 1; i++)
        {
            // find the next point to add to the tree
            int cur = GetMinNode(distFromTree, visited);

            visited[cur] = true;

            // update the dist from tree to include the new point in the tree
            for (int j = 0; j < parent.Length; j++)
            {
                if (!visited[j] && dists[cur, j] < distFromTree[j])
                {
                    distFromTree[j] = dists[cur, j];
                    parent[j] = cur;
                }
            }
        }

        return parent;
    }

    /// <summary>
    /// recursive function to scan the tree with inorder
    /// </summary>
    private static void InOrder(List<int>[] children, int node, List<int> route)
    {
        route.Add(node);

        foreach(var child in children[node])
        {
            InOrder(children, child, route);
        }
    }

    public static List<int> GetRoute(int[] mst, int[,] distMatrix)
    {
        // convert the tree from list of parants to list of childs on each node
        var route = new List<int>();
        List<int>[] children = new List<int>[mst.Length];

        for (int i = 0; i < mst.Length; i++)
        {
            children[i] = new();
        }

        for (int i = 0; i < mst.Length; i++)
        {
            if (mst[i] == -1) continue;
            children[mst[i]].Add(i);
        }

        for(int i =0; i < children.Length; i++)
        {
            children[i].Sort((a, b) => distMatrix[i, a].CompareTo(distMatrix[i, b]));
        }

        // scan the tree in order
        InOrder(children, 0, route);

        return route;
    }
}
