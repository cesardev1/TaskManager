async function handleApiError(response) {
    let errorMessages = '';
    
    if(response.status === 400) {
        errorMessages = await response.text();
    } else if(response.status === 404) {
        errorMessages = resourceNotFound;
    } else {
        errorMessages = unexpectError;
    }
    
    showErrorMessage(errorMessages);
}

function showErrorMessage(message){
    Swal.fire({
        icon: 'error',
        title: 'Error...',
        text: message
    })
}