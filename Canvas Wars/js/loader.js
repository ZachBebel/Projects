/*
loader.js
variable 'app' is in global scope - i.e. a property of window.
app is our single global object literal - all other functions and properties of 
the game will be properties of app.
*/
"use strict";

// if app exists use the existing copy
// else create a new object literal
var app = app || {};

// CONSTANTS 
app.KEYBOARD = {
    "KEY_LEFT": 37,
    "KEY_UP": 38,
    "KEY_RIGHT": 39,
    "KEY_DOWN": 40,
    "KEY_SPACE": 32
};

app.IMAGES = {
    shipImage: "images/Hunter1.png",
    enemyImage: "images/Drone1.png",
    explosionImage: "images/explosion.png",
    explosionImage2: "images/explosion2.png",
    explosionImage3: "images/explosion3.png",
    backgroundImage: "images/space.png"
};

// app.keydown  array  to  keep  track  of  which  keys  are  down
// this is called a "key daemon"
// canvaswars.js will "poll" this array every frame
// this works because JS has "sparse arrays" - not every language does 
app.keydown = [];

window.onload = function () {
    // This is the "sandbox" where we hook our modules up so that 
    // we don't have any hard-coded dependencies in the modules themselves 
    app.ship.drawLib = app.drawLib;
    app.canvaswars.app = app;
    app.canvaswars.drawLib = app.drawLib;
    app.canvaswars.utils = app.utils;
    app.Emitter.utils = app.utils;

    // Preload Images and Sound
    app.queue  =  new  createjs.LoadQueue(false); 
    app.queue.installPlugin(createjs.Sound); 
    app.queue.on("complete", function () {
        app.canvaswars.init(app.ship);
    });

    // load image amd sound files 
    app.queue.loadManifest([
        { id: "shipImage", src: "images/Hunter1.png" },
        { id: "enemyImage", src: "images/Drone1.png" },
        { id: "explosionImage", src: "images/explosion.png" },
        { id: "explosionImage2", src: "images/explosion2.png" },
        { id: "explosionImage3", src: "images/explosion3.png" },
        { id: "backgroundImage", src: "images/space.png" },     // http://7-themes.com/data_images/out/1/6771739-awesome-blue-space-wallpaper.jpg
        { id: "bullet", src: "sounds/laser4.mp3" },
        { id: "explosion", src: "sounds/fireball4.mp3" },
        { id: "soundtrack", src: "sounds/soundtrack.mp3" }
    ]);

    // Mouse variables
    app.mousedown = false;
    app.mouse = { x: 0, y: 0 };

    // Keyboard event listeners 
    window.addEventListener("keydown", function (e) {
        app.keydown[e.keyCode] = true;
    });
    window.addEventListener("keyup", function(e){
        app.keydown[e.keyCode] = false;

        //if (e.keyCode == 77) app.canvaswars.toggleSound();  //  M - MUTE/UNMUTE
        if (e.keyCode == 80) app.canvaswars.paused = !app.canvaswars.paused;  //  P - PAUSE/UNPAUSE
    });

    // On blur
    window.addEventListener("blur", function (e) {
        if (app.canvaswars != undefined) {
            app.canvaswars.paused = true;
        }
    });

    // Mouse click event listeners
    window.addEventListener("mousedown", function (e) {
        app.mousedown = true;
    }, false);
    window.addEventListener("mouseup", function (e) {
        app.mousedown = false;
    }, false);

    // Mouse move - Player rotation
    window.addEventListener("mousemove", function (e) {
        app.mouse.x = e.pageX - e.target.offsetLeft;
        app.mouse.y = e.pageY - e.target.offsetTop;

        if (app.canvaswars.ship != undefined) {
            app.canvaswars.ship.rotate(app.mouse);
        }
    }, false);
}
