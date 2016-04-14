using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Recognizer.DTW
{
	public class DTWRecognizer
	{
		#region Types, Constants, Fields

        private const double DX = 250.0;
        public static readonly SizeR ResampleScale = new SizeR(DX, DX);
        public static readonly double Diagonal = Math.Sqrt(DX * DX + DX * DX);
        public static readonly double HalfDiagonal = 0.5 * Diagonal;
        public static readonly PointR ResampleOrigin = new PointR(0, 0);
        private static readonly double Phi = 0.5 * (-1 + Math.Sqrt(5)); // Golden Ratio
        public GameObject DebugHandle;
        public TextMesh DebugText;

        // batch testing
        private const int NumRandomTests = 100;

        private Gesture _training_data;
        private int _sn = 0;
        private int _sm = 0;
        private double[,] _dtw;

        public const int DATA_X_VALUES = 0;
        public const int DATA_Y_VALUES = 1;
        public const int DATA_Z_VALUES = 2;

		#endregion

		#region Constructor
	
		public DTWRecognizer(string data, int value_set)
		{
            DebugHandle = GameObject.Find("MRec");
            DebugText = DebugHandle.GetComponent<TextMesh>();
            LoadTrainingData(data, value_set);
		}

		#endregion

		#region Recognition

        public double Recognize(ArrayList points) // candidate points
        {
            // rotate so that the centroid-to-1st-point is at zero degrees
            double radians = Utils.AngleInRadians(Utils.Centroid(points), (PointR) points[0], false); // indicative angle
            points = Utils.RotateByRadians(points, -radians); // undo angle

            // scale to a common (square) dimension
            points = Utils.ScaleTo(points, ResampleScale);

            // translate to a common origin
            points = Utils.TranslateCentroidTo(points, ResampleOrigin);
            
                double[] best = GoldenSectionSearch(
                    points,                 // to rotate
                    _training_data.Points,               // to match
                    Utils.Deg2Rad(-45.0),   // lbound
                    Utils.Deg2Rad(+45.0),   // ubound
                    Utils.Deg2Rad(2.0));    // threshold

                //THIS IS THE SCOOREEE
                return 1d - best[0] / HalfDiagonal;
                
        }

        private double DTWPathDistance(ArrayList pts1, ArrayList pts2) //DTW
        {
            int n = pts1.Count;
            int m = pts2.Count;

            // setup the dtw array only if it's larger than last time
            if ((n > _sn) || (m > _sm))
            {
                _sn = n;
                _sm = m;
                _dtw = new double[n, m];
                for (int i = 1; i < m; i++)
                    _dtw[0, i] = Double.MaxValue;
                for (int i = 1; i < n; i++)
                    _dtw[i, 0] = Double.MaxValue;
                _dtw[0, 0] = 0;
            }

            for (int i = 1; i < n; i++)
            {
                for (int j = 1; j < m; j++)
                {
                    PointR p1 = (PointR) pts1[i];
                    PointR p2 = (PointR) pts2[j];
                    double cost = Utils.Distance(p1, p2);

                    _dtw[i, j] = Math.Min(Math.Min(      // min3
                        _dtw[i - 1, j] + cost,           // insertion
                        _dtw[i, j - 1] + cost),          // deletion
                        _dtw[i - 1, j - 1] + cost);      // match
                }
            }

            return _dtw[n - 1, m - 1];
        }

        // From http://www.math.uic.edu/~jan/mcs471/Lec9/gss.pdf
        private double[] GoldenSectionSearch(ArrayList pts1, ArrayList pts2, double a, double b, double threshold)
        {
            double x1 = Phi * a + (1 - Phi) * b;
            ArrayList newPoints = Utils.RotateByRadians(pts1, x1);
            double fx1 = DTWPathDistance(newPoints, pts2) / newPoints.Count;

            double x2 = (1 - Phi) * a + Phi * b;
            newPoints = Utils.RotateByRadians(pts1, x2);
            double fx2 = DTWPathDistance(newPoints, pts2) / newPoints.Count;

            double i = 2.0; // calls
            while (Math.Abs(b - a) > threshold)
            {
                if (fx1 < fx2)
                {
                    b = x2;
                    x2 = x1;
                    fx2 = fx1;
                    x1 = Phi * a + (1 - Phi) * b;
                    newPoints = Utils.RotateByRadians(pts1, x1);
                    fx1 = DTWPathDistance(newPoints, pts2) / newPoints.Count;
                }
                else
                {
                    a = x1;
                    x1 = x2;
                    fx1 = fx2;
                    x2 = (1 - Phi) * a + Phi * b;
                    newPoints = Utils.RotateByRadians(pts1, x2);
                    fx2 = DTWPathDistance(newPoints, pts2) / newPoints.Count;
                }
                i++;
            }
            return new double[3] { Math.Min(fx1, fx2), Utils.Rad2Deg((b + a) / 2.0), i }; // distance, angle, calls to pathdist
        }
 
        #endregion

        #region Gestures & Xml


        public void LoadTrainingData(string data, int value_set)
        {
            string[] lines = data.Split("\n"[0]);
            _training_data = ReadTrainingData(lines, value_set);
        }

        // assumes the reader has been just moved to the head of the content.
        private Gesture ReadTrainingData(string[] data, int value_set)
        {

            ArrayList points = new ArrayList();

            int schmeconds = 0;

            foreach (string line in data)
            {
                string[] csvData = line.Split(',');
                PointR p = PointR.Empty;
                p.X = schmeconds;
                p.Y = Double.Parse(csvData[value_set]);
                p.T = schmeconds;
                points.Add(p);
                schmeconds++;
            }

            return new Gesture(points);
        }

        #endregion
        
	}
}