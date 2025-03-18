// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelectorAll(".accordion-button").forEach(button => {
    button.addEventListener("click", () => {
        const content = button.nextElementSibling;
        content.style.display = content.style.display === "none" ? "block" : "none";
    });
});