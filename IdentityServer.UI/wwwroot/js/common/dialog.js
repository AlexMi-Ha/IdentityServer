document.addEventListener('DOMContentLoaded', () => {

    document.getElementById('dialog-button-close').addEventListener('click', () => {
        document.getElementById('dialog-window').classList.remove('active');
    });

    document.querySelectorAll('.dialog-button-open').forEach(btn => {
        const dialogWindow = document.getElementById('dialog-window');
        const id = btn.getAttribute('data-dialog-id');
        btn.addEventListener('click', () => {
            loadDialogContent(dialogWindow, id);
        });
    });
});

function loadDialogContent(window, dialogContentId) {
    const url = window.getAttribute('data-dialogcontent-url');
    window.classList.add('active');
    fetch(
        url.replaceAll('{id}', dialogContentId),
        {
            method: 'get',
        }
    ).then(res => res.text())
    .then(res => {
        document.getElementById('dialog-content-window').innerHTML = res;
    }).catch(err => {
        console.log(err);
    })
}


function removeRole(role, userId) {
    fetch(`/admin/api/users/${userId}/roles/${role}`, {
        method: 'DELETE',
    })
        .then(e => e.text())
        .then(e => {
            document.getElementById('role-dialog-container').outerHTML = e;
        });
}

    function addToRole(role, userId) {
    if (role === 'lel') {
    return;
}
    fetch(`/admin/api/users/${userId}/roles/${role}`, {
    method: 'POST',
})
    .then(e => e.text())
    .then(e => {
    document.getElementById('role-dialog-container').outerHTML = e;
});
}