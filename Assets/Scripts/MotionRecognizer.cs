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

    public GameObject debugHandle;
    public GameObject movement;

    public double thres;
    
    public bool downlatch;

    // Use this for initialization
    void Start () {
        debugHandle = GameObject.Find("Debug");
        movement = GameObject.Find("Movement");
        debug = debugHandle.GetComponent<TextMesh>();
        debug.text = "NOT DEFAULT";

        debugHandle.transform.parent = this.transform;

        x_rec = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_X_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        y_rec = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Y_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        z_rec = new DTWRecognizer(Training.daniel_walking_acc, DTWRecognizer.DATA_Z_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        Rx_rec = new DTWRecognizer(Training.daniel_walking_gyro, DTWRecognizer.DATA_X_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        Ry_rec = new DTWRecognizer(Training.daniel_walking_gyro, DTWRecognizer.DATA_Y_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        Rz_rec = new DTWRecognizer(Training.daniel_walking_gyro, DTWRecognizer.DATA_Z_VALUES, DTWRecognizer.RECOGNIZE_DTW);
        x_input = new ArrayList();
        y_input = new ArrayList();
        z_input = new ArrayList();
        Rx_input = new ArrayList();
        Ry_input = new ArrayList();
        Rz_input = new ArrayList();
        history = new ArrayList();
        timeElapsed = 0;
        debug.text = "1";
        thres = 0.766;

        downlatch = false;
    }

    private bool latched = false;

    private bool is_walking = false;
    Vector3 move = Vector3.zero;
    private int animate = 30;

    // Update is called once per frame
    void Update () {

        timeElapsed += (Time.deltaTime * 1000);
        

        x_input.Add(new PointR(timeElapsed, Input.acceleration.x * 100, (int) timeElapsed ));
        y_input.Add(new PointR(timeElapsed, Input.acceleration.y * 100, (int) timeElapsed ));
        z_input.Add(new PointR(timeElapsed, Input.acceleration.z * 100, (int) timeElapsed ));

        Rx_input.Add(new PointR(timeElapsed, this.transform.rotation.x * 100, (int) timeElapsed));
        Ry_input.Add(new PointR(timeElapsed, this.transform.rotation.y * 100, (int) timeElapsed));
        Rz_input.Add(new PointR(timeElapsed, this.transform.rotation.z * 100, (int) timeElapsed));


        if (timeElapsed >= 573 || latched)
        {
            if (!latched)
                latched = true;


            double res = 0;
            res += y_rec.Recognize(y_input) * 0.5;    
            res += z_rec.Recognize(z_input) * 0.5;

            history.Add(res);

            double dH = getAverage(history);


            if (dH >= thres && Input.acceleration.y >= -0.95)// && Input.acceleration.x >= 0.5) 
            {
                move = new Vector3(this.transform.forward.x, 0, this.transform.forward.z);
                animate = 30;
            }

            //(new Vector3(this.transform.forward.x, 0, this.transform.forward.z) * 3 * Time.deltaTime)
            if (animate > 0)
            {
                animate--;
                movement.transform.position = Vector3.Lerp(movement.transform.position, movement.transform.position + move, 0.02f);
            }

            string result = string.Format("{0:0.0000}\nN/A:\n{1}", dH, 0);
            // string result = string.Format("X:{0:0.0000}\nY:{1:0.0000}\nZ:{2:0.0000}\nRX{3:0.0000}\nRY{4:0.0000}\nRZ{5:0.0000}",
             //  x_rec.Recognize(x_input), y_rec.Recognize(y_input), z_rec.Recognize(z_input), Rx_rec.Recognize(Rx_input),
              //  Ry_rec.Recognize(Ry_input), Rz_rec.Recognize(Rz_input));

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
        foreach (double i in data)
        {
            sum += i;
        }
        return sum / data.Count;
    }
}
