using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hoverImage;
    public Image activeImage;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    [HideInInspector] public AudioSource audio_source;
    public AudioClip[] click_sounds;

    private void Awake()
    {
        audio_source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Selected = false;
        SquareOccupied = false;
    }
    
    public bool CanWeUseThisSquare()
    {
        return hoverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard(Color color)
    {
        ActivateSquare(color);
        if(click_sounds != null && click_sounds.Length > 0 )
        {
            int random_index = Random.Range(0, click_sounds.Length);
            audio_source.PlayOneShot(click_sounds[random_index]);
        }
    }

    public void ActivateSquare(Color color)
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        activeImage.color = color;
        Selected = true;
        SquareOccupied = true;
    }    

    public void Deactivate()
    {
        activeImage.gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        Selected = false;
        SquareOccupied = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            Selected = true;
            hoverImage.gameObject.SetActive(true);
            hoverImage.color = collision.GetComponentInParent<Shape>().ShapeColor;
            Color c = hoverImage.color;
            c.a = 180f / 225f;
            hoverImage.color = c;
        }
        else if(collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;

        if (SquareOccupied == false)
        {
            hoverImage.gameObject.SetActive(true);
            hoverImage.color = collision.GetComponentInParent<Shape>().ShapeColor;
            Color c = hoverImage.color;
            c.a = 180f / 225f;
            hoverImage.color = c;
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            Selected = false;
            hoverImage.gameObject.SetActive(false);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupied();
        }
    }
}
