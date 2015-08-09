"use strict";

app.Button = function () {

    function Button(x, y, size, text) {
        // ivars
        this.height = size;
        this.width = 5 * this.height;
        this.x = x;
        this.y = y;

        this.text = text;
        this.fontSize = this.height * 0.5;
        this.color = '#4C4C4C';
        this.highlightColor = '#808080';
        this.borderColor = 'black';
        this.textColor = 'white';

        this.mouseOver = false;
    };

    var b = Button.prototype;

    b.draw = function (ctx) {
        var halfW = this.width / 2;
        var halfH = this.height / 2;

        // draw button rect
        var offset = 2;
        
        // Outer Border
        ctx.save();
        ctx.fillStyle = this.borderColor;
        ctx.fillRect(this.x - halfW, this.y - halfH, this.width, this.height);

        // check mouse hover
        this.hover(app.mouse.x, app.mouse.y, ctx);

        // Inner Fill
        ctx.fillRect(
                this.x + offset - halfW,
                this.y + offset - halfH,
                this.width - 2 * offset,
                this.height - 2 * offset);
        ctx.restore();

        // draw button text
        this.drawText(ctx);
    };

    b.hover = function (mouseX, mouseY, ctx) {
        var halfW = this.width / 2;
        var halfH = this.height / 2;

        if (mouseX >= this.x - halfW && mouseX <= this.x + halfW &&
            mouseY >= this.y - halfH && mouseY <= this.y + halfH) {

            this.mouseOver = true;
            ctx.fillStyle = this.highlightColor;
        } else {
            this.mouseOver = false;
            ctx.fillStyle = this.color;
        }
    }

    b.drawText = function (ctx) {
        ctx.save();
        ctx.font = "bold " + this.fontSize + "px 'Source Code Pro'";
        ctx.fillStyle = this.textColor;
        ctx.fillText(
            this.text, 
            this.x - ctx.measureText(this.text).width/2, 
            this.y + this.height * 0.17);
        ctx.restore();
    };

    b.isClicked = function () {
        if (this.mouseOver) {
            this.mouseOver = false;
            return true;
        } else {
            return false;
        }
    };

    return Button;

}();

