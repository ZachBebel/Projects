// Zach Bebel
// IMD Project #1

// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// A circular particle

class Ball {

  // We need to keep track of a Body and a radius
  Body body;
  float r;
  PImage pinball;

  Ball(float x, float y, float r_) {
    r = r_;
    // This function puts the particle in the Box2d world
    makeBody(x, y, r);
    body.setUserData(this);
    pinball = loadImage("pinball.png");
  }

  // This function removes the particle from the box2d world
  void killBody() {
    box2d.destroyBody(body);
  }

  // Is the particle ready for deletion?
  boolean done() {
    // Let's find the screen position of the particle
    Vec2 pos = box2d.getBodyPixelCoord(body);
    // Is it off the bottom of the screen?
    if (pos.y > height+r*2) {
      killBody();
      return true;
    }
    return false;
  }

  // 
  void display() {
    // We look at each body and get its screen position
    Vec2 pos = box2d.getBodyPixelCoord(body);
    // Get its angle of rotation
    float a = body.getAngle();
    pushMatrix();
    translate(pos.x, pos.y);

    rotate(-a);
    fill(127);
    noStroke();
    //ellipse(0, 0, r*2, r*2);
    image(pinball, -r, -r, r*2, r*2);
    popMatrix();
  }

  // Here's our function that adds the particle to the Box2D world
  void makeBody(float x, float y, float r) {
    // Define a body
    BodyDef bd = new BodyDef();
    bd.bullet = true;

    // Set its position
    bd.position = box2d.coordPixelsToWorld(x, y);
    bd.type = BodyType.DYNAMIC;

    body = box2d.world.createBody(bd);

    // Make the body's shape a circle
    CircleShape cs = new CircleShape();
    cs.m_radius = box2d.scalarPixelsToWorld(r);

    FixtureDef fd = new FixtureDef();
    fd.shape = cs;

    fd.density = 2.0;
    fd.friction = 0.01;
    fd.restitution = 0; // Restitution is bounciness

    body.createFixture(fd);
    body.setFixedRotation(true);
  }
}

