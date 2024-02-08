using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    //public List<int[]> ListofGridsWithAmbient = new List<int[]>();
    public GameObject cTTS;
    public Vector3 AmbientVect;

    public CSVReaderFinal CVSR;

    public GridNavigator gridNavigator;

    public GridManager gridManager;

    public int myRow;
    public int myCol;

    //public float max_distance;

    // Start is called before the first frame update
    void Start()
    {
        CVSR = GameObject.Find("MainFrame and Controller").GetComponent<CSVReaderFinal>();
        gridNavigator = GameObject.Find("MainFrame and Controller").GetComponent<GridNavigator>();
        gridManager = GameObject.Find("Grid Manager").GetComponent<GridManager>();
        
        if (this.gameObject.GetComponent<AudioSource>().clip == null)
        {
           // this.gameObject.GetComponent<AudioSource>().enabled = false;
        }
        else
        {
            this.gameObject.GetComponent<AudioSource>().enabled = true;
            this.gameObject.GetComponent<AudioSource>().Play();
        }
        
        UpdateSoundPosition(0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        //do we need to update anything every frame?
        //UpdateSoundPosition();
    }

    public void UpdateSoundPosition(int user_position_row, int user_position_col) //this should be an attribute to a game object
    { 
        //gridManager.grid[CVSR.ListofGridsWithAmbient[0][0], CVSR.ListofGridsWithAmbient[0][1]].GetComponent<AudioSource>().clip = trainSound;
        if (this.gameObject.GetComponent<AudioSource>().clip != null)
        {
            float max_distance = 5;
            //hardcoded for now
            //float ambient_row = ; //CVSR.ListofGridsWithAmbient[0].transform.position.x
            //float ambient_col = 8; //CVSR.ListofGridsWithAmbient[0].transform.position.y)

            //
            //Debug.Log("gridManager.grid[x] " + CVSR.ListofGridsWithAmbient[0].transform.position.x);
            //Debug.Log("gridManager.grid[y] " + CVSR.ListofGridsWithAmbient[0].transform.position.y);

            Vector2 pointA = new Vector2((float)user_position_row, (float)user_position_col);
            //Debug.Log("ambient coords y: " + CVSR.ListofGridsWithAmbient[0][0]);
           // Debug.Log("ambient coords x: " + CVSR.ListofGridsWithAmbient[0][1]);

            Vector2 pointB = new Vector2((float)myRow, (float)myCol);

            // Calculate distance
            float distance = CalculateDistance(pointA, pointB);
            //Debug.Log("Distance: " + distance);
            //if distance is <=5 volume=1 to 0.5 , else vol =0
            if(distance <= max_distance)
            {
                float volume = 1 - ((1/max_distance) * distance); //slope formula for setting volume, slope=volume/distance=1/5
                // Calculate angle
                float angle = CalculateAngle(pointA, pointB);
                //Debug.Log("Angle: " + angle);

                // Map angle to output
                float panValue = MapAngleToPanning(angle);
                //Debug.Log("Mapped Value: " + panValue);

                this.gameObject.GetComponent<AudioSource>().volume = volume;
                this.gameObject.GetComponent<AudioSource>().panStereo = panValue;
                //this.gameObject.GetComponent<AudioSource>().Play();
            }
            else
            {
                this.gameObject.GetComponent<AudioSource>().volume = 0;
            }

            
            //need to get source coordinates from list
            //double angle = CalculateAngle(0, 0, 3, -5);
            //Console.WriteLine("Angle: " + angle);
            //double panvalue = MapAngleToPanning(angle);
            //Console.WriteLine("Angle: " + angle);


            //this.gameObject.GetComponent<AudioSource>().panStereo = 0.5;


            //if 5 blocks away, vol=0
            //do for each element in the list:
            /*int ambient_sound_row=CVSR.ListofGridsWithAmbient[0][0];
            int ambient_sound_col=CVSR.ListofGridsWithAmbient[0][1];
            int row_dir= ambient_sound_row - user_position_row;
            int col_dir= ambient_sound_col- user_position_col;
            AmbientVect= new Vector3 (col_dir,0, row_dir);


            cTTS.transform.position= parentObject.position +  AmbientVect  * soundDistance;
            gridManager.speakAroundMe(ambient_sound_row, ambient_sound_col, cTTS.transform*/

            //call this every time up down left right
            //everytime person moves
            //calculate how far the person is with respect to the sound
            //magnitude plus direction
            //what happens when the person is 20 units vs 5
            //set audiosource */
        }
    }

    float CalculateDistance(Vector2 pointA, Vector2 pointB)
    {
        // Calculate the magnitude distance between two points
        return Vector2.Distance(pointA, pointB);
    }

    float CalculateAngle(Vector2 pointA, Vector2 pointB)
    {
        // Calculate the angle in radians using Atan2
        float angleRadians = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x);

        // Convert radians to degrees
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        // Ensure the angle is positive (0 to 360 degrees)
        angleDegrees = (angleDegrees + 360) % 360;

        return angleDegrees;
    }

    float MapAngleToPanning(float angle)
    {
        float panvalue;
        // Perform linear mapping
        if (angle >= 0 && angle <= 90)
        {
            panvalue = (90 - angle) / 90;
        }
        else if (angle > 90 && angle <= 180)
        {
            panvalue = ((180 - angle) * -1) / 90;
        }
        else if (angle > 180 && angle <= 270)
        {
            panvalue = ((270 - angle) * -1) / 90;
        }
        else if (angle > 270 && angle <= 360)
        {
            panvalue = (360 - angle) / 90;
        }
        else
        {
            panvalue = 0; // For angles outside the specified range
        }
        return panvalue;
    }
}
