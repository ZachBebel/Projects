// Zach Bebel
// IMD Project #1

class Button
{
  Vec2 pos;
  boolean pressed;
  boolean wasPressed;
  boolean active;
  float r;
  PImage bPressed;
  PImage bUnpressed;
  Box box;

  Button(float x, float y, float r_, Box box_)
  {
    pos = new Vec2(x, y);
    r = r_;
    box = box_;
    resetButton();
    bPressed = loadImage("pressed.png");
    bUnpressed = loadImage("unpressed.png");
  }
  
  void resetButton()
  {
    box.killBody();
    active = true;
    pressed = false;
    wasPressed = false;
  }

  void display()
  {
    if (pressed && active) 
    {
      image(bPressed, pos.x-r, pos.y-r, r*2, r*2);
    }
    else if (pressed == false && active)
    {
      image(bUnpressed, pos.x-r, pos.y-r, r*2, r*2);
    }
    if (box.body != null) box.display();
  }

  void checkCollisions(Ball ball)
  { 
    if (active)
    {
      Vec2 ballPos = box2d.getBodyPixelCoord(ball.body);

      if (dist(ballPos.x, ballPos.y, pos.x, pos.y) < r + ball.r)
      { 
        pressed = true;
        if (wasPressed == false)
        {
          if (box.display == true)
          {
            box.killBody();
          }
          else
          {
            box.makeBody();
          }
          wasPressed = true;
        }
      }
      else
      { 
        pressed = false;
        wasPressed = false;
      }
    }
  }
}

