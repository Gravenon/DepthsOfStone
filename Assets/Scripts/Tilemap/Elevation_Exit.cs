using UnityEngine;
using UnityEngine.Tilemaps;

public class Elevation_Exit : MonoBehaviour
{
    public TilemapRenderer[] elevationTilemaps; 
    public Collider2D[] borderColliders; 
    public Collider2D[] offBorderColliders; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            foreach (var tilemap in elevationTilemaps)
            {
                tilemap.sortingOrder = 10; 
            }

            foreach (Collider2D border in borderColliders)
            {
                border.enabled = true; 
            }

            foreach (Collider2D offBorder in offBorderColliders)
            {
                offBorder.enabled = false; 
            }
        }
    }
}
