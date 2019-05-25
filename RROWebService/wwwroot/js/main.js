function showSnackbar() {document.getElementsByClassName("snackbar")[0].style.visibility = "visible";}

function hideSnackbar() {document.getElementsByClassName("snackbar")[0].style.visibility = "hidden";}

function showButton(button) {button.style.visibility = "visible";}

function hideButton(button) {button.style.visibility = "hidden";}


function startLoad(button) {
    hideButton(button);
    showSnackbar();
}

function endLoad(button) {
    showButton(button);
    hideSnackbar();
}