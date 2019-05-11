using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UniExtensions.Graph
{
    /// <summary>
    /// The A* path finding algorithm. Uses objects that implement IAStarNode.
    /// </summary>
    public class AStar
    {
        IAStarNode[] nodes;
        Dictionary<IAStarNode, float> g;
        Dictionary<IAStarNode, IAStarNode> parent;
        Dictionary<IAStarNode, bool> inPath;
        List<IAStarNode> openset, closedset, path;

        public AStar (IEnumerable<IAStarNode> nodes)
        {
            this.nodes = nodes.ToArray ();
            g = new Dictionary<IAStarNode, float> (this.nodes.Length);
            parent = new Dictionary<IAStarNode, IAStarNode> (this.nodes.Length);
            inPath = new Dictionary<IAStarNode, bool> (this.nodes.Length);
            openset = new List<IAStarNode>(this.nodes.Length);
            closedset = new List<IAStarNode>(this.nodes.Length);
            path = new List<IAStarNode>();
        }

        /// <summary>
        /// Calculate the route from start to end.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public IAStarNode[] Route (IAStarNode start, IAStarNode end)
        {
            if (start == null || end == null) {
                return null;
            }

            foreach (var s in nodes) {
                g [s] = 0f;
                parent [s] = null;
                inPath [s] = false;
            }
            openset.Clear();
            closedset.Clear();
            path.Clear();

            var current = start;
            openset.Add (current);
            while (openset.Count > 0) {
                openset.Sort ((a,b) => g [a].CompareTo (g [b]));
                current = openset [0];
                if (current == end) {

                    while (parent[current] != null) {
                        path.Add (current);
                        inPath [current] = true;
                        current = parent [current];
                        if (path.Count >= nodes.Length) {
                            return null;
                        }
                    }
                    inPath [current] = true;
                    path.Add (current);
                    path.Reverse ();
                    return path.ToArray ();
                }
                openset.Remove (current);
                closedset.Add (current);
                foreach (var node in current.GetConnectedNodes()) {
                    if (closedset.Contains (node))
                        continue;
                    if (openset.Contains (node)) {
                        var new_g = g [current] + current.CalculateMoveCost (node);
                        if (g [node] > new_g) {
                            g [node] = new_g;
                            parent [node] = current;
                        }
                    } else {
                        g [node] = g [current] + current.CalculateMoveCost (node);
                        parent [node] = current;
                        openset.Add (node);
                    }
                }
            }
            return null;
        }
    }
}
