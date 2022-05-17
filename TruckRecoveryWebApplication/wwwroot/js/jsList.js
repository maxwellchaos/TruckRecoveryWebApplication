
function drawMarker(index) {
    let progressCollection = document.querySelectorAll('li');
    for (let i = 0; i < progressCollection.length; i++) {
        if (i < index) {
            progressCollection[i].style.background = "url(images/correct2.png) no-repeat 0 50%";

        } else {
            progressCollection[i].style.background = "url(images/dry-clean3.png) no-repeat 0 50%";
            progressCollection[i].classList.add('makeColorLineForActiveElements');
        }
    }

}

drawMarker(+prompt('Введите число от 1 до 5', 2));



