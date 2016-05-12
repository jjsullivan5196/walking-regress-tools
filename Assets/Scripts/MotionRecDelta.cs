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


    // it was 3 for num inputs
    int num_input_points = 5;
    double rise_threshold_lower = 0.8;
    double rise_threshold_upper = 1;
    double continue_threshold_lower = 0.75;
    double continue_threshold_upper = 1;
    double fall_threshold_lower = 0.65;
    double fall_threshold_upper = 1;
    double stop_threshold = 2.5;
    bool start_detected = false;
    bool continue_detected = false;
    bool stop = false;
    

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

            // if (yrec.Recognize(y_input, SeriesRecognizer.ACT_STOP) >= stop_threshold)
            // {



            if (!continue_detected)
            {
                double score = yrec.Recognize(y_input, SeriesRecognizer.ACT_RISE);

                if (score <= rise_threshold_upper && score >= rise_threshold_lower)
                {
                    start_detected = true;
                    stop = false;
                    debug.text = "Start";
                }
            }
            else
            {
                debug.text = "Mid";
                stop = true;
            }

            if (start_detected)
            {
                double score = yrec.Recognize(y_input, SeriesRecognizer.ACT_CONT);

                if (score <= continue_threshold_upper && score >= continue_threshold_lower)
                {
                    continue_detected = true;
                    stop = false;
                    debug.text = "Cont";
                }
            }
            else
            {
                debug.text = "Mid";
                stop = true;
            }

            if (continue_detected)
                {
                    double score = yrec.Recognize(y_input, SeriesRecognizer.ACT_FALL);

                    if (score <= fall_threshold_upper && score >= fall_threshold_lower)
                    {
                        debug.text = "Fall";
                        continue_detected = start_detected = false;
                    }
                }
            else
            {
                debug.text = "Mid";
                stop = true;
            }


            if ((start_detected || continue_detected) && !stop)
            {
                Vector3 forward = new Vector3(this.transform.forward.x, 0, this.transform.forward.z) * 10;
                move.transform.position = Vector3.Lerp(move.transform.position, move.transform.position + forward, 0.5f);
            }

            //  }
            //  else
            //  {
            //      debug.text = "Stop";
            //  }

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
