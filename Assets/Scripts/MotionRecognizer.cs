using UnityEngine;
using System.Collections;
using Recognizer.DTW;

public class MotionRecognizer : MonoBehaviour {
    public TextMesh debug;
    public DTWRecognizer x_rec;
    public DTWRecognizer y_rec;
    public DTWRecognizer z_rec;
    public DTWRecognizer Rx_rec;
    public DTWRecognizer Ry_rec;
    public DTWRecognizer Rz_rec;
    public ArrayList x_input;
    public ArrayList y_input;
    public ArrayList z_input;

    private float timeElapsed;
    //private static string dataPath = "jar:file://" + Application.dataPath + "!/assets/";

    // Use this for initialization
    void Start () {
        debug = GameObject.Find("MRec").GetComponent<TextMesh>();
        //debug.text = "NOT DEFAULT";
        x_rec = new DTWRecognizer(Training.john_walking_acc, DTWRecognizer.DATA_X_VALUES);
        y_rec = new DTWRecognizer(Training.john_walking_acc, DTWRecognizer.DATA_Y_VALUES);
        z_rec = new DTWRecognizer(Training.john_walking_acc, DTWRecognizer.DATA_Z_VALUES);
        x_input = new ArrayList();
        y_input = new ArrayList();
        z_input = new ArrayList();
	}
    
	// Update is called once per frame
	void Update () {

        timeElapsed += Time.deltaTime;

        //PROBLEM HERE, cast of int will always!!! be 0
        x_input.Add(new PointR(timeElapsed, Input.acceleration.x, (int)timeElapsed));
        y_input.Add(new PointR(timeElapsed, Input.acceleration.y - 0.980f, (int)timeElapsed));
        z_input.Add(new PointR(timeElapsed, Input.acceleration.z, (int)timeElapsed));
        
        
        //debug.text = Time.deltaTime + "";
        if (timeElapsed >= 0.250)
        {
            double x_res = x_rec.Recognize(x_input);
            double y_res = y_rec.Recognize(y_input);
            double z_res = z_rec.Recognize(z_input);

            double basic_avg = (x_res + y_res + z_res) / 3;

            string result = string.Format("{0:0.0000}", basic_avg);
            //debug.text = "REC OUT: " + result;
            x_input = new ArrayList();
            y_input = new ArrayList();
            z_input = new ArrayList();
            timeElapsed = 0;

        }

        
	
	}
}
