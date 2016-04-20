using UnityEngine;
using System.IO;
using System.Collections;

public class Record : MonoBehaviour {
    public System.DateTime epoch;
    public StreamWriter acc;
    public StreamWriter gyro;
    public MemoryStream fsAcc;
    public MemoryStream fsGyro;
    public string uploadURL;
    public int curtime;
    public float timeelapsed;
    public bool record;
    public GameObject mainCamera;
    public TextMesh debugText;

	// Use this for initialization
	void Start () {
        epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        fsAcc = new MemoryStream();
        fsGyro = new MemoryStream();
        acc = new StreamWriter(fsAcc);
        gyro = new StreamWriter(fsGyro);

        timeelapsed = 0;
        record = false;
        curtime = 0;
        uploadURL = "http://10.12.174.79/upload.php";

        debugText = GetComponent<TextMesh>();
        mainCamera = GameObject.Find("Main Camera");
        this.transform.parent = mainCamera.transform;
    }

    public IEnumerator sendData(WWW sendTo) {
        yield return sendTo;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Tap") && !record) record = !record;
        else if(Input.GetButton("Back") && record) {
            acc.WriteLine("{0},{1},{2},{3}", Input.acceleration.x, Input.acceleration.y, Input.acceleration.z, timeelapsed);
            gyro.WriteLine("{0},{1},{2},{3}", mainCamera.transform.rotation.x, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, timeelapsed);
            curtime = (int)(System.DateTime.UtcNow - epoch).TotalSeconds;

            byte[] accBytes = fsAcc.ToArray();
            WWWForm sendAcc = new WWWForm();
            sendAcc.AddField("frameCount", Time.frameCount.ToString());
            sendAcc.AddBinaryData("file", accBytes, "acc" + curtime + ".csv", "text/plain");
            WWW uploadAcc = new WWW(uploadURL, sendAcc);
            sendData(uploadAcc);

            byte[] gyroBytes = fsGyro.ToArray();
            WWWForm sendGyro = new WWWForm();
            sendAcc.AddField("frameCount", Time.frameCount.ToString());
            sendAcc.AddBinaryData("file", gyroBytes, "gyro" + curtime + ".csv", "text/plain");
            WWW uploadGyro = new WWW(uploadURL, sendAcc);
            sendData(uploadGyro);

            record = !record;
            timeelapsed = 0;

            acc.Flush();
            gyro.Flush();

            fsAcc.SetLength(0);
            fsGyro.SetLength(0);
        }
        if (record) {
            debugText.text = "RECORDING";
            timeelapsed += Time.deltaTime * 1000;
            acc.WriteLine("{0},{1},{2},{3}", Input.acceleration.x, Input.acceleration.y, Input.acceleration.z, timeelapsed);
            gyro.WriteLine("{0},{1},{2},{3}", mainCamera.transform.rotation.x, mainCamera.transform.rotation.y, mainCamera.transform.rotation.z, timeelapsed);
        }
        if(!record) debugText.text = "NOT RECORD";
	}
}
