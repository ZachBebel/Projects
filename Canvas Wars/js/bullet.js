// bullet.js 

"use strict";

var app = app || {};

// another IIFE! 
app.Bullet  =   function(){

    function Bullet(x, y, xspeed, yspeed){
        // ivars - unique for every instance 
        this.x = x;
        this.y = y;
        this.active  =  true;
        this.xVelocity = xspeed;
        this.yVelocity = yspeed; 
        this.width = 4;
        this.height = 4; 
        this.color = "#FFF";
    }  // end Bullet Constructor
    
    var p = Bullet.prototype; 
    
    p.update = function(dt) {
        this.x += this.xVelocity * dt; 
        this.y += this.yVelocity * dt;
        this.active = this.active && inBounds(this.x, this.y);
    };
    
    p.draw = function(ctx) { 
        ctx.fillStyle = this.color;
        ctx.fillRect(this.x, this.y, this.width, this.height);
    };
    
    // private method 
    function inBounds(x, y){
        var bouding = 10;

        return x >= -bouding && x <= 640 + bouding &&
               y >= -bouding && y <= 480 + bouding;
    };
    
    return Bullet;
}();
