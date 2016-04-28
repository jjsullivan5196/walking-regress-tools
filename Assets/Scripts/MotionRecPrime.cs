using UnityEngine;
using Recognizer.DTW;
using System.Collections;

public class MotionRecPrime : MonoBehaviour {
    public TextMesh debug;
    public GameObject move;

    private DTWRecognizer xrec_acc;
    private DTWRecognizer yrec_acc;
    private DTWRecognizer zrec_acc;
    private DTWRecognizer xrec_gyro;
    private DTWRecognizer yrec_gyro;
    private DTWRecognizer zrec_gyro;

    private ArrayList xinput_acc;
    private ArrayList yinput_acc;
    private ArrayList zinput_acc;
    private ArrayList xinput_gyro;
    private ArrayList yinput_gyro;
    private ArrayList zinput_gyro;

    private ArrayList history;
    private ArrayList avg_history;

    private float timeElapsed;

    private bool is_walking;

	// Use this for initialization
	void Start () {
        debug = GameObject.Find("Debug").GetComponent<TextMesh>();
        move = GameObject.Find("Movement");

        debug.text = "MPRIME";

        xrec_acc = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_X_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        yrec_acc = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Y_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        zrec_acc = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Z_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        xrec_gyro = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_X_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        yrec_gyro = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Y_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        zrec_gyro = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Z_VALUES, DTWRecognizer.RECOGNIZE_DTW);

        xinput_acc = new ArrayList();
        yinput_acc = new ArrayList();
        zinput_acc = new ArrayList();
        xinput_gyro = new ArrayList();
        yinput_gyro = new ArrayList();
        zinput_gyro = new ArrayList();

        history = new ArrayList();
        avg_history = new ArrayList();

        timeElapsed = 0;

        is_walking = false;
	}

    // Update is called once per frame
    void Update() {
        timeElapsed += (Time.deltaTime * 1000);

        xinput_acc.Add(new PointR(timeElapsed, Input.acceleration.x * 100, (int)timeElapsed));
        yinput_acc.Add(new PointR(timeElapsed, Input.acceleration.y * 100, (int)timeElapsed));
        zinput_acc.Add(new PointR(timeElapsed, Input.acceleration.z * 100, (int)timeElapsed));

        xinput_gyro.Add(new PointR(timeElapsed, Input.acceleration.x * 100, (int)timeElapsed));
        yinput_gyro.Add(new PointR(timeElapsed, Input.acceleration.y * 100, (int)timeElapsed));
        zinput_gyro.Add(new PointR(timeElapsed, Input.acceleration.z * 100, (int)timeElapsed));

        if (timeElapsed >= 573) {
            double res = 0;
            double score = 0;

            res += yrec_acc.Recognize(yinput_acc) * 0.5;
            res += zrec_acc.Recognize(zinput_acc) * 0.5;
            history.Add(res);

            if (history.Count >= 20) {
                avg_history.Add(getAverage(history));
                if (avg_history.Count >= 3)
                {
                    score = DTWRecognizer.ddx((double)avg_history[2], (double)avg_history[1], (double)avg_history[0]);
                    avg_history.RemoveAt(0);
                }
                history.RemoveAt(0);
            }

            if (System.Math.Abs(score * 100) >= 1)
                move.transform.position = Vector3.Lerp(move.transform.position, move.transform.position + new Vector3(this.transform.forward.x, 0, this.transform.forward.z) * 1, 35);

            debug.text = string.Format("SCORE: {0:0.0000}", System.Math.Abs(score) * 100);

            xinput_acc.RemoveAt(0);
            yinput_acc.RemoveAt(0);
            zinput_acc.RemoveAt(0);
            xinput_gyro.RemoveAt(0);
            yinput_gyro.RemoveAt(0);
            zinput_gyro.RemoveAt(0);
        }
    }


    double getAverage(ArrayList data)
    {
        if (data.Count == 0)
        {
            return 0.0;
        }
        double sum = 0;
        foreach (double i in data)
        {
            sum += i;
        }
        return sum / data.Count;
    }
}
