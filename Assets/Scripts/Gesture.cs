using System;
using System.Collections;

namespace Recognizer.DTW
{
	public class Gesture
	{
        public ArrayList RawPoints;     // raw points (for drawing) -- read in from XML
        public ArrayList Points;        // processed points (rotated, scaled, translated)

		public Gesture()
		{
            this.RawPoints = null;
            this.Points = null;
		}

		public Gesture(ArrayList points)
		{
            this.RawPoints = new ArrayList(points); // copy (saved for drawing)
            this.Points = points;

            // rotate so that the centroid-to-1st-point is at zero degrees
            double radians = Utils.AngleInRadians(Utils.Centroid(Points), (PointR) Points[0], false);
            Points = Utils.RotateByRadians(Points, -radians); // undo angle

            // scale to a common (square) dimension
            Points = Utils.ScaleTo(Points, DTWRecognizer.ResampleScale);

            // finally, translate to a common origin
            Points = Utils.TranslateCentroidTo(Points, DTWRecognizer.ResampleOrigin);
		}

        public int Duration
        {
            get
            {
                if (RawPoints.Count >= 2)
                {
                    PointR p0 = (PointR) RawPoints[0];
                    PointR pn = (PointR) RawPoints[RawPoints.Count - 1];
                    return pn.T - p0.T;
                }
                else
                {
                    return 0;
                }
            }
        }

     

	}
}
