// Zach Bebel
// IMD Project #1

// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// Example demonstrating revolute joint

import pbox2d.*;
import org.jbox2d.common.*;
import org.jbox2d.dynamics.joints.*;
import org.jbox2d.collision.shapes.*;
import org.jbox2d.collision.shapes.Shape;
import org.jbox2d.common.*;
import org.jbox2d.dynamics.*;
import org.jbox2d.dynamics.contacts.*;

// A reference to our box2d world
PBox2D box2d;

String gameState;

Course course;
Scoreboard scoreboard;

// An object to describe a Flipper (two bodies and one joint)
FlipperAssembly leftFlipper;
FlipperAssembly rightFlipper;

Launcher launcher;
ArrayList<Box> boundaries;
Button launcherButton;
Button leftCourseButton;
Button rightCourseButton;

// An ArrayList of balls that will fall on the surface
ArrayList<Ball> balls;
ArrayList<Bumper> bumpers;

void setup() {
  size(600, 600);
  newGame();
}

void newGame()
{
  // Initialize box2d physics and create the world
  box2d = new PBox2D(this);
  box2d.createWorld();

  gameState = "play";

  course = new Course(width/2, height/2, 500, 600, 5);
  scoreboard = new Scoreboard(0, 3);

  launcher = new Launcher(course.center.x + course.w/2 - 20, course.center.y + course.h/2 - 10, 30, 30, 0.78*course.h);
  launcherButton = new Button(launcher.position.x - launcher.b1.w, launcher.position.y - launcher.l - launcher.b1.h, launcher.b1.w/2, 
  new Box(launcher.position.x - 10, launcher.position.y - launcher.l, launcher.b1.w, 10, PI/4, true));

  leftFlipper = new FlipperAssembly(course.center.x - course.w/8 - launcher.b1.w/2, course.center.y + course.h/3, 20, -PI/4, PI/4);
  rightFlipper = new FlipperAssembly(course.center.x + course.w/8 - launcher.b1.w/2, course.center.y + course.h/3, -20, -5*PI/4, -3*PI/4);

  // Boudaries
  boundaries = new ArrayList<Box>();
  boundaries.add(new Box(course.center.x - leftFlipper.flipper.pos.x*0.65, course.center.y + course.h*0.28, 10, course.h/5, 3*PI/8, true));
  boundaries.add(new Box(course.center.x + rightFlipper.flipper.pos.x*0.33, course.center.y + course.h*0.28, 10, course.h/5, -3*PI/8, true));
  boundaries.add(new Box(course.center.x - leftFlipper.flipper.pos.x*0.88, course.center.y + course.h*0.15, 10, course.h/5, 0, true));
  boundaries.add(new Box(course.center.x + rightFlipper.flipper.pos.x*0.48, course.center.y + course.h*0.15, 10, course.h/5, 0, true));

  // Create the empty list
  balls = new ArrayList<Ball>();

  bumpers = new ArrayList<Bumper>();
  bumpers.add(new Bumper(course.center.x - course.w/8 - launcher.b1.w/2, course.center.y - course.h/6, 35, 200, "bumper1.png", 5));
  bumpers.add(new Bumper(course.center.x + course.w/8 - launcher.b1.w/2, course.center.y - course.h/6, 35, 200, "bumper1.png", 5));

  bumpers.add(new Bumper(course.center.x - course.w/4, course.center.y - course.h/3, 20, 300, "bumper3.png", 20));
  bumpers.add(new Bumper(course.center.x + course.w/4, course.center.y - course.h/3, 20, 300, "bumper3.png", 20));

  bumpers.add(new Bumper(course.center.x- launcher.b1.w/2, course.center.y, 20, 300, "bumper2.png", 5));
  bumpers.add(new Bumper(course.center.x + course.w*0.38 - launcher.b1.w/2, course.center.y - course.h/8, 30, 100, "bumper1.png", 8));
  bumpers.add(new Bumper(course.center.x - course.w*0.41 - launcher.b1.w/2, course.center.y - course.h/8, 30, 100, "bumper1.png", 8));

  leftCourseButton = new Button(bumpers.get(4).position.x - course.w*0.3, bumpers.get(4).position.y + course.h/8, 15, 
  new Box(bumpers.get(4).position.x - course.w/4, bumpers.get(4).position.y, course.w/4, 10, PI/8, true));
  rightCourseButton = new Button(bumpers.get(4).position.x + course.w*0.3, bumpers.get(4).position.y + course.h/8, 15, 
  new Box(bumpers.get(4).position.x + course.w/4, bumpers.get(4).position.y, course.w/4, 10, -PI/8, true));
  //leftCourseButton.box.makeBody();
  //rightCourseButton.box.makeBody();

  // ---------- set up for collisions -----------//
  box2d.listenForCollisions();
}



