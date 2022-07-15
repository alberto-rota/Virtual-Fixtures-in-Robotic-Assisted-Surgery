// Copyright (c) 2022 Alberto Rota
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
// ,RequireComponent(typeof(FixedJoint))]
public class IsPinchable : MonoBehaviour
{
    Material materialpinched;
    Material materialown;
    Material materialpinchable;
    Vector3 psm;
    Vector3 p;
    public float d;
    public float targetRadius; 
    public Transform pincherObject;
    public bool addGravity = false;

    public bool pinched = false;
    bool pinchable = false;
    public bool graphics = false;

    void Start()
    {
        materialpinched = Resources.Load<Material>("Materials/Pinched");
        materialown = gameObject.GetComponent<Renderer>().sharedMaterial;
        materialpinchable = Resources.Load<Material>("Materials/Pinchable");
        //Disable the collider
        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().mass = 0;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        pincherObject = GameObject.Find(Global.tooltip_path).transform;
    }

    void Update()
    {
        // DEPRECATED: USE ONLY IF CONTROLLING THE ROBOT WITH THE KEYBOARD
        // bool pinchingAction = Input.GetKeyDown(KeyCode.Space);
        // bool releasingAction = Input.GetKeyUp(KeyCode.Space);

        bool pinchingAction  = false;
        if (GameObject.Find("ROBOT").GetComponent<RosSharp.RosBridgeClient.JointJawSubscriber>().jawPosition <  0.2f) {
            pinchingAction = true;
        }


        targetRadius = gameObject.GetComponent<SphereCollider>().radius*gameObject.transform.lossyScale.x;
        psm = pincherObject.position;
        p = gameObject.transform.position;
        d = Vector3.Distance(p,psm);
        if (d < targetRadius) {
            pinchable = true;
            if (pinchingAction) {
                pinched = true;
            } else {
                pinched = false;
            }
        } else pinchable = false;
       
        if (graphics) {
            Global.Arrow(psm,p,Color.yellow);
        }

        if (pinched) {
            if (gameObject.GetComponent<Renderer>().material != materialpinched) {
                gameObject.GetComponent<Renderer>().material = materialpinched;
            }
            if (gameObject.GetComponent<FixedJoint>() == null) {
                gameObject.AddComponent<FixedJoint>();
                gameObject.GetComponent<FixedJoint>().connectedBody = pincherObject.gameObject.GetComponent<Rigidbody>();
            }
        } else {
            if (gameObject.GetComponent<FixedJoint>() != null) {
                Destroy(gameObject.GetComponent<FixedJoint>());
            }
            // gameObject.GetComponent<FixedJoint>().connectedBody = null;
            if (addGravity) {
                gameObject.GetComponent<Rigidbody>().useGravity = true;
            }else{
                gameObject.GetComponent<Rigidbody>().useGravity = false;
            }

        }
        if (pinchable && !pinched) {
            gameObject.GetComponent<Renderer>().material = materialpinchable;
        }
        if (!pinchable && !pinched) {
            
            if (gameObject.GetComponent<Renderer>().material != materialown) {
                gameObject.GetComponent<Renderer>().material = materialown;
            }
        }

        // DEPRECATED: USE ONLY IF CONTROLLING THE ROBOT WITH THE KEYBOARD
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     GameObject.Find(pinch2).transform.Rotate(Vector3.up,-10);            
        //     GameObject.Find(pinch1).transform.Rotate(Vector3.up,+10);
        // }else if (Input.GetKeyUp(KeyCode.Space)) {
        //     GameObject.Find(pinch2).transform.Rotate(Vector3.up,10);            
        //     GameObject.Find(pinch1).transform.Rotate(Vector3.up,-10);
        // }
    }
}
