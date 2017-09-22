//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

using UnityEditor;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MeshCombiner mb = (MeshCombiner)target;
        if (GUILayout.Button("Combine"))
        {
            mb.StartCombine();
        }
        if (GUILayout.Button("Clear Childs"))
        {
            while (mb.transform.childCount > 0)
            {
                DestroyImmediate(mb.transform.GetChild(0).gameObject);
            }
        }

        //if (GUILayout.Button("ReGenerateUV2"))
        //{
        //    while (mb.transform.childCount > 0)
        //    {
        //        mb.GenerateUV2();
        //    }
        //}
    }


}
