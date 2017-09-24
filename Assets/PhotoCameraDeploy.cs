using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCameraDeploy : MonoBehaviour {

    public float delay = 5f;
    public float animTime = 1.2f;

    private bool isShowing = false;
    private bool isAnimating = false;
    private RectTransform rectTransform;
    private Rect rect;
    private float startTime = 0f;
    private Vector2 startPos = new Vector2(0, 1);
    private Vector2 endPos = new Vector2(0, 0);
    private Vector3 endScale = new Vector3(1.4f, 1.4f, 1f);

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        rect = rectTransform.rect;
        rect.yMin = rect.height;
        rectTransform.anchorMin = startPos;


    }
	
	// Update is called once per frame
	void Update () {
		if (!isShowing && Time.time > delay)
        {
            isShowing = true;
            isAnimating = true;
            startTime = Time.time;
        }
        if (isAnimating)
        {
            float ratio = (Time.time - startTime) / animTime;
            rectTransform.anchorMin = Vector2.Lerp(startPos, endPos, ratio);
            rectTransform.localScale = Vector3.Lerp(Vector3.one, endScale, ratio);
            if (ratio > 1)
                isAnimating = false;
        }
	}
}
