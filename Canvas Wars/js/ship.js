// ship.js
// Dependencies: 
// Description: singleton object that is a module of app
// properties of the ship and what it needs to know how to do go here

"use strict";

// if app exists use the existing copy
// else create a new object literal
var app = app || {};

// the 'ship' object literal is now a property of our 'app' global variable
app.ship = {
    x: 320,
    y: 240,
    width: 34,
    height: 42,
    speed: 250,
    image: undefined,
    drawLib: undefined,
    exhaust: undefined,
    color: "yellow",

    gun: undefined,

    rotation: 0,


	init: function(){
        // set up emitter for exhaust
	    this.exhaust = new app.Emitter(); // oops - global 
	    this.exhaust.numParticles = 100;
	    this.exhaust.red = 255;
	    this.exhaust.green = 150;
	    this.exhaust.createParticles(this.emitterPoint());
	},

	emitterPoint : function(){
	    // 2 pixels underneath the bottom of the ship 
	    return {
	        x: this.x,
	        y: this.y + this.height / 2 + 2
	    };
    },

    draw: function(ctx) {
        this.drawLib.ship(ctx, this.image, this.x, this.y, this.width, this.height, this.rotation + Math.PI / 2);
        //this.exhaust.updateAndDraw(ctx, this.emitterPoint());
    },

    moveLeft: function (dt) {
        this.x -= this.speed * dt;
    },
    moveRight: function (dt) {
        this.x += this.speed * dt;
    },
    moveUp: function (dt) {
        this.y -= this.speed * dt;
    },
    moveDown: function (dt) {
        this.y += this.speed * dt;
    },

    rotate: function (mouse) {
        var dir = {};
        dir.x = mouse.x - this.x;
        dir.y = mouse.y - this.y;
        var target = Math.atan2(dir.y, dir.x);

        this.rotation = target;
    }

}; // end app.ship