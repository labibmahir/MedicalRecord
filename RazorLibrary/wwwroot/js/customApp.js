function togglePasswordVisibility() {
    var passwordInput = $("#password");
    var showEyeIcon = $("#showEye");
    var hideEyeIcon = $("#hideEye");

    if (passwordInput.attr("type") === "password") {
        passwordInput.attr("type", "text");
        showEyeIcon.hide();
        hideEyeIcon.show();
    } else {
        passwordInput.attr("type", "password");
        hideEyeIcon.hide();
        showEyeIcon.show();
    }
}