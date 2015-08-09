// canvaswars.js
// Dependencies: 
// Description: singleton object
// This object will be our main "controller" class and will contain references
// to most of the other objects in the game.

"use strict";

// if app exists use the existing copy
// else create a new object literal
var app = app || {};

app.canvaswars = {
	// CONSTANT properties
    WIDTH : 640, 
    HEIGHT: 480,
    dt: 1 / 60.0,  //  "delta  time” 
    canvas: undefined,
    ctx: undefined,
    app: undefined,
    ship: undefined,
    drawLib: undefined,
    utils: undefined,
    pulsar: undefined,

    // game properties

    // Game States:
    // 0 - Start Screen
    // 1 - Instructions
    // 2 - Options
    // 3 - Pause
    // 4 - Game
    // 5 - Game Over / End Screen
    gameState: 0,
    paused: false,

    // Buttons
    startButton: undefined,
    instructButton: undefined,
    optionButton: undefined,
    resumeButton: undefined,
    returnButton: undefined,
    soundToggleButton: undefined,

    // player properties
    playerBullets: [],
    cooldown: 0,
    FIRE_RATE: 6,
    maxHealth: 100,
    currentHealth: 100,
    maxLives: 3,
    lives: 3,
    score: 0,

    // enemy properties
    ENEMY_PROBABILITY_PER_SECOND: 1.0,
    enemies: [],
    sectors: [],
    enemyImage: undefined,
    enemyWaveCount: 0,
    enemyDeadCount: 0,
    wave: 1,

    // sprite animations & images
    explosions: [],
    explosionImage: undefined,
    explosionImage2: undefined,
    explosionImage3: undefined,
    backgroundImage: undefined,

    // sounds
    soundtrack: undefined,
    soundPaused: false,
    
    // methods
	init : function(ship) {
		// declare properties
		this.canvas = document.querySelector('canvas');
		this.canvas.width = this.WIDTH;
		this.canvas.height = this.HEIGHT;
		this.ctx = this.canvas.getContext('2d');
		
		// set up player ship
		this.ship = ship;

	    // create an image object 
		var image = new Image();

	    // get the ship PNG	- it was  already loaded for us 
		image.src = this.app.IMAGES['shipImage'];

	    // set .image property of ship 
		this.ship.image = image;

	    // prepare enemy image var  
		image = new Image();
		image.src = this.app.IMAGES['enemyImage'];
		this.enemyImage = image;

	    // set explosion image vars
		image = new Image();
		image.src = this.app.IMAGES['explosionImage'];
		this.explosionImage = image;
		image = new Image();
		image.src = this.app.IMAGES['explosionImage2'];
		this.explosionImage2 = image;
		image = new Image();
		image.src = this.app.IMAGES['explosionImage3'];
		this.explosionImage3 = image;

	    // background image
		image = new Image();
		image.src = this.app.IMAGES['backgroundImage'];
		this.backgroundImage = image;

	    // intialize buttons
		var size = 40, offset = 5;

        //start screen
		this.startButton = new app.Button(this.WIDTH / 2, this.HEIGHT / 2, size, "Start");
		this.instructButton = new app.Button(this.WIDTH / 2, this.HEIGHT / 2 + size + offset, size, "Instructions");
		this.optionButton = new app.Button(this.WIDTH / 2, this.HEIGHT / 2 + 2 * (size + offset), size, "Options");

        // pause screen
		this.resumeButton = new app.Button(this.WIDTH / 2, this.HEIGHT * 0.8 - (size + offset), size, "Resume");

        // multiple screens
		this.returnButton = new app.Button(this.WIDTH / 2, this.HEIGHT * 0.8, size, "Return");
		this.soundToggleButton = new app.Button(this.WIDTH / 2, this.HEIGHT / 2 + size / 2, size / 2, "On");
		this.soundToggleButton.width = 2 * size;
		this.soundToggleButton.x -= size;

        /*
	    // set up pulsar
		this.pulsar = new this.app.Emitter();
		this.pulsar.red = 255;
		this.pulsar.minXspeed = this.pulsar.minYspeed = -0.25;
		this.pulsar.maxXspeed = this.pulsar.maxYspeed = 0.25;
		this.pulsar.lifetime = 500;
		this.pulsar.expansionRate = 0.05;
		this.pulsar.numParticles = 100;
		this.pulsar.xRange = 1;
		this.pulsar.yRange = 1;
		this.pulsar.useCircles = false;
		this.pulsar.useSquares = true;

	    // 100,100 is where the particles will appear. 
		this.pulsar.createParticles({ x: 100, y: 100 });
        */

	    // Background Sound
	    // play the sound at 40%  volume, loop forever
		this.soundtrack = createjs.Sound.play("soundtrack", { loop: -1, volume: 0.4 });

		this.createSectors();

		this.newGame();

		this.update();
	},

    // Reset game
	newGame: function() {
	    this.currentHealth = this.maxHealth;
	    this.lives = this.maxLives;
	    this.score = 0;
	    this.ship.init();
	    this.ENEMY_PROBABILITY_PER_SECOND = 1.0;
	    this.enemies = [];
	    this.explosions = [];
	    this.enemyDeadCount = 0;
	    this.enemyWaveCount = 0;
	    this.wave = 1;
	},

    // Create positions for enemies to randomly spawn from
	createSectors: function() {
	    var padding = 20;

	    this.sectors.push({ x: padding,                 y: padding });    // Upper Left
	    this.sectors.push({ x: this.WIDTH/2,            y: padding });    // Upper Middle
	    this.sectors.push({ x: this.WIDTH - padding,    y: padding });    // Upper Right
	    this.sectors.push({ x: padding,                 y: this.HEIGHT/2 });    // Middle Left
	    this.sectors.push({ x: this.WIDTH - padding,    y: this.HEIGHT/2 });    // Middle Right
	    this.sectors.push({ x: padding,                 y: this.HEIGHT - padding });    // Bottom Left
	    this.sectors.push({ x: this.WIDTH / 2,          y: this.HEIGHT - padding });    // Bottom Middle
	    this.sectors.push({ x: this.WIDTH - padding,    y: this.HEIGHT - padding });    // Bottom Right
	},

    // Spawn enemies
	createEnemies: function(wave) {
	    if (Math.random() < this.ENEMY_PROBABILITY_PER_SECOND / 60 &&                // Spawn at a random probability
            this.enemyWaveCount < parseInt(this.wave * (Math.random() * 2 + 1))) {   // Spawn a random increasing number per wave

            // Spawn at a random sector
	        var sector = parseInt(Math.random() * 8);

            // Augment enemy count
	        this.enemyWaveCount++;

	        // Create enemy
	        this.enemies.push(new
                app.Enemy(this.enemyImage, this.sectors[sector].x, this.sectors[sector].y));

	        //console.log("New Enemy created! enemies.length = " + this.enemies.length);
	    }
	},

	newWave: function(){
	    this.wave++;
	    this.ENEMY_PROBABILITY_PER_SECOND += 0.1;
	    this.enemyDeadCount = 0;
	    this.enemyWaveCount = 0;
	},

	moveSprites: function(){

	    //
	    // Ship
        //

	    // Ask "Key Daemon" which keys are down 
	    if (this.app.keydown[this.app.KEYBOARD.KEY_LEFT] || this.app.keydown[65]) {     // LEFT or A
	        this.ship.moveLeft(this.dt);
	    }
	    if (this.app.keydown[this.app.KEYBOARD.KEY_RIGHT] || this.app.keydown[68]) {    // RIGHT or D
	        this.ship.moveRight(this.dt);
	    }
	    if (this.app.keydown[this.app.KEYBOARD.KEY_UP] || this.app.keydown[87]) {       // UP or W
	        this.ship.moveUp(this.dt);
	    }
	    if (this.app.keydown[this.app.KEYBOARD.KEY_DOWN] || this.app.keydown[83]) {     // DOWN or S
	        this.ship.moveDown(this.dt);
	    }

	    // clamp(val, min, max)
	    // ex. clamp(800,0,640); returns 640
	    // ex. clamp(-10,0,640); returns 0
	    var paddingX = this.ship.width/2;
	    var paddingY = this.ship.height/2;
	    this.ship.x = this.utils.clamp(this.ship.x, paddingX, this.WIDTH - paddingX);
	    this.ship.y = this.utils.clamp(this.ship.y, paddingY, this.HEIGHT - paddingY);

        //
	    // Bullets
	    //

	    this.cooldown--;

	    // poll keyboard 
	    if (this.cooldown <= 0 && (this.app.keydown[app.KEYBOARD.KEY_SPACE] || this.app.mousedown)) {
	        this.shoot(
                // center   +      raidius         *        angle
                this.ship.x + this.ship.height / 2 * Math.cos(this.ship.rotation),
                this.ship.y + this.ship.height / 2 * Math.sin(this.ship.rotation),
                // speed *  angle
                600 * Math.cos(this.ship.rotation),
                600 * Math.sin(this.ship.rotation));
	        this.cooldown = 60 / this.FIRE_RATE;  // assuming 60 FPS here
	    }

	    // move bullets
	    for(var i=0; i < this.playerBullets.length; i++){ 
	        this.playerBullets[i].update(this.dt);
	    }

	    // array.filter() returns a new array with only active bullets 
	    this.playerBullets = this.playerBullets.filter(function (bullet) {
	        return bullet.active;
	    });

        //
	    // Enemies
        //

        // update
	    for(var i=0; i < this.enemies.length; i++) {
	        this.enemies[i].rotate(this.ship.x, this.ship.y);
	        this.enemies[i].update(this.dt);
	    };

	    // array.filter() returns a new  array with only active enemies 
	    this.enemies = this.enemies.filter(function (enemy) {
	        return enemy.active;
	    });

        // check for new wave
	    if (this.enemyDeadCount == this.enemyWaveCount && this.enemyWaveCount > 1) {
	        this.newWave();
	    }

        // spawn new enemies
	    this.createEnemies(5);

	    // explosions
	    this.explosions.forEach(function (exp) {
	        exp.update(this.dt);
	    },this);

	    this.explosions = this.explosions.filter(function (exp) {
	        return exp.active;
        });
	},

	drawSprites: function() {
	    // clear screen
	    // this clear is actually redundant because our gradient fills
	    // the entire screen
	    //this.drawLib.clear(this.ctx, 0, 0, this.WIDTH, this.HEIGHT);

	    // draw background
	    this.drawLib.drawBackground(this.ctx, this.backgroundImage, 0, 0, this.WIDTH, this.HEIGHT);

        //
	    // Start Screen
	    //

	    if (this.gameState == 0) {
	        // title image
	        var width = this.WIDTH * 2/3;
	        var height = this.HEIGHT/4;
	        this.drawLib.rectGradient(this.ctx, this.WIDTH / 2 - width / 2, this.HEIGHT / 8, width, height);
	        this.drawLib.text(this.ctx, 'CANVAS WARS', this.WIDTH / 2 - 195, this.HEIGHT / 8 + 84, 59.5, 'white');

	        // draw buttons
	        this.startButton.draw(this.ctx);
	        this.instructButton.draw(this.ctx);
	        this.optionButton.draw(this.ctx);
	    }

	    //
	    // Instructions
        //

	    else if (this.gameState == 1) {
	        // draw overlay
	        this.drawLib.createOverlay("Instructions", this.ctx, this.WIDTH, this.HEIGHT);

	        // draw text
	        var size = 16;
	        var offset = size * 1.5;
	        var xoff = 135;
	        var yoff = 139;

	        this.drawLib.text(this.ctx, 'Controls:', xoff, yoff, size, 'white');
	        this.drawLib.text(this.ctx, 'Move:              W, A, S, D', xoff, yoff + offset, size, 'white');
	        this.drawLib.text(this.ctx, 'Aim & Shoot:       Mouse + Left-Click', xoff, yoff + offset * 2, size, 'white');
	        this.drawLib.text(this.ctx, 'Pause/Unpause:     P', xoff, yoff + offset * 3, size, 'white');

	        yoff += offset * 4;
	        this.drawLib.text(this.ctx, 'Objective:', xoff, yoff + offset, size, 'white');
	        this.drawLib.text(this.ctx, 'Destroy as many enemies as you can', xoff, yoff + offset * 2, size, 'white');
	        this.drawLib.text(this.ctx, 'Survive as long as you can', xoff, yoff + offset * 3, size, 'white');
	        this.drawLib.text(this.ctx, 'And acheive the highest possible score!', xoff, yoff + offset * 4, size, 'white');

	        // draw buttons
	        this.returnButton.draw(this.ctx);
	    }

	    //
	    // Options
	    //

	    else if (this.gameState == 2) {
	        // draw overlay
	        this.drawLib.createOverlay("Options", this.ctx, this.WIDTH, this.HEIGHT);

	        // draw text
	        this.drawLib.text(this.ctx, 'Sound:', this.WIDTH/2 - 45, this.HEIGHT / 2 - 25, 25, 'white');

	        // draw sound button shadow
	        var offset = 1;
	        var color = "black";
	        this.drawLib.rect(
                this.ctx,
                this.WIDTH/2 - this.soundToggleButton.width - offset,
                this.soundToggleButton.y - this.soundToggleButton.height/2 - offset,
                this.soundToggleButton.width * 2 + offset * 2,
                this.soundToggleButton.height + offset * 2,
                color);

	        // draw buttons
	        this.returnButton.draw(this.ctx);
	        this.soundToggleButton.draw(this.ctx);
	    }

	    //
	    // Pause
        //

	    else if (this.gameState == 3) {
	        // draw overlay
	        this.drawLib.createOverlay("Paused", this.ctx, this.WIDTH, this.HEIGHT);

	        // draw text
	        this.drawLib.text(this.ctx, 'Sound:', this.WIDTH / 2 - 45, this.HEIGHT / 2 - 35, 25, 'white');

	        // draw sound button shadow
	        var offset = 1;
	        var color = "black";
	        this.soundToggleButton.y -= this.soundToggleButton.height;
	        this.drawLib.rect(
                this.ctx,
                this.WIDTH / 2 - this.soundToggleButton.width - offset,
                this.soundToggleButton.y - this.soundToggleButton.height / 2 - offset,
                this.soundToggleButton.width * 2 + offset * 2,
                this.soundToggleButton.height + offset * 2,
                color);

	        // draw buttons
	        this.returnButton.text = "Quit";
	        this.returnButton.draw(this.ctx);
	        this.returnButton.text = "Return";
	        this.resumeButton.draw(this.ctx);
	        this.soundToggleButton.draw(this.ctx);
	        this.soundToggleButton.y += this.soundToggleButton.height;

	        // Unpause
	        if (this.paused == false) {
	            this.gameState = 4;
	        }
	    }

	    //
	    // Game
        //

	    else if (this.gameState == 4) {
	        // draw lives and score
	        this.drawLib.text(this.ctx, 'Waves: ' + this.wave + '', 15, 30, 20, 'white');
	        this.drawLib.text(this.ctx, 'Score: ' + this.score + '', this.WIDTH - 150, 30, 20, 'white');

	        // draw health bar
	        this.drawLib.health(this.ctx, this.WIDTH / 2, this.HEIGHT / 15, this.WIDTH / 3, this.HEIGHT / 25, this.currentHealth / this.maxHealth, this.lives);

	        // draw sprites
	        this.ship.draw(this.ctx);

	        // draw bullets
	        for (var i = 0; i < this.playerBullets.length; i++) {
	            this.playerBullets[i].draw(this.ctx);
	        }

	        // draw enemies
	        for (var i = 0; i < this.enemies.length; i++) {
	            this.enemies[i].draw(this.ctx);
	        };

	        // draw pulsar
	        //this.pulsar.updateAndDraw(this.ctx, { x: 100, y: 100 });

	        // draw explosions 
	        this.explosions.forEach(function (exp) {
	            exp.draw(this.ctx);
	        }, this);

	        // Pause
	        if (this.paused) {
	            this.gameState = 3;
	        }
	    }

	    //
	    // Game Over / End Screen
        //

	    else if (this.gameState == 5) {
	        // draw overlay
	        this.drawLib.createOverlay("Game Over", this.ctx, this.WIDTH, this.HEIGHT);

	        // draw text
	        this.drawLib.text(this.ctx, 'Wave Reached:  ' + this.wave + '', 190, this.HEIGHT/2 - 40, 25, 'white');
	        this.drawLib.text(this.ctx, 'Final Score:   ' + this.score + '', 190, this.HEIGHT / 2 - 10, 25, 'white');
	        this.drawLib.text(this.ctx, 'Zach Bebel', 20, this.HEIGHT - 20, 10, 'white');

	        // draw buttons
	        this.returnButton.text = "Main Menu";
	        this.returnButton.draw(this.ctx);
	        this.returnButton.text = "Return";
	        this.resumeButton.text = "Try Again";
	        this.resumeButton.draw(this.ctx);
	        this.resumeButton.text = "Resume";
	    }

	    //
	    // Check Button State
	    //

	    if (this.app.mousedown) {
	        if (this.startButton.isClicked()) {
	            this.gameState = 4;
	            this.paused = false;
	            this.newGame();
	        }
	        else if (this.instructButton.isClicked()) {
	            this.gameState = 1;
	        }
	        else if (this.optionButton.isClicked()) {
	            this.gameState = 2;
	        }
	        else if (this.returnButton.isClicked()) {
	            this.gameState = 0;
	            this.paused = false;
	        }
	        else if (this.resumeButton.isClicked()) {
	            if (this.gameState == 5) {
	                this.newGame();
	            }
	            this.gameState = 4;
	            this.paused = false;
	        }
	        else if (this.soundToggleButton.isClicked()) {
	            this.toggleSound();

	            if (this.soundPaused) {
	                this.soundToggleButton.text = "Off";
	                this.soundToggleButton.x += this.soundToggleButton.width;
	            } else {
	                this.soundToggleButton.text = "On";
	                this.soundToggleButton.x -= this.soundToggleButton.width;
	            }
	        }
	    }
	    
	},

	checkForCollisions : function(){
	    // nested forEach loops!
	    // "this" becomes undefined in a foreach loop
	    // "self" will preserve "this" i.e. app.blastum 
	    var self = this;

	    // bullets v. enemies 
	    this.playerBullets.forEach(function (bullet) {
	        self.enemies.forEach(function(enemy) {  
	            if  (self.collides(bullet, 0, enemy, enemy.height/2))  {

                    // Enemy Explosion
	                enemy.explode();
	                self.createExplosion(enemy.x, enemy.y, -enemy.xVelocity / 4, -enemy.yVelocity / 4);
	                bullet.active = false;
	                if (self.soundPaused == false) createjs.Sound.play("explosion");   // Play sound

	                self.score += 3 * self.wave;

                    // Augment enemy count
	                self.enemyDeadCount++;
	            }   // end if
	        });     // end forEach enemy
	    });         // end forEach bullet
        
        // enemies v. ship 
        this.enemies.forEach(function(enemy) {
            if (self.collides(enemy, enemy.height/2, self.ship, self.ship.height/2)) { 

                // Enemy Explosion
                enemy.explode();
                self.createExplosion(enemy.x, enemy.y, -enemy.xVelocity / 4, -enemy.yVelocity / 4);
                if (self.soundPaused == false) createjs.Sound.play("explosion");   // Play sound

                // Augment enemy count
                self.enemyDeadCount++;

                // Ship Take Damage
                self.currentHealth -= 5;
                if (self.currentHealth <= 0) {
                    self.currentHealth = self.maxHealth;
                    self.lives--;
                    if (self.lives <= 0) {
                        // Game Over
                        self.lives = 0;
                        self.gameState = 5;
                    }
                }
            }
        });
    },

	collides: function (a, ar, b, br) {
	    // a = sprite1
	    // b = sprite2
	    // bounding box collision detection
	    // https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection
	    // we have to adjust our bounding boxes because
	    // we're drawing sprites from their center x,y
	    //var ax = a.x - a.width/2;
	    //var ay = a.y - a.height/2;
	    //var bx = b.x - b.width/2;
	    //var by = b.y - b.height/2;
		//
	    //return  ax < bx + b.width &&
		//		ax + a.width > bx &&
		//	    ay < by + b.height &&
	    //		ay + a.height > by;


	    // Point-Circle / Circle-Circle Collisions
	    var dist = Math.sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
	    return ar + br > dist;
	},

	createExplosion: function(x, y, xVelocity, yVelocity){
	    // ExplosionSprite(image, width, height, frameWidth, frameHeight, frameDelay)
	    // global! you might want to create an ExplosionSprite property
	    // in canvaswars.js and hook it up in the loader.js “sandbox”

        // image 1
	    var exp = new app.ExplosionSprite(this.explosionImage, 84, 84, 84, 84, 1 / 7);
	    // image 2
	    //var exp = new app.ExplosionSprite(this.explosionImage2, 128, 128, 64, 64, 1 / 16);
	    // image 3
	    //var exp = new app.ExplosionSprite(this.explosionImage3, 64, 32, 256, 128, 1 / 12);

	    // faster and larger
	    //var exp = new app.ExplosionSprite(this.explosionImage, 200, 200, 84, 84, 1 / 14);
	    // ridiculously fast and large
	    //var exp = new app.ExplosionSprite(this.explosionImage, 500, 500, 84, 84, 1 / 28);
	    // slower and smaller
	    //var exp = new app.ExplosionSprite(this.explosionImage, 30, 30, 84, 84, 1 / 3);

	    exp.x = x;
	    exp.y = y;

	    // Make explosions move
	    exp.xVelocity = xVelocity;
	    exp.yVelocity = yVelocity;

	    this.explosions.push(exp);
	},

	shoot: function (x, y, xspeed, yspeed) {
        // Add bullet to array and fire
	    this.playerBullets.push(new app.Bullet(x, y, xspeed, yspeed));

	    // Play sound
	    if(this.soundPaused == false) createjs.Sound.play("bullet");
	},


    // SOUNDS
	resumeSound: function() {
	    this.soundtrack.resume();
	},

	pauseSound: function () {
	    this.soundtrack.pause();
	},

	toggleSound: function(){
	    this.soundPaused = !this.soundPaused;
	    if (this.soundPaused) {
	        this.pauseSound();
	    } else {
	        this.resumeSound();
	    }

	},


    // UPDATE
    update: function(){
        // LOOP s
        // bind() will create a new function that permanently binds the value of this to whatever the ﬁrst argument of bind() is. 
        // Thus when update() is called, the value of this will always be app.canvaswars
        requestAnimationFrame(this.update.bind(this));

        // Game
        if (this.gameState == 4) {
            // UPDATE
            this.moveSprites();

            // CHECK FOR COLLISIONS
            this.checkForCollisions();
        }

        // DRAW
        this.drawSprites();
    }

}; // end app.canvaswars