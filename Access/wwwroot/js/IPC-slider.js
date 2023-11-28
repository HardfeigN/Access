function changeImageTo(el, index) {
    var imgWidth = parseFloat($('.slider-image').css('width'));
    $(el).css('right', index * imgWidth);
}
$(window).on('resize', function () {
    $('.a-IPC-imgages').css('right', 0);
});
$(".a-IPC-img-container").mousemove(function (event) {
    var relX = event.pageX - $(this).offset().left;
    var imgWidth = parseFloat($('.slider-image').css('width'));
    var imagesCount = $(this).children('.a-IPC-imgages').children('.slider-image').length;
    var index = parseInt(relX * imagesCount / imgWidth);
    changeImageTo($(this).children('.a-IPC-imgages'), index);
});