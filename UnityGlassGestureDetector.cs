using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityGlassGestureDetector : MonoBehaviour
{

    public enum Gesture
    {
        TAP,
        SWIPE_FORWARD,
        SWIPE_BACKWARD,
        SWIPE_UP,
        SWIPE_DOWN,
        NULL
    }


    public UnityEvent OnSwipeDown = new UnityEvent();
    public UnityEvent OnSwipeUp = new UnityEvent();
    public UnityEvent OnSwipeBack = new UnityEvent();
    public UnityEvent OnSwipeForward = new UnityEvent();
    public UnityEvent OnTap = new UnityEvent();

    Vector3 mouseDeltaPosition = new Vector3();


    protected Gesture gesture = Gesture.NULL;


    //used purely for debugging purposes
    protected bool isTouched = false;



    protected static float SWIPE_DISTANCE_THRESHOLD_PX = 0.3f;
    protected static float SWIPE_VELOCITY_THRESHOLD_PX = 0.3f;
    protected static double TAN_60_DEGREES = Mathf.Tan(Mathf.Deg2Rad * 60);

  

    protected Vector2 touchStartPosition;

    [SerializeField]
    protected bool isFocusOfTaps = true;

    [SerializeField]
    protected bool isDebugging = false;


    float deltaX;
    float deltaY;
    double tan;
    float velocityX;
    float velocityY;

    DateTime touchStartTime;



    protected virtual void Update()
    {

        if (isFocusOfTaps == true)
        {

#if UNITY_ANDROID
            //If there is one touch then record where the touch begins and ends
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    touchStartPosition = Input.touches[0].position;
                    touchStartTime = DateTime.Now;
                }
                if (Input.touches[0].phase == TouchPhase.Ended)
                {

                    float timeDifference = (DateTime.Now - touchStartTime).Milliseconds;

                    //calculate the velocity of the touch to help determien whether it was a swipe or a tap
                    velocityX = Mathf.Abs(Input.touches[0].position.x - touchStartPosition.x) / timeDifference;
                    velocityY = Mathf.Abs(Input.touches[0].position.y - touchStartPosition.y) / timeDifference;


                    OnFling(touchStartPosition, Input.touches[0].position, velocityX, velocityY);



                }
                isTouched = true;



            }
            else
            {
                isTouched = false;
                gesture = Gesture.NULL;
            }

#endif

#if UNITY_EDITOR


            SWIPE_DISTANCE_THRESHOLD_PX = 0.3f;
            SWIPE_VELOCITY_THRESHOLD_PX = 0.3f;
            

            if (Input.GetMouseButtonDown(1))
            {
                touchStartPosition = Input.mousePosition;
                
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                Debug.Log("Swiped with mouse");
                //calculate the velocity of the touch to help determien whether it was a swipe or a tap
                float velocityX = (mouseDeltaPosition.x - Input.mousePosition.x) * Time.deltaTime;
                float velocityY = (mouseDeltaPosition.y - Input.mousePosition.y) * Time.deltaTime;

                Debug.Log("Velocity: " + velocityX + " : " + velocityY + " : " + Input.mousePosition);

                OnFling(touchStartPosition, Input.mousePosition, velocityX, velocityY);

            }
            if (Input.GetMouseButton(1))
            {
                mouseDeltaPosition = Input.mousePosition;
            }

#endif
        }


    }

    /// <summary>
    /// When the gesture pad is touched, this method is called in order to determine what type of gesture was made
    /// </summary>
    /// <param name="startPos">The position of the touch when the gesture started</param>
    /// <param name="endPos">The position of the touch when the gesture ended</param>
    /// <param name="velocityX">How fast the touch was moving along the y axis (Used to determine whether the touch is moving fast enough to be a swipe or if it was just a tap)</param>
    /// <param name="velocityY">How fast the touch was moving along the X axis (Used to determine whether the touch is moving fast enough to be a swipe or if it was just a tap)</param>
    /// <returns></returns>
    public virtual void OnFling(Vector2 startPos, Vector2 endPos, float velocityX, float velocityY)
    {
        deltaX = endPos.x - startPos.x;
        deltaY = endPos.y - startPos.y;
        tan = deltaX != 0 ? Mathf.Abs(deltaY / deltaX) : double.MaxValue;

        //if the angle of the touch is less than 60 then that means it is a vertical swipe or a tap
        if (tan > TAN_60_DEGREES)
        {
            //if the swipe was not moving fast enough then consider it a tap
            if (Mathf.Abs(deltaY) < SWIPE_DISTANCE_THRESHOLD_PX || velocityY < SWIPE_VELOCITY_THRESHOLD_PX)
            {
                
                gesture = Gesture.TAP;
                Tap();
               
            }
            else if (deltaY > 0)
            {
                gesture = Gesture.SWIPE_UP;
                SwipeUp();
                
            }
            else
            {
                gesture = Gesture.SWIPE_DOWN;
                SwipeDown();
                
            }
        }
        else
        {
            //if the swipe was not moving fast enough then consider it a tap
            if (Mathf.Abs(deltaX) < SWIPE_DISTANCE_THRESHOLD_PX || velocityX < SWIPE_VELOCITY_THRESHOLD_PX)
            {
                gesture = Gesture.TAP;
                Tap();
                
            }
            //determine the direction of the swipe to see if it goes forwards or backwards
            
            else if (deltaX < 0)
            {
                gesture = Gesture.SWIPE_FORWARD;
                SwipeForward();
                
            }
            else
            {
                gesture = Gesture.SWIPE_BACKWARD;
                SwipeBack();
                
            }
        }
    }



    /*  
          Swipe detection depends on the:
          - movement tan value,
          - movement distance,
          - movement velocity.
         
          To prevent unintentional SWIPE_DOWN and SWIPE_UP gestures, they are detected if movement
          angle is only between 60 and 120 degrees.
          Any other detected swipes, will be considered as SWIPE_FORWARD and SWIPE_BACKWARD, depends
          on deltaX value sign.
         
                    ______________________________________________________________
                   |                     \        UP         /                    |
                   |                       \               /                      |
                   |                         60         120                       |
                   |                           \       /                          |
                   |                             \   /                            |
                   |  BACKWARD  <-------  0  ------------  180  ------>  FORWARD  |
                   |                             /   \                            |
                   |                           /       \                          |
                   |                         60         120                       |
                   |                       /               \                      |
                   |                     /       DOWN        \                    |
                    --------------------------------------------------------------
         */

    #region -----------------Gestures-----------------
    protected virtual void SwipeUp()
    {
        OnSwipeUp.Invoke();
    }

    protected virtual void SwipeDown()
    {
        OnSwipeDown.Invoke();
    }

    protected virtual void SwipeForward()
    {
        OnSwipeForward.Invoke();
    }

    protected virtual void SwipeBack()
    {
        OnSwipeBack.Invoke();
    }

    protected virtual void Tap()
    {
        OnTap.Invoke();
    }
    #endregion



    public void SetFocus(bool set)
    {
        isFocusOfTaps = set;
    }



    #region -----------------Debugging-----------------
    //Used for debugging
    public virtual void OnGUI()
    {
        if (isDebugging == true)
        {
            GUI.skin.label.fontSize = Screen.width / 40;

                GUILayout.Label(gesture.ToString());
                if (isTouched == true)
                {
                    GUILayout.Label("X: " + Input.touches[0].position.x);
                    GUILayout.Label("Y: " + Input.touches[0].position.y);
                }
                
                GUILayout.Label("delta X: " + deltaX);
                GUILayout.Label("delta Y: " + deltaY);
                GUILayout.Label("velocity X: " + velocityX);
                GUILayout.Label("velocity Y: " + velocityY);
                GUILayout.Label("Tan 60  deg: " + TAN_60_DEGREES);
                GUILayout.Label("Tan: " + tan);
          
        }
    }
    #endregion
}