void beginContact(Contact cp) {
  // Get both shapes
  Fixture f1 = cp.getFixtureA();
  Fixture f2 = cp.getFixtureB();

  // Get both bodies
  Body b1 = f1.getBody();
  Body b2 = f2.getBody();

  // Get our objects that reference these bodies
  Object obj1 = b1.getUserData();
  Object obj2 = b2.getUserData();

  if (obj1 instanceof Bumper && obj2.getClass() == Ball.class) {
    Bumper targ = (Bumper) obj1;
    Ball ball = (Ball) obj2;
    targ.collisionResponse(ball);
    scoreboard.score += targ.pointValue;
    launcherButton.active = false;
  }
}

void endContact(Contact cp) {
  //println("end contact");
}




void draw() {
  background(0);

  if (gameState == "play")
  {
    if (balls.size() < 1) {
      balls.add(new Ball(launcher.position.x, launcher.position.y - 2*launcher.len, launcher.b1.w/2));
      launcherButton.resetButton();
    }


    // We must always step through time!
    box2d.step();

    course.display();


    for (Box b : boundaries)
    {
      b.display();
    }

    launcher.compress(keyPressed && key == ' ');
    launcher.display();

    leftFlipper.display();
    rightFlipper.display();

    if (leftFlipper.flipper.body.getAngle() <= leftFlipper.lowerAngle)
    {
      leftFlipper.flipper.body.setType(BodyType.STATIC);
    }
    if (rightFlipper.flipper.body.getAngle() <= rightFlipper.lowerAngle + 2*PI)
    {
      rightFlipper.flipper.body.setType(BodyType.STATIC);
    }

    for (Ball b : balls)
    {
      launcherButton.checkCollisions(b);
      leftCourseButton.checkCollisions(b);
      rightCourseButton.checkCollisions(b);
    }
    launcherButton.display();
    leftCourseButton.display();
    rightCourseButton.display();

    // Look at all balls
    for (int i = balls.size()-1; i >= 0; i--) {
      Ball b = balls.get(i);
      b.display();
      // Balls that leave the screen, we delete them
      // (note they have to be deleted from both the box2d world and our list
      if (b.done()) {
        balls.remove(i);
        scoreboard.numBalls--;
        if (scoreboard.numBalls <= 0) gameState = "end";
      }
    }

    for (Bumper b : bumpers)
    {
      b.display();
    }
  }

  scoreboard.display(gameState);
}

void keyPressed()
{
  if (key == 'q' && leftFlipper.flipper.body.getAngle() >= leftFlipper.upperAngle)
  {
    leftFlipper.flipper.body.setType(BodyType.STATIC);
  }
  if (key == 'q' && leftFlipper.flipper.body.getAngle() < leftFlipper.upperAngle)
  {
    leftFlipper.flipper.body.setType(BodyType.DYNAMIC);
    leftFlipper.reverseSpeed();
  }
  if (key == 'p' && rightFlipper.flipper.body.getAngle() >= rightFlipper.upperAngle + 2*PI)
  {
    rightFlipper.flipper.body.setType(BodyType.STATIC);
  }
  if (key == 'p' && rightFlipper.flipper.body.getAngle() > rightFlipper.upperAngle + 2*PI)
  {
    rightFlipper.flipper.body.setType(BodyType.DYNAMIC);
    rightFlipper.reverseSpeed();
    //println(rightFlipper.upperAngle + 2*PI);
    //println(rightFlipper.flipper.body.getAngle());
  }
}

void keyReleased()
{
  if (key == 'q')
  {
    leftFlipper.flipper.body.setType(BodyType.DYNAMIC);
    leftFlipper.reverseSpeed();
  }
  if (key == 'p')
  {
    rightFlipper.flipper.body.setType(BodyType.DYNAMIC);
    rightFlipper.reverseSpeed();
    //println("hi");
  }
  if (key == 'r')
  {
    gameState = "end";
  }
  if (key == ' ' && gameState == "end")
  {
    newGame();
  }
}

