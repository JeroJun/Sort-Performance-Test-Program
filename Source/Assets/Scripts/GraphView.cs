using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class GraphView : MonoBehaviour
{
    public enum SortType
    {
        Bubble,
        Insertion,
        Selection,
        Merge,
        Quick
    }

    [Header("Setting")]
    public SortType sortType;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private TextMeshPro infoText;
    public int size
    {
        get { return data.Count; }
    }

    [Header("List")]
    [SerializeField] private List<int> data;
    [SerializeField] private List<GameObject> line = new List<GameObject>();
    private List<SpriteRenderer> lineSr = new List<SpriteRenderer>();

    // Float
    private float delay = 0.00001f;

    // Vector
    private Vector2 delta = Vector2.zero;
    private Vector2 start = Vector2.zero;
    private Vector2 current = Vector2.zero;

    // Coroutines
    private Coroutine timer, bubble, insertion, selection, merge, quick;

    private void OnValidate()
    {
        UpdateGraph();
    }
    public void UpdateGraphView(List<int> list)
    {
        StopAllCoroutines();
        data = list.ToList();
        UpdateChild();
        UpdateGraph();
        UpdateValue();
    }

    private void UpdateChild()
    {
        foreach (Transform child in parent)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(child.gameObject);
            };
#endif
        }
        line.Clear();
    }
    private void UpdateGraph()
    {
        if (parent == null) parent = Instantiate(new GameObject("Parent"), transform).transform;
        if (infoText == null) infoText = GetComponentInChildren<TextMeshPro>();
        infoText.text = sortType.ToString() + " Sort";
        gameObject.name = sortType.ToString() + " Sort";

        int sizeDelta = size - line.Count;
        if (linePrefab == null || parent == null || sizeDelta == 0)
        {
            return;
        }
        else
        {
            SpriteRenderer sr = linePrefab.GetComponent<SpriteRenderer>();
            delta.x = (sr.sprite.rect.size.x / sr.sprite.pixelsPerUnit) / (1 / linePrefab.transform.localScale.x);
            start.x = -(delta.x * size / 2);
            current = start;
            if (sizeDelta > 0)
            {
                for (int i = 0; i < sizeDelta; i++)
                {
                    line.Add(Instantiate(linePrefab, parent));
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(sizeDelta); i++)
                {
                    GameObject targetLine = line[line.Count - 1];
                    line.RemoveAt(line.Count - 1);
                    if (Application.isPlaying)
                    {
                        Destroy(targetLine);
                    }
                    else
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.delayCall += () =>
                        {
                            DestroyImmediate(targetLine);
                        };
#endif
                    }
                }
            }
        }
        lineSr.Clear();
        for (int i = 0; i < line.Count; i++)
        {
            line[i].transform.localPosition = current;
            current.x += delta.x;
            lineSr.Add(line[i].GetComponent<SpriteRenderer>());
        }
    }
    private void UpdateValue()
    {
        if (linePrefab != null)
        {
            int max = data[0];
            for (int i = 0; i < size; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
            }
            Vector3 set = linePrefab.transform.localScale;
            for (int i = 0; i < size; i++)
            {
                set.y = (float)data[i] / (float)max;
                line[i].transform.localScale = set;
            }
        }
    }

    public void StartSort()
    {
        switch (sortType)
        {
            case SortType.Bubble:
                if (bubble != null) StopCoroutine(bubble);
                bubble = StartCoroutine(Bubble());
                break;
            case SortType.Insertion:
                if (insertion != null) StopCoroutine(insertion);
                insertion = StartCoroutine(Insertion());
                break;
            case SortType.Selection:
                if (selection != null) StopCoroutine(selection);
                selection = StartCoroutine(Selection());
                break;
            case SortType.Merge:
                if (merge != null) StopCoroutine(merge);
                merge = StartCoroutine(Merge());
                break;
            case SortType.Quick:
                if (quick != null) StopCoroutine(quick);
                quick = StartCoroutine(Quick());
                break;
            default:
                break;
        }
    }

    private IEnumerator Bubble()
    {
        yield return new WaitForSeconds(0.5f);
        Stopwatch sw = new Stopwatch();
        sw.Start();
        int hold = 0;
        for (int i = 0; i < data.Count - 1; i++)
        {
            for (int j = 0; j < data.Count - 1 - i; j++)
            {
                if (data[j] > data[j + 1])
                {
                    hold = data[j];
                    data[j] = data[j + 1];
                    data[j + 1] = hold;
                }
            }
        }

        sw.Stop();
        infoText.text = sortType.ToString() + " Sort\nTime: " + sw.Elapsed.TotalMilliseconds + "ms";

        RangeSet(0, size);
        foreach (SpriteRenderer sr in lineSr)
        {
            sr.color = Color.cyan;
            yield return new WaitForSeconds(delay);
        }
        yield break;
    }
    private IEnumerator Insertion()
    {
        yield return new WaitForSeconds(0.5f);
        Stopwatch sw = new Stopwatch();
        sw.Start();

        int index, hold, n = data.Count;
        for (int i = 1; i < n; i++)
        {
            hold = data[(index = i)];
            while (--index >= 0 && hold < data[index])
            {
                data[index + 1] = data[index];
                data[index] = hold;
            }
        }

        sw.Stop();
        infoText.text = sortType.ToString() + " Sort\nTime: " + sw.Elapsed.TotalMilliseconds + "ms";

        RangeSet(0, size);
        foreach (SpriteRenderer sr in lineSr)
        {
            sr.color = Color.cyan;
            yield return new WaitForSeconds(delay);
        }
        yield break;
    }
    private IEnumerator Selection()
    {
        yield return new WaitForSeconds(0.5f);
        Stopwatch sw = new Stopwatch();
        sw.Start();

        int min, hold, n = data.Count;
        for (int i = 0; i < n - 1; i++)
        {
            min = i;
            for (int j = i + 1; j < n; j++)
            {
                if (data[j] < data[min])
                {
                    min = j;
                }
            }
            hold = data[min];
            data[min] = data[i];
            data[i] = hold;
        }

        sw.Stop();
        infoText.text = sortType.ToString() + " Sort\nTime: " + sw.Elapsed.TotalMilliseconds + "ms";

        RangeSet(0, size);
        foreach (SpriteRenderer sr in lineSr)
        {
            sr.color = Color.cyan;
            yield return new WaitForSeconds(delay);
        }
        yield break;
    }
    private void MergeFragment(int[] data, int left, int mid, int right)
    {
        int i, j, k, l;
        i = left;
        j = mid + 1;
        k = left;
        while (i <= mid && j <= right)
        {
            if (data[i] <= data[j])
                this.data[k++] = data[i++];
            else
                this.data[k++] = data[j++];
        }
        if (i > mid)
        {
            for (l = j; l <= right; l++)
                this.data[k++] = data[l];
        }
        else
        {
            for (l = i; l <= mid; l++)
                this.data[k++] = data[l];
        }
        for (l = left; l <= right; l++)
        {
            data[l] = this.data[l];
        }
    }
    private void MergeSort(int[] data, int left, int right)
    {
        int mid;

        if (left < right)
        {
            mid = (left + right) / 2;
            MergeSort(data, left, mid);
            MergeSort(data, mid + 1, right);
            MergeFragment(data, left, mid, right);
        }
    }
    private IEnumerator Merge()
    {
        yield return new WaitForSeconds(0.5f);
        Stopwatch sw = new Stopwatch();
        sw.Start();
        MergeSort(data.ToArray(), 0, size - 1);
        sw.Stop();
        infoText.text = sortType.ToString() + " Sort\nTime: " + sw.Elapsed.TotalMilliseconds + "ms";

        RangeSet(0, size);
        foreach (SpriteRenderer sr in lineSr)
        {
            sr.color = Color.cyan;
            yield return new WaitForSeconds(delay);
        }
        yield break;
    }
    private IEnumerator Quick()
    {
        yield return new WaitForSeconds(0.5f);
        Stopwatch sw = new Stopwatch();
        sw.Start();
        data.Sort();
        sw.Stop();
        infoText.text = sortType.ToString() + " Sort\nTime: " + sw.Elapsed.TotalMilliseconds + "ms";

        RangeSet(0, size);
        foreach (SpriteRenderer sr in lineSr)
        {
            sr.color = Color.cyan;
            yield return new WaitForSeconds(delay);
        }
        yield break;
    }


    private void RangeSet(int start, int end)
    {
        if (linePrefab != null)
        {
            int max = data[0];
            for (int i = 0; i < size; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
            }
            Vector3 set = linePrefab.transform.localScale;
            for (int i = start; i < end; i++)
            {
                lineSr[i].color = Color.red;
                set.y = (float)data[i] / (float)max;
                line[i].transform.localScale = set;
            }
        }
    }
}
