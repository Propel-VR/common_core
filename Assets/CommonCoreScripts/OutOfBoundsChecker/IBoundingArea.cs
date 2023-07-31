using UnityEngine;

namespace CommonCoreScripts.OutOfBoundsChecker
{

    /// <summary>
    /// An area used to check if some position lies within a boundary.
    /// 
    /// <see cref="OutOfBoundsChecker"/>
    /// </summary>
    interface IBoundingArea
    {
        /// <summary>
        /// Checks to see if the given position is within this bounding area.
        /// </summary>
        public bool IsInsideArea(Vector3 position);

        /// <summary>
        /// Find the point inside this bounding area closest to the given position.
        /// 
        /// If <c>offset</c> is given and greater than 0, will return a point such that 
        /// a sphere of radius <c>offset</c> at this point will entirely fit within the
        /// bounds.
        /// </summary>
        public Vector3 ClosestPoint(Vector3 position, float offset = 0f);
    }

}
