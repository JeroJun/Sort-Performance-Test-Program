using UnityEngine;

public class UI : Singleton<UI>
{
    [Header("Reference")]
    public GameObject window;

    [Header("List UI")]
    public Transform parentHandler;
    public GameObject buttonPrefab;

    private bool isActive = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isActive = !isActive;
            ShowUI(isActive);
        }
    }

    private void ShowUI(bool state)
    {
        if (state)
        {
            if (window.activeSelf == false) window.SetActive(true);

            // Play animation
            Utility.instance.PlayAnimation(window, "Fade_Out");

            // Update graph
            Sort.instance.UpdateGraph();

            // Destory all child
            foreach (Transform child in parentHandler)
            {
                Destroy(child.gameObject);
            }

            // Instantiate child
            for (int i = 0; i < Sort.instance.dataSets.Count; i++)
            {
                Button button = Instantiate(buttonPrefab, parentHandler).GetComponent<Button>();
                button.Initialize(i, Sort.instance.dataSets[i].name);
            }
        }
        else
        {
            // Play animation
            Utility.instance.PlayAnimation(window, "Fade_In");
        }
    }

    public void StartSort()
    {
        Sort.instance.StartSort();
    }
    public void LoadText(int index)
    {
        Sort.instance.SetIndex(index);
    }
}
