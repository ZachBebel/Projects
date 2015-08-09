// drawLib.js "use  strict";
var app = app || {};

app.drawLib = {
    clear: function (ctx, x, y, w, h) {
        ctx.clearRect(x, y, w, h);
    },

    rect: function (ctx, x, y, w, h, col) {
        ctx.save();
        ctx.fillStyle = col;
        ctx.fillRect(x, y, w, h);
        ctx.restore();
    },

    // a generalized linear gradient function
    rectGradient: function (ctx, x, y, width, height) {

        ctx.save();

        // Create gradient - top to bottom
        var grad = ctx.createLinearGradient(x, y, x, y + height);
        grad.addColorStop(0.0, "#00478F");  // top
        grad.addColorStop(.05, "#001248");
        grad.addColorStop(.10, "#0A141A");
        grad.addColorStop(.90, "#0A141A");
        grad.addColorStop(.95, "#001248");
        grad.addColorStop(1.0, "#00478F");  // bottom

        // change this to fill entire ctx with gradient 
        ctx.fillStyle = grad;
        ctx.fillRect(x, y, width, height);
        ctx.restore();
    },

    drawBackground: function (ctx, image, x, y, width, height) {
        this.rect(ctx, x, y, width, height, 'black');
        ctx.drawImage(image, x, y, width, height);
    },

    ship: function (ctx, image, x, y, width, height, rotation) {
        // ctx.fillRect() draws from the upper left of the x,y
        // we're doing these calculations so we are drawing the
        // ship from the center x,y
        var halfW = width / 2;
        var halfH = height / 2;

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(rotation);

        if (!image) {
            rect(ctx, -halfW, -halfH, width, height, color);
        } else {
            ctx.drawImage(image, 28, 2, 17, 21,   // source
                -halfW, -halfH, width, height);   // dest
        }
        ctx.restore();
    },

    // Background rect for menus
    createOverlay: function (text, ctx, width, height) {
        // draw menu rect
        var halfW = width / 2;
        var halfH = height / 2;
        var offset = 50;

        this.rectGradient(
            ctx,
            offset,
            offset,
            width - 2 * offset,
            height - 2 * offset);

        // draw menu text
        ctx.save();
        var fontSize = 30;
        ctx.font = ctx.font = "bold " + fontSize + "px 'Source Code Pro'";
        ctx.fillStyle = "white";
        ctx.fillText(
            text,
            width / 2 - ctx.measureText(text).width / 2,
            offset + fontSize * 1.5);
        ctx.restore();
    },

    text : function(ctx, string, x, y, size, col) { 
        ctx.save();
        ctx.font = ctx.font = "bold " + size + "px 'Source Code Pro'";
        ctx.fillStyle = col;
        ctx.fillText(string, x, y);
        ctx.restore();
    },

    health : function (ctx, x, y, width, height, percent, lives) {
        var halfW = width / 2;
        var halfH = height / 2;
        var offset = 5;

        var innerColor;
        var underColor;
        var outerColor = "black";

        if (lives == 3) {
            innerColor = "#006600";
            underColor = "#FFCC00";
        } else if(lives == 2) {
            innerColor = "#FFCC00";
            underColor = "#990000";
        } else if (lives == 1) {
            innerColor = "#990000";
            underColor = "black";
        }

        // Outer Border
        this.rect(ctx, x - halfW, y - halfH, width, height, outerColor);

        // Under Amount
        this.rect(ctx,
                x + offset - halfW,
                y + offset - halfH,
                width - 2 * offset,
                height - 2 * offset,
                underColor);

        // Inner Amount
        this.rect(   ctx,
                x + offset - halfW,
                y + offset - halfH,
                (width - 2 * offset) * percent,
                height - 2 * offset,
                innerColor);
    }

};
