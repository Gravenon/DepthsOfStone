using UnityEngine;
using UnityEngine.Tilemaps;

public class Elevation_Entry : MonoBehaviour
{
    public TilemapRenderer[] elevationTilemaps; // Array of TilemapRenderers for different elevation levels
    public Collider2D[] borderColliders; // Collider that defines the entry point to the elevated area
    public Collider2D[] offBorderColliders; // Colliders that define the exit points from the elevated area

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            foreach (var tilemap in elevationTilemaps)
            {
                tilemap.sortingOrder = 4; 
            }

            foreach (Collider2D border in borderColliders)
            {
                border.enabled = true; // Enable the border collider to prevent exit
            }

            foreach (Collider2D offBorder in offBorderColliders)
            {
                offBorder.enabled = false; // Disable the off-border colliders to allow entry
            }
        }
    }
}
