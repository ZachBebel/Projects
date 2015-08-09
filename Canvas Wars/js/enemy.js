"use strict";

app.Enemy = function(){

	function Enemy(image, x, y) {
		// ivars
		this.active = true;
		this.age = Math.floor(Math.random() * 128);
		
		this.color = "#A2BA2B";
		
		this.width = 34;
		this.height = 40;
		this.x = x;
		this.y = y;
		this.xVelocity = 0
		this.yVelocity = 0;
		this.rotation = 0;
		this.speed = app.utils.getRandom(100, 200);
		this.image = image;
	};

	var p = Enemy.prototype;
	
	p.draw = function(ctx) {
	    var halfW = this.width/2;
	    var halfH = this.height/2;
	    
	    ctx.save();
	    ctx.translate(this.x, this.y);
	    ctx.rotate(this.rotation - Math.PI / 2);

	    if(!this.image){
	    	ctx.fillStyle = this.color;
	    	ctx.fillRect(-halfW, -halfH, this.width, this.height);
	    } else{
	        ctx.drawImage(this.image, 52, 98, 17, 20,   // source
                -halfW, -halfH, this.width, this.height);   // dest
	    }

	    ctx.restore();
	};

    // Seek Player
	p.rotate = function (playerX, playerY) {
	    var dir = {};
	    dir.x = playerX - this.x;
	    dir.y = playerY - this.y;
	    var target = Math.atan2(dir.y, dir.x);

	    this.rotation = target;
	}
	
	p.update = function(dt) {
	    this.xVelocity = this.speed * Math.cos(this.rotation);
	    this.yVelocity = this.speed * Math.sin(this.rotation);
		this.x += this.xVelocity * dt;
		this.y += this.yVelocity * dt;
		this.age++;
	};
	  
	p.explode = function () {
	    this.active = false;
	};
	
	return Enemy;

}();
