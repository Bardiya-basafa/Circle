document.addEventListener("DOMContentLoaded", function () {
    document.getElementById('posts-container').addEventListener('click', function (event) {
        if (event.target.closest('.like-button')) {
            event.preventDefault();

            let button = event.target.closest('.like-button');
            let form = button.closest('form');
            let postId = form.querySelector('input[name="postId"]').value();
            let postContainer = form.getElementById('post-' + postId);

            fetch(form.action, {
                method: 'POST',
                headers: {},
                body: new FormData(form)
            })
                .then(res => res.text())
                .then(html => postContainer.innerHTML = html)
                .catch
                (err => console.log(err));

        }
    })
})