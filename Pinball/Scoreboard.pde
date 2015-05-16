// Zach Bebel
// IMD Project #1

class Scoreboard
{
  int score;
  int numBalls;
  PImage plaque;

  Scoreboard(int score_, int numBalls_)
  {
    score = score_;
    numBalls = numBalls_;
    plaque = loadImage("plaque.png");
  }

  void display(String gameState)
  {
    if (gameState == "play")
    {
      image(plaque, 0, 0, 110, 55);
      fill(0);
      textSize(20);
      text("Score:", 20, 25);
      text(score, 40, 45);
      image(plaque, width-110, 0, 110, 55);
      text("Balls Left:", width-110+10, 25);
      text(numBalls, width-60, 45);
    }
    else if (gameState == "end")
    {
      image(plaque, width/2-0.4*width, height/2-0.4*height, 0.8*width, 0.8*height);
      fill(0);
      textSize(40);
      text("Game Over!", width/2-150, height/2 - 100);
      text("Final Score:  " + scoreboard.score, width/2-150, height/2);
      textSize(32);
      text("Press SPACE to play again!", width/2-200, height/2 + 100);
    }
  }
}

