//FricDTW -- It's just frickin DTW!!! (For common C#)
//Implementation authored by John Sullivan (jsullivan@csumb.edu - gh:jjsullivan5196)
//Algorithim shamelessly stolen from Wikipedia ( https://en.wikipedia.org/wiki/Dynamic_time_warping#Implementation )
//All copies/derivations of this software fall under the MIT License ( https://opensource.org/licenses/MIT )
//Copyright 2016 John Sullivan
//Long live Professor Wobbrock

using System;
using System.Linq;
using System.Collections.Generic;

namespace FricDTW
{
	public class tPoint
	{
		private double yDisp;
		private double time;
		
		public tPoint(double yDisp, double time)
		{
			this.yDisp = yDisp;
			this.time = time;
		}
		
		public static double dist(tPoint p1, tPoint p2)
		{
			double d = Math.Pow((p2.yDisp - p1.yDisp), 2);
			return Math.Sqrt(d);
		}
	}
	
	public class RecognizerDTW
	{
		private List<tPoint> train;

        public const int DATA_X = 0;
        public const int DATA_Y = 1;
        public const int DATA_Z = 2;
        public const int DATA_T = 3;

        public RecognizerDTW() { }
		
		public RecognizerDTW(List<tPoint> train)
		{
			this.train = train;
		}

        public RecognizerDTW(string data, int field)
        {
            List<tPoint> train = new List<tPoint>();
            string[] tdata = data.Split('\n');
            foreach(string line in tdata)
            {
                string[] d = line.Split(',');
                train.Add(new tPoint(Double.Parse(d[field]), Double.Parse(d[DATA_T])));
            }
            this.train = train;
        }
		
		public double DTWDistance(List<tPoint> input)
		{
			double[,] DTW = new double[train.Count, input.Count];
			
			for(int i = 1; i < train.Count; i++)
				DTW[i, 0] = Single.MaxValue;
			for(int i = 1; i < input.Count; i++)
				DTW[0, i] = Single.MaxValue;
			DTW[0, 0] = 0;
			
			for(int i = 1; i < train.Count; i++)
				for(int j = 1; j < input.Count; j++)
				{
					double trv = new double[] {DTW[i-1, j], DTW[i, j-1], DTW[i-1, j-1]}.Min();
					double cost = tPoint.dist(train[i], input[j]);
					DTW[i, j] = cost + trv;
				}
			
			return DTW[train.Count - 1, input.Count - 1];
		}
		
		public double DTWDistanceWindow(List<tPoint> input, int window)
		{
			if(Math.Abs(train.Count - input.Count) > window) return -1;
			
			double[,] DTW = new double[train.Count, input.Count];
			
			for(int i = 0; i < train.Count; i++)
				for(int j = 0; j < input.Count; j++)
					DTW[i, j] = Single.MaxValue;
			DTW[0, 0] = 0;
			
			for(int i = 1; i < train.Count; i++)
				for(int j = Math.Max(1, i - window); j < Math.Min(input.Count, i + window); j++)
				{
					double trv = new double[] {DTW[i-1, j], DTW[i, j-1], DTW[i-1, j-1]}.Min();
					double cost = tPoint.dist(train[i], input[j]);
					DTW[i, j] = cost + trv;
				}
				
			return DTW[train.Count - 1, input.Count - 1];
		}
	}

    public class SeriesRecognizer
    {
        private RecognizerDTW[] activity;

        public const int ACT_RISE = 0;
        public const int ACT_CONT = 1;
        public const int ACT_FALL = 2;
        public const int ACT_STOP = 3;
        public const int MAXACT = 4;

        public SeriesRecognizer(List<tPoint>[] input)
        {
            activity = new RecognizerDTW[MAXACT];
            for (int i = 0; i < MAXACT; i++)
                    activity[i] = new RecognizerDTW(input[i]);
        }

        public SeriesRecognizer(string[] input, int field)
        {
            activity = new RecognizerDTW[MAXACT];
            for (int i = 0; i < MAXACT; i++)
                activity[i] = new RecognizerDTW(input[i], field);
        }

        public double Recognize(List<tPoint> input, int step)
        {
            return activity[step].DTWDistance(input);
        }
    }
}