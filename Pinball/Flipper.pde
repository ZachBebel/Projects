// Zach Bebel
// IMD Project #1

// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// A rectangular box
class Flipper {

  // We need to keep track of a Body and a width and height
  Body body;
  boolean lock;
  Vec2 pos;

  // Constructor
  Flipper(float x, float y, boolean lock_) {
    // Add the box to the box2d world
    lock = lock_;
    pos = new Vec2(x, y);
    makeBody(pos);
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
    if (pos.y > height) {
      killBody();
      return true;
    }
    return false;
  }

  // Drawing the box
  void display() {
    // We look at each body and get its screen position
    pos = box2d.getBodyPixelCoord(body);
    // Get its angle of rotation
    float a = body.getAngle();

    Fixture f = body.getFixtureList();
    PolygonShape ps = (PolygonShape) f.getShape();


    rectMode(CENTER);
    pushMatrix();
    translate(pos.x, pos.y);
    rotate(-a);
    fill(127);
    stroke(0);
    strokeWeight(2);
    beginShape();
    //println(vertices.length);
    // For every vertex, convert to pixel vector
    for (int i = 0; i < ps.getVertexCount(); i++) {
      Vec2 v = box2d.vectorWorldToPixels(ps.getVertex(i));
      vertex(v.x, v.y);
    }
    endShape(CLOSE);
    popMatrix();
  }

  // This function adds the rectangle to the box2d world
  void makeBody(Vec2 center) {

    float size = 6;
    int count = 0;
    Vec2[] vertices = new Vec2[8];
    for (float i=PI/2; i<=3*PI/2; i+= PI/5)
    {
      vertices[count] = box2d.vectorPixelsToWorld(new Vec2(1.5 * size * cos(i), 1.5 * size * sin(i)));
      count++;
    }
    vertices[6] = box2d.vectorPixelsToWorld(new Vec2(10*size, -0.5*size));
    vertices[7] = box2d.vectorPixelsToWorld(new Vec2(10*size, 0.5*size));

    //    vertices[0] = box2d.vectorPixelsToWorld(new Vec2(direction * -size, -size));
    //    vertices[1] = box2d.vectorPixelsToWorld(new Vec2(direction * 10*size, -size));
    //    vertices[2] = box2d.vectorPixelsToWorld(new Vec2(direction * 10*size, size));
    //    vertices[3] = box2d.vectorPixelsToWorld(new Vec2(direction * -size, size));

    // Define a polygon (this is what we use for a rectangle)
    PolygonShape ps = new PolygonShape();
    ps.set(vertices, vertices.length);

    // Define the body and make it from the shape
    BodyDef bd = new BodyDef();
    if (lock) bd.type = BodyType.STATIC;
    else bd.type = BodyType.DYNAMIC;
    bd.position.set(box2d.coordPixelsToWorld(center));
    body = box2d.createBody(bd);
    
    FixtureDef fd = new FixtureDef();
    fd.shape = ps;
    
    fd.density = 2;
    fd.friction = 0.01;
    fd.restitution = 0.1;

    body.createFixture(fd);
  }

  void rotateBody(float angle)
  {
    body.setTransform(body.getPosition(), angle);
  }
}

