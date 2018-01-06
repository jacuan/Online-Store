$(function () {
    var $block = $("#adv"),
        $slides = $block.find(".slides"),
        $ul = $slides.find("ul"),
        _width = $slides.width(),
        _left = _width * -1,
        _animateSpeed = 400,

        timer, _showSpeed = 3000,
        _stop = false;

    $ul.find("li:first").before($ul.find("li:last")).end().css({
        left: _left,
        width: _width * ($ul.find("li") + 1)
    });

    var $prev = $block.find("a.prev").click(function () {
        $ul.stop(false, true).animate({ "left": _left + _width }, _animateSpeed, function () {
            $ul.find("li:first").before($ul.find("li:last")).end().css("left", _left);
            if (!_stop) {
                clearTimeout(timer);
                timer = setTimeout(move, _showSpeed);
            }
        });
        return false;
    });

    var $next = $block.find("a.next").click(function () {
        $ul.stop(false, true).animate({ "left": _left - _width }, _animateSpeed, function () {
            $ul.find("li:last").after($ul.find("li:first")).end().css("left", _left);
            if (!_stop) {
                clearTimeout(timer);
                timer = setTimeout(move, _showSpeed);
            }
        });
        return false;
    });

    $block.hover(function () {
        _stop = true;
        clearTimeout(timer);
    }, function () {
        _stop = false;
        timer = setTimeout(move, _showSpeed);
    }).find("a").focus(function () {
        this.blur();
    });

    function move() {
        $next.click();
    };

    timer = setTimeout(move, _showSpeed);
});