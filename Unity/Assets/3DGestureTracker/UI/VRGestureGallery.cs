﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace WinterMute
{
    public class VRGestureGallery : MonoBehaviour
    {
        VRGestureManager vrGestureManager;

        public GameObject framePrefab;

        public float gestureDrawSize;
        public float gridUnitSize;
        public int gridMaxColumns;
        private Vector3 frameOffset;
        public float lineWidth;
        public Vector3 galleryPosition;

        public string currentGesture;
        private string currentNeuralNet;

        List<GestureExample> examples;

        // Use this for initialization
        void Start()
        {
            vrGestureManager = FindObjectOfType<VRGestureManager>();
            frameOffset = new Vector3(gridUnitSize / 4, gridUnitSize / 4, -(gridUnitSize / 2));
        }

        void RefreshGestureExamples ()
        {
            examples = GetGestureExamples();
        }

        void GenerateGestureGallery()
        {
            
            float xPos = 0;
            float yPos = 0;
            int column = 0;
            int row = 0;

            // draw gesture at position
            float gridStartPosX = (gridUnitSize * gridMaxColumns) / 2;
            int gridMaxRows = examples.Count / gridMaxColumns;
            float gridStartPosY = (gridUnitSize * gridMaxRows) / 2;
            Debug.Log(gridStartPosX);

            // go through all the gesture examples and draw them in a grid
            for (int i = 0; i < examples.Count; i++)
            {

                // set the next position
                xPos = column * gridUnitSize;
                yPos = -row * gridUnitSize;

                // offset positions to center the transform
                xPos -= gridStartPosX;
                yPos += gridStartPosY;

                Vector3 localPos = new Vector3(xPos, yPos, 0);
                
                // draw the gesture
                GameObject gestureLine = DrawGesture(examples[i].data, localPos, i);

                // draw the frame
                Vector3 framePos = localPos + frameOffset;
                GameObject frame = GameObject.Instantiate(framePrefab) as GameObject;
                frame.transform.parent = transform;
                frame.transform.localPosition = framePos;
                frame.transform.localRotation = Quaternion.identity;
                frame.name = "Frame " + i;
                frame.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridUnitSize);
                frame.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gridUnitSize);
                Button frameButton = frame.GetComponent<Button>();
                GestureExample example = examples[i];
                GameObject lineObj = gestureLine;
                frameButton.onClick.AddListener(() => CallDeleteGesture(example, frame, lineObj));


                // change column or row
                column += 1;
                if (column >= gridMaxColumns)
                {
                    column = 0;
                    row += 1;
                }
            }
        }

        void CallDeleteGesture(GestureExample gestureExample, GameObject frame, GameObject line)
        {
            int lineNumber = examples.IndexOf(gestureExample);
            examples.Remove(gestureExample);
            Utils.Instance.DeleteGestureExample(currentNeuralNet, currentGesture, lineNumber);
            GameObject.Destroy(frame);
            GameObject.Destroy(line);
            //DestroyGestureGallery();
            //GenerateGestureGallery();
        }

        List<GestureExample> GetGestureExamples()
        {
            //read in the file
            string filePath = Config.SAVE_FILE_PATH + vrGestureManager.currentNeuralNet + "/Gestures/";
            string fileName = currentGesture + ".txt";
            string[] lines = System.IO.File.ReadAllLines(filePath + fileName);
            List<GestureExample> gestures = new List<GestureExample>();
            foreach (string currentLine in lines)
            {
                gestures.Add(JsonUtility.FromJson<GestureExample>(currentLine));
            }
            return gestures;
        }

        void DestroyGestureGallery()
        {
            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));
        }

        GameObject DrawGesture(List<Vector3> capturedLine, Vector3 startCoords, int gestureExampleNumber)
        {
            // create a game object
            //Debug.Log(startCoords);
            GameObject tmpObj = new GameObject();
            tmpObj.name = "Gesture Example " + gestureExampleNumber;
            tmpObj.transform.SetParent(transform);
            tmpObj.transform.localPosition = startCoords;
            tmpObj.transform.localRotation = Quaternion.identity;

            // get the list of points in capturedLine and modify positions based on gestureDrawSize
            List<Vector3> capturedLineAdjusted = new List<Vector3>();
            foreach (Vector3 point in capturedLine)
            {
                Vector3 pointScaled = point * gestureDrawSize;
                capturedLineAdjusted.Add(pointScaled);
            }

            LineRenderer lineRenderer = tmpObj.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.SetColors(Color.red, Color.red);
            lineRenderer.SetWidth(lineWidth, lineWidth);
            lineRenderer.SetVertexCount(capturedLineAdjusted.Count);
            lineRenderer.SetPositions(capturedLineAdjusted.ToArray());

            return tmpObj;
        }

        void OnEnable()
        {
            PanelManager.OnPanelFocusChanged += PanelFocusChanged;
        }

        void OnDisable()
        {
            PanelManager.OnPanelFocusChanged -= PanelFocusChanged;
        }

        void PanelFocusChanged (string panelName)
        {
            if (panelName == "Editing Menu")
            {
                currentGesture = vrGestureManager.gestureToRecord;
                currentNeuralNet = vrGestureManager.currentNeuralNet;
                RefreshGestureExamples();
                GenerateGestureGallery();
            }
            else if (panelName == "Edit Menu")
            {
                DestroyGestureGallery();
            }

        }
    }
}
