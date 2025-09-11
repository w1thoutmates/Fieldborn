using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum CellType { Damage, Heal, Shield }

public class GridSquare : MonoBehaviour
{
    public Image hoverImage;
    public Image activeImage;

    public CellType cellType;

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
        if (color == new Color(229f / 255f, 115f / 255f, 115f / 255f)) // red
            cellType = CellType.Damage;
        else if (color == new Color(129f / 255f, 199f / 255f, 132f / 255f)) // green
            cellType = CellType.Heal;
        else if (color == new Color(100f / 255f, 181f / 255f, 246f / 255f)) // blue
            cellType = CellType.Shield;

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

    public void ShakeIncreasing(float duration, float start_strength, float end_strength)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOShakePosition(duration * 0.5f, start_strength));

        seq.Append(transform.DOShakePosition(duration * 0.5f, end_strength));
    }

   
}
