using System;
using System.Collections;
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

        private bool derivative;

        public const int DATA_X_VALUES = 0;
        public const int DATA_Y_VALUES = 1;
        public const int DATA_Z_VALUES = 2;
        public const int DATA_MAX_VALUES = 3;
        public const bool RECOGNIZE_DTW = false;
        public const bool RECOGNIZE_DDTW = true;
        

		#endregion

		#region Constructor
	
		public DTWRecognizer(string data, int value_set, bool derivative)
		{
            LoadTrainingData(data, value_set);
            this.derivative = derivative;
		}

        #endregion

        #region Recognition

        public ArrayList ddx_avg(ArrayList points)
        {
            ArrayList new_data = new ArrayList();

            for (int i = 1; i < points.Count - 1; i++)
            {

                PointR left_point = ((PointR)points[i - 1]);
                PointR main_point = ((PointR)points[i]);
                PointR next_point = ((PointR)points[i + 1]);
                PointR point = PointR.Empty;

                point.Y = ddx(left_point.Y, main_point.Y, next_point.Y);
                point.X = ddx(left_point.X, main_point.X, next_point.X);
                point.T = (int)ddx(left_point.T, main_point.T, next_point.T);

                new_data.Add(point);

            }

            return new_data;

        }

        public static double ddx(double l_point, double point, double r_point)
        {
            return ((point - l_point) + ((r_point - l_point) / 2)) / 2;
        }

        public double Recognize(ArrayList points) // candidate points
        {
            if(derivative)
                points = ddx_avg(points);

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
        
                                                //input,      training
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
            

            foreach (string line in data)
            {
                string[] csvData = line.Split(',');
                PointR p = PointR.Empty;
                p.X = Double.Parse(csvData[3]);
                p.Y = Double.Parse(csvData[value_set]) * 100;
                p.T = (int)Double.Parse(csvData[3]);
                points.Add(p);
            }
            
            if(derivative)
                points = ddx_avg(points);

            return new Gesture(points);
        }

      

        #endregion

    }
}