using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Dot : MonoBehaviour
{
    //Properties
    private Player _owner;
    private Button _dotBtn;

    public bool Owned{
        get { return _owner != null;}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _dotBtn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwner(Player owner){
        _owner = owner;
        _dotBtn.image.color = _owner.color;
        _dotBtn.interactable = false;
    }
}
