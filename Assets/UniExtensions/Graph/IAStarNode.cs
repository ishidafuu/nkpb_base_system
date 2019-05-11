using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Interface required for objects to be used with the AStar path finding class.
/// </summary>
public interface IAStarNode {
    /// <summary>
    /// Gets all nodes that are connected to this node.
    /// </summary>
    /// <returns>The connected nodes.</returns>
    IEnumerable<IAStarNode> GetConnectedNodes ();

    /// <summary>
    /// Calculates the estimated move cost to another node.
    /// </summary>
    /// <returns>The move cost.</returns>
    /// <param name="node">Node.</param>
    float CalculateMoveCost (IAStarNode node);
}