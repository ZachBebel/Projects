// Zach Bebel
// IMD Project #1

// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// Series of Boxs connected with distance joints

class Launcher {

  Box b1;
  Box b2;
  Box leftRail;
  Box rightRail;
  float compressForce;
  Vec2 position;
  
  Vec2 pos1;
  Vec2 pos2;

  float len;
  float l;
  // Chain constructor
  Launcher(float x, float y, float w, float h, float l_) {
    
    position = new Vec2(x, y);

    len = 75;
    l = l_;

    b1 = new Box(x, y, w, h, 0, true);
    b2 = new Box(x, y-75, w, h, 0, false);

    leftRail = new Box(x - w/2 - 6, y + h/2 - l/2, 10, l, 0, true);
    rightRail = new Box(x + w/2 + 5, y + h/2 - l/2, 10, l, 0, true);

    DistanceJointDef djd = new DistanceJointDef();
    // Connection between previous particle and this one
    djd.bodyA = b1.body;
    djd.bodyB = b2.body;
    // Equilibrium length
    djd.length = box2d.scalarPixelsToWorld(len);

    // These properties affect how springy the joint is 
    djd.frequencyHz = 3;  // Try a value less than 5 (0 for no elasticity)
    djd.dampingRatio = 0.4; // Ranges between 0 and 1 (1 for no springiness)

    // Make the joint.  Note we aren't storing a reference to the joint ourselves anywhere!
    // We might need to someday, but for now it's ok
    DistanceJoint dj = (DistanceJoint) box2d.world.createJoint(djd);
  }

  void compress(boolean compressing)
  {
    if (compressing)
    {
      b2.body.applyForceToCenter(new Vec2(0, -compressForce));
      if (b2.pos.y+b2.h < b1.pos.y)
      {
        compressForce += 1000;
      }
    }
    else
    {
      compressForce = 1000;
    }
  }

  void display() {
    pos1 = box2d.getBodyPixelCoord(b1.body);
    pos2 = box2d.getBodyPixelCoord(b2.body);
    //line(pos1.x, pos1.y, pos2.x, pos2.y);

    leftRail.display();
    rightRail.display();
    
    b2.display();
    b2.display = false;
    
    stroke(0);
    strokeWeight(1);
    fill(2);
    rect(pos1.x, pos2.y + 1.5*b2.h, b2.w/2, 3*b2.h);
    fill(240, 7, 50);
    rect(pos1.x, pos2.y - b2.h/4, b2.w, b2.h/2, 10);
    b1.display();
  }
}

