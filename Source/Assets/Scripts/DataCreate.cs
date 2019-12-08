using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DataCreate : MonoBehaviour
{
    public List<int> data = new List<int>();
    public Transform indexMarker;

    StringBuilder sb = new StringBuilder();
    [Multiline] public string text;

    public bool active = false;
    public int index;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            indexMarker.position = pos;
            index = (int)Input.mousePosition.y;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            active = !active;
            if (!active)
            {
                text = sb.ToString();
                sb.Length = 0;
            }
        }
        if (active)
        {
            sb.Append(Input.mousePosition.y - index + ",");
        }
    }
}
