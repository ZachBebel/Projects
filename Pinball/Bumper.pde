// Zach Bebel
// IMD Project #1

// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// A circular particle

class Bumper {

  // We need to keep track of a Body and a radius
  Body body;
  float r;
  PImage bumper;
  int pointValue;
  int forceMagnitude;
  int transparency = 0;
  Vec2 position;
  String fileName;

  Bumper(float x, float y, float r_, int forceMagnitude_, String fileName_, int pointValue_) {
    r = r_;
    forceMagnitude = forceMagnitude_;
    pointValue = pointValue_;
    fileName = fileName_;
    position = new Vec2(x, y);
    // This function puts the particle in the Box2d world
    makeBody(x, y, r);
    body.setUserData(this);
    bumper = loadImage(fileName);
  }
  
  void collisionResponse(Object obj) {
    if (obj.getClass()==Ball.class) {
      Ball ball = (Ball) obj;
      //ball.body.applyForceToCenter(ball.body.getLinearVelocity().mul(-1000000000));
      //ball.body.applyLinearImpulse(body.getPosition(), ball.body.getPosition().mul(forceMagnitude));
      //ball.body.applyLinearImpulse(ball.body.getLinearVelocity().mul(-forceMagnitude), body.getPosition());
      Vec2 dist = ball.body.getPosition().sub(body.getPosition());
      ball.body.applyLinearImpulse(dist.mul(-forceMagnitude), body.getPosition());
      transparency = 200;
    }
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
    fill(255, 255, 255, transparency);
    noStroke();
    if(fileName == "bumper3.png") image(bumper, -r, -1.4*r, r*2, 1.4*r*2);
    else image(bumper, -r, -r, r*2, r*2);
    ellipse(0, 0, r*2, r*2);
    popMatrix();
    
    if(transparency > 0) transparency -= 10;
  }

  // Here's our function that adds the particle to the Box2D world
  void makeBody(float x, float y, float r) {
    // Define a body
    BodyDef bd = new BodyDef();
    
    // Set its position
    bd.position = box2d.coordPixelsToWorld(x, y);
    bd.type = BodyType.STATIC;

    body = box2d.world.createBody(bd);

    // Make the body's shape a circle
    CircleShape cs = new CircleShape();
    cs.m_radius = box2d.scalarPixelsToWorld(r);

    FixtureDef fd = new FixtureDef();
    fd.shape = cs;

    fd.density = 2.0;
    fd.friction = 0.01;
    fd.restitution = 0.3; // Restitution is bounciness

    body.createFixture(fd);
  }
}

