// Zach Bebel
// IMD Project #1

//This class puts together a flipper and joint with a motor 
//Upper and lower angles (in radians) constrain the motion 
//Steve Kurtz 2013

class FlipperAssembly {
  RevoluteJoint joint;
  Flipper flipper;  //multi-shape object for flipper 
  Box box;          // anchor/hinge 
  float speed;
  float lowerAngle;
  float upperAngle;
  RevoluteJointDef rjd;

  FlipperAssembly(float x, float y, float speed_, float lowerAngle_, float upperAngle_) {
    flipper = new Flipper(x, y, false);
    box = new Box(x, y, 10, 10, 0, true);
    lowerAngle = lowerAngle_;
    upperAngle = upperAngle_;
    speed = speed_; 
    rjd = new RevoluteJointDef();
    rjd.initialize(flipper.body, box.body, flipper.body.getPosition());
    rjd.motorSpeed = speed;
    rjd.maxMotorTorque = 200000.0; //could tweak this value
    rjd.enableMotor = true;      
    rjd.lowerAngle = lowerAngle_;
    rjd.upperAngle = upperAngle_;
    rjd.enableLimit = true;
    joint = (RevoluteJoint) box2d.world.createJoint(rjd);
    
    if(upperAngle > 0) flipper.rotateBody(0);
    else flipper.rotateBody(PI);
  }

  void reverseSpeed() {
    speed*=-1;
    joint.setMotorSpeed(speed);
  }


  void display() {
    box.display();
    flipper.display();
  }
}
