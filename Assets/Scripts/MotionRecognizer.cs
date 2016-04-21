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

    public ArrayList Rx_input;
    public ArrayList Ry_input;
    public ArrayList Rz_input;

    public ArrayList history;

    private float timeElapsed;
    static double max_thres = 0.0;
    public int steps = 0;

    public GameObject debugHandle;
    public GameObject movement;
    //private static string dataPath = "jar:file://" + Application.dataPath + "!/assets/";

    public double thres;

    public bool uplatch;
    public bool downlatch;

    // Use this for initialization
    void Start () {
        debugHandle = GameObject.Find("Debug");
        movement = GameObject.Find("Movement");
        debug = debugHandle.GetComponent<TextMesh>();
        debug.text = "NOT DEFAULT";

        debugHandle.transform.parent = this.transform;

        x_rec = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_X_VALUES);
        y_rec = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Y_VALUES);
        z_rec = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Z_VALUES);
        Rx_rec = new DTWRecognizer(Training.daniel_walking_gyro, DTWRecognizer.DATA_X_VALUES);
        Ry_rec = new DTWRecognizer(Training.daniel_walking_gyro, DTWRecognizer.DATA_Y_VALUES);
        Rz_rec = new DTWRecognizer(Training.daniel_walking_gyro, DTWRecognizer.DATA_Z_VALUES);
        x_input = new ArrayList();
        y_input = new ArrayList();
        z_input = new ArrayList();
        Rx_input = new ArrayList();
        Ry_input = new ArrayList();
        Rz_input = new ArrayList();
        history = new ArrayList();
        timeElapsed = 0;
        debug.text = "1";
        thres = 0.76;

        uplatch = downlatch = false;
    }

    private bool latched = false;

	// Update is called once per frame
	void Update () {

        timeElapsed += Time.deltaTime * 1000;
        

        x_input.Add(new PointR(timeElapsed, Input.acceleration.x * 100, (int) timeElapsed ));
        y_input.Add(new PointR(timeElapsed, Input.acceleration.y * 100, (int) timeElapsed ));
        z_input.Add(new PointR(timeElapsed, Input.acceleration.z * 100, (int) timeElapsed ));
    
        Rx_input.Add(new PointR(timeElapsed, this.transform.rotation.x * 100, (int) timeElapsed));
        Ry_input.Add(new PointR(timeElapsed, this.transform.rotation.y * 100, (int) timeElapsed));
        Rz_input.Add(new PointR(timeElapsed, this.transform.rotation.z * 100, (int) timeElapsed));

        //debug.text = Time.deltaTime + "";


        if (timeElapsed >= 573 || latched)
        {
            if(!latched)
                latched = true;
            
            double res = 0;
            res += y_rec.Recognize(y_input) * .5;
            res += z_rec.Recognize(z_input) * .5;
         
            history.Add(res);

            double dH = getAverage(history);

            if (dH >= thres)
            {
                steps++;
                movement.transform.Translate(Vector3.Lerp(movement.transform.position, new Vector3(this.transform.forward.x,0, this.transform.forward.z) * 4 * Time.deltaTime, 2));
            }
            

            string result = string.Format("{0:0.0000}\nThres:\n{1}", getAverage(history), thres);
            // string result = string.Format("X:{0:0.0000}\nY:{1:0.0000}\nZ:{2:0.0000}\nRX{3:0.0000}\nRY{4:0.0000}\nRZ{5:0.0000}",
            //    x_rec.Recognize(x_input), y_rec.Recognize(y_input), z_rec.Recognize(z_input), Rx_rec.Recognize(Rx_input),
            //    Ry_rec.Recognize(Ry_input), Rz_rec.Recognize(Rz_input));

            debug.text = "REC OUT:\n" + result;
            x_input.RemoveAt(0);
            y_input.RemoveAt(0);
            z_input.RemoveAt(0);
            Rx_input.RemoveAt(0);
            Ry_input.RemoveAt(0);
            Rz_input.RemoveAt(0);
            if (history.Count > 10)
            {
                history.RemoveAt(0);
            }

            /*if(!uplatch && Input.GetButtonDown("Back"))
            {
                thres += 0.001;
                uplatch = true;
            }
            if (uplatch && Input.GetButtonUp("Back")) uplatch = false;*/

            if(!downlatch && Input.GetButtonDown("Tap"))
            {
                thres += 0.001;
                downlatch = true;
            }
            if (downlatch && Input.GetButtonUp("Tap")) downlatch = false;
        }
	}

    double getAverage(ArrayList data)
    {
        if (data.Count == 0)
        {
            return 0.0;
        }
        double sum = 0;
        foreach (double i in data){
            sum += i;
        }
        return sum / data.Count;
    }
}
