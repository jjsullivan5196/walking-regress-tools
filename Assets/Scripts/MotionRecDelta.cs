using UnityEngine;
using FricDTW;
using Recognizer.DTW;
using System.Collections;
using System.Collections.Generic;

public class MotionRecDelta : MonoBehaviour {
    public TextMesh debug;
    public GameObject move;

    //private RecognizerDTW yrec;
    private SeriesRecognizer yrec;
    private List<tPoint> y_input;
    private List<double> history;

    private double timeElapsed;

	// Use this for initialization
	void Start () {
        debug = GameObject.Find("Debug").GetComponent<TextMesh>();
        move = GameObject.Find("Movement");

        //yrec = new RecognizerDTW(Training.daniel_walking_acc, RecognizerDTW.DATA_Y);
        yrec = new SeriesRecognizer(new string[] { Training.acc_step_rise, Training.acc_step_cont, Training.acc_step_fall, Training.acc_step_stop }, DTWRecognizer.DATA_Y_VALUES);
        y_input = new List<tPoint>();
        history = new List<double>();

        timeElapsed = 0;
	}

    int num_input_points = 3;
    double rise_threshold = 1;
    double continue_threshold = 1;
    double fall_threshold = 1;
    double stop_threshold = 0.5;
    bool start_detected = false;
    bool continue_detected = false;
    

	// Update is called once per frame
	void Update () {
        timeElapsed += Time.deltaTime * 1000;
        y_input.Add(new tPoint(Input.acceleration.y, timeElapsed));

        if (y_input.Count >= num_input_points)
        {

#if false // DEBUG NEVER AFTER THERSHOLD  ???HISTORY???

            double rise = yrec.Recognize(y_input, SeriesRecognizer.ACT_RISE);
            double cont = yrec.Recognize(y_input, SeriesRecognizer.ACT_CONT);
            double fall = yrec.Recognize(y_input, SeriesRecognizer.ACT_FALL);
            double stop = yrec.Recognize(y_input, SeriesRecognizer.ACT_STOP);

            string result = string.Format("{0:0.0000}\n{1:0.0000}\n{2:0.0000}\n{3:0.0000}", rise, cont, fall, stop);

            debug.text = result;

            
#endif

            if (yrec.Recognize(y_input, SeriesRecognizer.ACT_STOP) >= stop_threshold)
            {

                if (!continue_detected && yrec.Recognize(y_input, SeriesRecognizer.ACT_RISE) <= rise_threshold)
                {
                    start_detected = true;
                    debug.text = "Start";
                }
                if (start_detected && yrec.Recognize(y_input, SeriesRecognizer.ACT_CONT) <= continue_threshold)
                {
                    continue_detected = true;
                    debug.text = "Cont";
                }
                if (continue_detected && yrec.Recognize(y_input, SeriesRecognizer.ACT_FALL) <= fall_threshold)
                {
                    debug.text = "Fall";
                    continue_detected = start_detected = false;
                }

            }
            else
            {
                debug.text = "Stop";
            }

            y_input.RemoveAt(0);

        }

        
       
	}

    double getAverage(List<double> t)
    {
        double res = 0;
        foreach (double val in t)
            res += val;
        return res / t.Count;
    }
}
