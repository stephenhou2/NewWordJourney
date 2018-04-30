using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

    public class ZoomHUD : MonoBehaviour
    {

        public Transform contentContainer;

        protected float zoomDuration = 0.2f;
        protected IEnumerator zoomCoroutine;


        protected IEnumerator HUDZoomIn()
        {

            contentContainer.localScale = new Vector3(0.2f, 0.2f, 1);

            float scale = 0.2f;

            float zoomInSpeed = (1 - scale) / zoomDuration;

            float lastFrameRealTime = Time.realtimeSinceStartup;

            while (scale < 1)
            {

                yield return null;

                scale += zoomInSpeed * (Time.realtimeSinceStartup - lastFrameRealTime);

                lastFrameRealTime = Time.realtimeSinceStartup;

                contentContainer.transform.localScale = new Vector3(scale, scale, 1);

            }

            contentContainer.transform.localScale = Vector3.one;


        }

        protected IEnumerator HUDZoomOut()
        {

            float scale = 1f;

            float zoomInSpeed = (1 - 0.2f) / zoomDuration;

            float lastFrameRealTime = Time.realtimeSinceStartup;

            while (scale > 0.2f)
            {

                yield return null;

                scale -= zoomInSpeed * (Time.realtimeSinceStartup - lastFrameRealTime);

                lastFrameRealTime = Time.realtimeSinceStartup;

                contentContainer.transform.localScale = new Vector3(scale, scale, 1);

            }

            contentContainer.localScale = new Vector3(0.2f, 0.2f, 1);

            this.gameObject.SetActive(false);

        }


    }


}
