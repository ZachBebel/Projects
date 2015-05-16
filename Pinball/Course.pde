// Zach Bebel
// IMD Project #1

class Course
{
  Body body;
  ArrayList<Vec2> surface;
  Vec2 center;
  float w;
  float h;
  float t;
  PImage floor;

  Course(float x, float y, float w_, float h_, float t_)
  {
    w = w_;
    h = h_;
    t = t_;
    center = new Vec2(x, y);
    makeBody();
    floor = loadImage("floor.png");
  }

  void makeBody()
  {
    surface = new ArrayList<Vec2>();
    float size = w/2;
    float theta = PI;
    float numSteps = 100;
    
    surface.add(new Vec2(center.x - w/6, center.y + h/2));
    surface.add(new Vec2(center.x - w/2, center.y + h/3));
    for (float i=0; i<=numSteps; i++)
    {
      surface.add(new Vec2(center.x + size*cos(theta), center.y - 0.27*h + size*sin(theta)));
      theta += PI/numSteps;
      if(i<numSteps/2) size -= w/(1.5*numSteps);
      else size += w/(1.5*numSteps);
    }
    surface.add(new Vec2(center.x + w/2, center.y + h/2));
    
    surface.add(new Vec2(center.x + w/2 - 40, center.y + h/2));
    surface.add(new Vec2(center.x + w/2 - 40, center.y + h/3));
    surface.add(new Vec2(center.x + w/6 - 40, center.y + h/2));
    
    surface.add(new Vec2(center.x + w/2 + 20, center.y + h/2));
    surface.add(new Vec2(center.x + w/2 + 20, center.y - h/2));
    surface.add(new Vec2(center.x - w/2 - 20, center.y - h/2));
    surface.add(new Vec2(center.x - w/2 - 20, center.y + h/2));
    surface.add(new Vec2(center.x - w/2, center.y + h/2));
    

    Vec2[] vertices = new Vec2[surface.size()];
    for (int i = 0; i < vertices.length; i++)
    {
      vertices[i] = box2d.coordPixelsToWorld(surface.get(i));
    }


    ChainShape chain = new ChainShape();
    chain.createChain(vertices, vertices.length);
    BodyDef bd = new BodyDef();
    bd.type = BodyType.STATIC;
    body = box2d.createBody(bd);
    
    FixtureDef fd = new FixtureDef();
    fd.shape = chain;

    fd.density = 200;
    fd.friction = 0.01;
    fd.restitution = 0.3;
    
    body.createFixture(fd);
  }

  void display()
  {
    image(floor, center.x - w/2, center.y - h/2, w, h);
    strokeWeight(t);
    stroke(0);
    fill(55); 
    beginShape();
    for (Vec2 v: surface) {
      vertex(v.x, v.y);
    }
    endShape();
  }
}

