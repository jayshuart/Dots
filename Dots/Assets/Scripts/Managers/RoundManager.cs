using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Player[] players;
    [SerializeField] private GameObject dotPrefab;

    [Header("Grid Settings")]
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject gridRowPrefab;
    [SerializeField] private int gridRows = 10;
    [SerializeField] private int gridColumns = 10;

    //private props
    private Dot[] _dots;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buildGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void buildGrid(){
        for(int x = 0; x < gridRows; x++){
            GameObject row = Instantiate(gridRowPrefab, gridParent.transform);
            for(int y = 0; y < gridColumns; y++){
                Dot dot = Instantiate(dotPrefab, row.transform).GetComponent<Dot>();
            }
        }
    }
}
