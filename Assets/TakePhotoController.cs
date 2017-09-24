using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TakePhotoController : MonoBehaviour {

    public bool takePhoto = false;
    private bool tookPhoto = false;
    public int resWidth = 1200;
    public int resHeight = 1080;
    public int startX = 100;
    public int startY = 100;
    private Animal[] animals;

    private Camera photoCamera;
    private Canvas canvas;

    private string folder;
    private bool shouldCapturePhoto = false;
    private bool photoCaptured = false;

    // Use this for initialization
    void Start () {
        //webCamTexture = new WebCamTexture();
        //webCamTexture.Play();
        photoCamera = GetComponent<Camera>();
        canvas = GetComponentInChildren<Canvas>();

        folder = Application.dataPath;
        if (Application.isEditor)
        {
            // put screenshots in folder above asset path so unity doesn't index the files
            var stringPath = folder + "/..";
            folder = Path.GetFullPath(stringPath);
        }
        folder += "/screenshots";

        animals = FindObjectsOfType<Animal>();

        // make sure directoroy exists
        System.IO.Directory.CreateDirectory(folder);
    }

    private void Update()
    {
        if (takePhoto && !tookPhoto)
        {
            tookPhoto = true;
            shouldCapturePhoto = true;
            canvas.gameObject.SetActive(false);
        }
        else if (!takePhoto && tookPhoto)
        {
            tookPhoto = false;
        }
        if (photoCaptured)
        {
            photoCaptured = false;
            canvas.gameObject.SetActive(true);
        }
    }

    // Image captures runs in onPostRender so that we have screen pixels to grab on calling ReadPixels
    void OnPostRender () {
		if (shouldCapturePhoto)
        {
            shouldCapturePhoto = false;
            TakePhoto();
        }

	}

    void TakePhoto()
    {

        //RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        //photoCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        //photoCamera.Render();
        //RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(startX, startY, resWidth, resHeight), 0, 0);
        //photoCamera.targetTexture = null;
        //RenderTexture.active = null; // JC: added to avoid errors
        //Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(resWidth, resHeight);

        new System.Threading.Thread(() =>
        {
            // create file and write optional header with image bytes
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, bytes.Length));
            photoCaptured = true;
        }).Start();
        foreach (Animal animal in animals)
        {
            IsInView(animal);
        }
    }

    public string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screen_{1}x{2}_{3}.png",
                             folder,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    private bool IsInView(Animal toCheck)
    {
        Vector3 pointOnScreen = photoCamera.WorldToScreenPoint(toCheck.renderer.bounds.center);

        //Is in front
        if (pointOnScreen.z < 0)
        {
            Debug.Log("Behind: " + toCheck.name);
            return false;
        }
        // Is too far away to see properly
        if (pointOnScreen.z > toCheck.maxVisibleRange)
        {
            Debug.Log("Too far: " + toCheck.name);
            return false;
        }


        //Is in FOV
        //if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
        //        (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
        if ((pointOnScreen.x < startX) || (pointOnScreen.x > startX + resWidth) ||
            (pointOnScreen.y < startY) || (pointOnScreen.y > startY + resHeight))
        {
            Debug.Log("Out of FOV: " + toCheck.name);
            return false;
        }

        RaycastHit hit;
        //Vector3 heading = toCheck.transform.position - photoCamera.transform.position;
        //Vector3 direction = heading.normalized;// / heading.magnitude;

        if (Physics.Linecast(photoCamera.transform.position, toCheck.renderer.bounds.center, out hit))
        {
            if (hit.transform.name != toCheck.name)
            {
                /* -->
                Debug.DrawLine(cam.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, Color.red);
                Debug.LogError(toCheck.name + " occluded by " + hit.transform.name);
                */
                Debug.Log(toCheck.name + " occluded by " + hit.transform.name);
                return false;
            } else
            {
                Debug.Log("Detected: " + toCheck.name + " in view");
            }
        }
        return true;
    }
}
