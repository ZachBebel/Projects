// Zach Bebel
// IMD Project #1

// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// A rectangular box

class Box {

  // We need to keep track of a Body and a width and height
  Body body;
  float w;
  float h;
  float angle;
  Vec2 pos;
  PImage metal;
  boolean display;
  boolean lock;
  Vec2 center;

  // Constructor
  Box(float x, float y, float w_, float h_, float angle_, boolean lock_) {
    metal = loadImage("metal.jpg");
    w = w_;
    h = h_;
    lock = lock_;
    angle = angle_;
    center = new Vec2(x, y);
    makeBody();
  }

  void makeBody() {
    display = true;

    // Define and create the body
    BodyDef bd = new BodyDef();
    bd.position.set(box2d.coordPixelsToWorld(center));
    if (lock) bd.type = BodyType.STATIC;
    else bd.type = BodyType.DYNAMIC;

    body = box2d.createBody(bd);

    // Define the shape -- a  (this is what we use for a rectangle)
    PolygonShape sd = new PolygonShape();
    float box2dW = box2d.scalarPixelsToWorld(w/2);
    float box2dH = box2d.scalarPixelsToWorld(h/2);
    sd.setAsBox(box2dW, box2dH);

    // Define a fixture
    FixtureDef fd = new FixtureDef();
    fd.shape = sd;
    // Parameters that affect physics
    fd.density = 1;
    fd.friction = 0.3;
    fd.restitution = 0.05;

    body.createFixture(fd);
    body.setFixedRotation(true);
    body.setTransform(body.getPosition(), angle);
  }

  // This function removes the particle from the box2d world
  void killBody() {
    box2d.destroyBody(body);
    display = false;
  }

  // Drawing the box
  void display() {
    // We look at each body and get its screen position
    pos = box2d.getBodyPixelCoord(body);
    // Get its angle of rotation
    float a = body.getAngle();

    rectMode(PConstants.CENTER);
    pushMatrix();
    translate(pos.x, pos.y);
    rotate(-a);
    fill(127);
    stroke(0);
    strokeWeight(2);
    //rect(0, 0, w, h);
    rotate(PI/2);
    if (display) image(metal, -h/2, -w/2, h, w);
    popMatrix();
  }
}

