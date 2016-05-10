using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Recognizer.DTW;
using FricDTW;

public class RecordRecog : MonoBehaviour {
    public System.DateTime epoch;
    public StreamWriter recog;
    public MemoryStream fsRecog;
    public string uploadURL;
    public int curtime;
    public float timeelapsed;
    public bool record;
    public TextMesh debugText;

    private List<tPoint> y_input;
    private SeriesRecognizer yrec;



    // Use this for initialization
    void Start () {
        epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        fsRecog = new MemoryStream();
        recog = new StreamWriter(fsRecog);

        timeelapsed = 0;
        record = false;
        curtime = 0;
        uploadURL = "http://10.12.174.79/upload.php";

        yrec = new SeriesRecognizer(new string[] { Training.acc_step_rise, Training.acc_step_cont, Training.acc_step_fall, Training.acc_step_stop }, DTWRecognizer.DATA_Y_VALUES);
        y_input = new List<tPoint>();

        debugText = GameObject.Find("Debug").GetComponent<TextMesh>();
    }

    public IEnumerator sendData(WWW sendTo) {
        yield return sendTo;
    }
	
	// Update is called once per frame
	void Update () {
        y_input.Add(new tPoint(Input.acceleration.y, timeelapsed));

        if (y_input.Count >= 5)
        {
            if (Input.GetButton("Tap") && !record) record = !record;
            else if (Input.GetButton("Back") && record)
            {
                recog.WriteLine("{0},{1},{2},{3}", yrec.Recognize(y_input, SeriesRecognizer.ACT_RISE), yrec.Recognize(y_input, SeriesRecognizer.ACT_CONT), yrec.Recognize(y_input, SeriesRecognizer.ACT_FALL), yrec.Recognize(y_input, SeriesRecognizer.ACT_STOP));
                curtime = (int)(System.DateTime.UtcNow - epoch).TotalSeconds;

                byte[] recogBytes = fsRecog.ToArray();
                WWWForm sendRecog = new WWWForm();
                sendRecog.AddField("frameCount", Time.frameCount.ToString());
                sendRecog.AddBinaryData("file", recogBytes, "recog" + curtime + ".csv", "text/plain");
                WWW uploadRecog = new WWW(uploadURL, sendRecog);
                sendData(uploadRecog);

                record = !record;
                timeelapsed = 0;

                recog.Flush();
                fsRecog.SetLength(0);
            }
            if (record)
            {
                debugText.text = "RECORDING";
                timeelapsed += Time.deltaTime * 1000;
                recog.WriteLine("{0},{1},{2},{3}", yrec.Recognize(y_input, SeriesRecognizer.ACT_RISE), yrec.Recognize(y_input, SeriesRecognizer.ACT_CONT), yrec.Recognize(y_input, SeriesRecognizer.ACT_FALL), yrec.Recognize(y_input, SeriesRecognizer.ACT_STOP));
            }
            if (!record) debugText.text = "NOT RECORD";

            y_input.RemoveAt(0);
        }
	}
}
