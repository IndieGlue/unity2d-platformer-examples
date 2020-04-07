# Platformer Collisions

- [Colliding with layers](#colliding-with-layers)
- [Examples](#examples)
  - [Moving a unit](#moving-a-unit)
  - [Check if unit is grounded](#check-if-unit-is-grounded)
---
Collisions are probably one of the first important tasks to take on, and an unstable collision detection will become a pain in the ass.
We aren't using rigidbody to determine our collisions, so we'll need to find a easy way to get back collision data.

*All code are copy paste + hand written, create an issue and link this wiki if there are any typos*

## Colliding with layers

The class responsible for detecting and returning layer collisions. For this tutorial we will only be looking at box collision, but it shouldn't be difficult to implement other collision types. This class has functions that will check collision in a certain direction (vertical or horizontal) with a certain distance.

We are using a BoxCollider2D to determine the collider position and size. For example, a unit could have multiple collision boxes (feet, body, hands, etc.), we are passing through a BoxCollider2D to determine the collider position and size. But if you want to, you could even change this to use an object's transform to get the collider size.

```c#
public class ObjectLayerCollision
{
    public static GetHorizontalCollisionData(LayerMask layerMask, BoxCollider2D box, float distance)
    {
        // Get the current center of the box
        Vector2 center = new Vector2(box.bounds.center.x, box.bounds.center.y);
        // Get the current size of the box
        Vector2 size = new Vector2(box.bounds.size.x, box.bounds.size.y);
        // Where the box will be after adding the distance (only adding on the x-axis for horizontal)
        Vector2 nextPoint = center + new Vector2(distance, 0);

        // Will the new position of the box collide with anything in that LayerMask
        Collider2D collider = Physics2D.OverlapBox(
            nextPoint,
            size,
            0, // angle
            layerMask
        );

        if(collider) {
            // Create collision data (see below)
            return new CollisionData(box, collider);
        }

        return null;
    }
    
    public static GetVerticalCollisionData(LayerMask layerMask, BoxCollider2D box, float distance)
    {
        // Get the current center of the box
        Vector2 center = new Vector2(box.bounds.center.x, box.bounds.center.y);
        // Get the current size of the box
        Vector2 size = new Vector2(box.bounds.size.x, box.bounds.size.y);
        // Where the box will be after adding the distance (only adding on the y-axis for vertical)
        Vector2 nextPoint = center + new Vector2(0, distance);

        // Will the new position of the box collide with anything in that LayerMask
        Collider2D collider = Physics2D.OverlapBox(
            nextPoint,
            size,
            0, // angle
            layerMask
        );

        if(collider) {
            // Create collision data (see below)
            return new CollisionData(box, collider);
        }

        return null;
    }
}
```

The class that holds collision information. Add as many helper function here as you want, we only added the `Distance()` function for you.

```c#
public class CollisionData
{
    public Collider2D collider;
    public Collider2D box;

    public CollisionData(Collider2D box, Collider2D collider)
    {
        this.collider = collider;
        this.box = box;
    }

    public float Distance()
    {
        float distance = 0f;

        if(collider) {
            // Use unity's distance between colliders function
            ColliderDistance2D colliderDistance = collider.Distance(box);
            
            // We are rounding it, because unity will still find minimal distance, and we don't want to be that specific
            return Mathf.Round(colliderDistance.distance * 100.0f) * 0.001f;
        }

        return distance;
    }
}
```

## Examples

### Moving a unit
```c#
// The box collider of the unit
public BoxCollider2D bodyCollider;

public void MoveUnit(Vector2 moveVelocity, LayerMask layerMask)
{
    // NOTE: Make sure your moveVelocity x and y is multiplied with Time.deltaTime for smooth movement

    // Get horizontal speed
    float hspeed = moveVelocity.x;

    // We have a BoxCollider2D component called "bodyCollider" set on the unit

    // Check if there will be any collision with bodyCollider when we move the unit horizontally with the speed given
    CollisionData hCollider = ObjectLayerCollision.GetHorizontalCollisionData(layerMask, bodyCollider, hspeed);
    
    // When the player is moving and the player is going to collide after applying the speed
    if(Mathf.Abs(hspeed) > 0 && hCollider != null) {
        // Get the distance between the unit and whatever it is going to collide with
        float distanceToCollider = hCollider.Distance();

        // Let the unit move that distance instead of it's original speed to avoid moving past the collider (E.g. wall)
        hspeed = distanceToCollider * Mathf.Sign(moveVelocity.x);
    }
    
    // Get vertical speed
    float vspeed = moveVelocity.y;
    
    // Check if there will be any collision with bodyCollider when we move the unit vertically with the speed given
    CollisionData vCollider = ObjectLayerCollision.GetVerticalCollisionData(layerMask, bodyCollider, vspeed);
    
    // When the player is moving and the player is going to collide after applying the speed
    if(Mathf.Abs(vspeed) > 0 && vCollider != null) {
        // Let the unit move the distance between colliderse instead of it's original speed to avoid moving past the collider (E.g. wall)
        vspeed = vCollider.Distance() * Mathf.Sign(moveVelocity.y);
    }

    // Finally move the unit
    transform.position = (Vector2) transform.position + new Vector2(hspeed, vspeed);
}
```

### Check if unit is grounded
```C#
// The layers seen as "ground", E.g. "platform" layer, etc.
public LayerMask groundLayers;

// The box collider of the unit
public BoxCollider2D bodyCollider;

public bool IsGrounded() 
{
    // Is unit moving down
    if(velocity.y > 0) {
        return false;
    }

    // Is unit colliding with ground layer
    CollisionData collisionData = ObjectLayerCollision.GetVerticalCollisionData(groundLayers, bodyCollider, -0.1f);
    
    // If there is collision data then the unit is grounded
    return collisionData != null;
}
```
